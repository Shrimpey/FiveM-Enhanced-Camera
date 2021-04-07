using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using MenuAPI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using CustomCamera.langs;

namespace CustomCamera
{
    public class MainMenu : BaseScript {

        #region variables

        // Private variables
        private Menu Menu;
        private static MenuCheckboxItem leadCam;
        private static MenuCheckboxItem chaseCam;
        private static MenuCheckboxItem droneCam;
        private static Control MenuToggleControl;
        private static bool chaseCameraConfigEnabled;
        private static bool droneCameraConfigEnabled;

        // Public variables
        public static CustomCam CustomCamMenu { get; private set; }
        public static DroneCam DroneCamMenu { get; private set; }
        public static Camera driftCamera = null;
        public static Camera chaseCamera = null;
        public static Camera droneCamera = null;
        public static float userTilt = 0.0f;
        public static float userYaw = 0.0f;
        public static bool userLookBehind = false;
        public static Hashtable langData;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainMenu() {

            if (langData == null) {
                // Load language data
                int langId = GetCurrentLanguage();

                switch (langId) {
                    case 9:
                        langData = new zh_TW().data();
                        break;
                    case 12:
                        langData = new zh_CN().data();
                        break;
                    default:
                        langData = new en_US().data();
                        break;
                }
            }

            // Disable menu opening via gamepad (interferes with vMenu)
            MenuController.EnableMenuToggleKeyOnController = false;
            // Setup menu open/close key
            SetConfigParameters();
            // Setup main menu and submenus
            CreateSubmenus();
            // Register console command
            RegisterCommand("enhancedCam", new Action<int>((source) => {
                if (MenuController.IsAnyMenuOpen()) {
                    MenuController.CloseAllMenus();
                } else {
                    MenuController.MainMenu.OpenMenu();
                }
            }), false);
            // Right align menu
            try {
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            }catch(Exception e) {
                Debug.WriteLine("[EnhancedCamera] Exception: Cannot align menu to the right.");
            }
            // Initiate tick
            Tick += OnTick;
            Tick += GeneralUpdate;
            Tick += SlowUpdate;
        }

        /// <summary>
        /// Main OnTick task runs every game tick and handles all the menu stuff.
        /// </summary>
        /// <returns></returns>
        private async Task OnTick() {
            Game.DisableControlThisFrame(0, MenuToggleControl);
        }
        
        #region Setup main menu and submenus

        /// <summary>
        /// Add the menu to the menu pool and set it up correctly.
        /// Also add and bind the menu buttons.
        /// </summary>
        /// <param name="submenu"></param>
        /// <param name="menuButton"></param>
        private void AddMenu(Menu parentMenu, Menu submenu, MenuItem menuButton) {
            parentMenu.AddMenuItem(menuButton);
            MenuController.AddSubmenu(parentMenu, submenu);
            MenuController.BindMenuItem(parentMenu, submenu, menuButton);
            submenu.RefreshIndex();
        }

        /// <summary>
        /// Creates all the submenus of main menu
        /// </summary>
        /// <returns></returns>
        private void CreateSubmenus() {
            // Create the menu.
            Menu = new Menu(_t("MAIN_MENU_TITLE"), _t("MAIN_MENU_DESC"));
            MenuController.AddMenu(Menu);

            #region checkbox items

            // Enabling angular drift cam
            leadCam = new MenuCheckboxItem(_t("MAIN_MENU_ENABLE_LEAD"), _t("MAIN_MENU_ENABLE_LEAD_DESC"), false);
            // Enabling chase cam
            chaseCam = new MenuCheckboxItem(_t("MAIN_MENU_ENABLE_CHASE"), _t("MAIN_MENU_ENABLE_CHASE"), false);
            // Enabling chase cam
            droneCam = new MenuCheckboxItem(_t("MAIN_MENU_ENABLE_DRONE"), _t("MAIN_MENU_ENABLE_DRONE"), false);

            #endregion

            #region adding menu items
            // Checkboxes
            Menu.AddMenuItem(leadCam);
            if(chaseCameraConfigEnabled)
                Menu.AddMenuItem(chaseCam);
            if(droneCameraConfigEnabled)
                Menu.AddMenuItem(droneCam);
            
            // Custom cam parameters menu
            CustomCamMenu = new CustomCam();
            Menu customCamMenu = CustomCamMenu.GetMenu();
            MenuItem buttonCustom = new MenuItem(_t("MAIN_MENU_LEAD_CHASE_CONF"), _t("MAIN_MENU_LEAD_CHASE_CONF_DESC"))
            {
                Label = "→→→"
            };
            AddMenu(Menu, customCamMenu, buttonCustom);

            // Drone cam parameters menu
            DroneCamMenu = new DroneCam();
            Menu droneCamMenu = DroneCamMenu.GetMenu();
            MenuItem buttonDrone = new MenuItem(_t("MAIN_MENU_DRONE_CONF"), _t("MAIN_MENU_DRONE_CONF_DESC"))
            {
                Label = "→→→"
            };

            if (droneCameraConfigEnabled)
                AddMenu(Menu, droneCamMenu, buttonDrone);

            // Credits
            MenuItem credits = new MenuItem(_t("MAIN_MENU_CREDITS"),  _t("MAIN_MENU_CREDITS_DESC")) {};
            Menu.AddMenuItem(credits);

            #endregion

            #region handling menu changes

            // Handle checkbox changes
            Menu.OnCheckboxChange += (_menu, _item, _index, _checked) => {
                if (_item == leadCam)
                {
                    CustomCam.LeadCam = _checked;
                    chaseCam.Checked = false;
                    droneCam.Checked = false;
                    CustomCam.ChaseCam = false;
                    DroneCam.DroneCamVar = false;

                    if (!_checked){ ResetCameras(); }
                }
                if (_item == chaseCam)
                {
                    CustomCam.ChaseCam = _checked;
                    leadCam.Checked = false;
                    droneCam.Checked = false;
                    CustomCam.LeadCam = false;
                    DroneCam.DroneCamVar = false;

                    if (!_checked){
                        ResetCameras();
                    }else{
                        CustomCam.target = CustomCam.GetClosestVehicle(2000, CustomCam.maxAngle);
                    }
                }
                if (_item == droneCam)
                {
                    DroneCam.DroneCamVar = _checked;
                    chaseCam.Checked = false;
                    leadCam.Checked = false;
                    CustomCam.ChaseCam = false;
                    CustomCam.LeadCam = false;

                    if (!_checked){ ResetCameras(); }

                }
            };
            #endregion
        }

        #endregion
        
        #region math functions

        public class CamMath
        {
            public const float DegToRad = (float)Math.PI / 180.0f;

            /// <summary>
            /// Lerps two float values by a step
            /// </summary>
            /// <returns>lerped float value in between two supplied</returns>
            public static float Lerp(float current, float target, float by)
            {
                return current * (1 - by) + target * by;
            }

            /// <summary>
            /// Calculates angle between two vectors
            /// </summary>
            /// <returns>Angle between vectors in degrees</returns>
            public static float AngleBetween(Vector3 a, Vector3 b)
            {
                double sinA = a.X * b.Y - b.X * a.Y;
                double cosA = a.X * b.X + a.Y * b.Y;
                return (float)Math.Atan2(sinA, cosA) / DegToRad;
            }

            public static Vector3 RotateRadians(Vector3 v, float degree)
            {
                float ca = Cos(degree);
                float sa = Sin(degree);
                return new Vector3(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y, v.Z);
            }

            public static Vector3 RotateAroundAxis(Vector3 v, Vector3 axis, float angle)
            {
                return Vector3.TransformCoordinate(v, Matrix.RotationAxis(Vector3.Normalize(axis), angle));
            }

            public static float Fmod(float a, float b)
            {
                return (a - b * Floor(a / b));
            }

            public static Vector3 QuaternionToEuler(Quaternion q)
            {
                double r11 = (-2 * (q.X * q.Y - q.W * q.Z));
                double r12 = (q.W * q.W - q.X * q.X + q.Y * q.Y - q.Z * q.Z);
                double r21 = (2 * (q.Y * q.Z + q.W * q.X));
                double r31 = (-2 * (q.X * q.Z - q.W * q.Y));
                double r32 = (q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);

                float ax = (float)Math.Asin(r21);
                float ay = (float)Math.Atan2(r31, r32);
                float az = (float)Math.Atan2(r11, r12);

                return new Vector3(ax / DegToRad, ay / DegToRad, az / DegToRad);
            }
        }

        #endregion

        #region camera switching

        public static void SwitchCameraToDrift()
        {
            SwitchToGameplayCam();
            CustomCam.LeadCam = true;
            leadCam.Checked = true;
        }

        public static void SwitchCameraToChase()
        {
            SwitchToGameplayCam();
            CustomCam.ChaseCam = true;
            chaseCam.Checked = true;
        }

        public static void SwitchToGameplayCam()
        {
            CustomCam.LeadCam = false;
            CustomCam.ChaseCam = false;
            DroneCam.DroneCamVar = false;
            ResetCameras();
            leadCam.Checked = false;
            chaseCam.Checked = false;
            droneCam.Checked = false;
        }

        #endregion

        #region camera operations

        /// <summary>
        /// Creates a base camera for lead and chase cam that is not
        /// attached to any entity
        /// </summary>
        /// <returns></returns>
        public static Camera CreateNonAttachedCamera()
        {
            // Create new camera as a copy of GameplayCamera
            Camera newCam = World.CreateCamera(GameplayCamera.Position, GameplayCamera.Rotation, CustomCam.fov);
            newCam.IsActive = true;
            RenderScriptCams(true, true, 500, true, true);
            return newCam;
        }

        /// <summary>
        /// Used to reset lead and chase camera
        /// </summary>
        /// <returns></returns>
        public static void ResetCameras()
        {
            RenderScriptCams(false, true, 500, true, true);
            driftCamera = null;
            chaseCamera = null;
            droneCamera = null;
            World.DestroyAllCameras();
            SetFocusArea(GameplayCamera.Position.X, GameplayCamera.Position.Y, GameplayCamera.Position.Z, 0, 0, 0);
            EnableGameplayCam(true);
            UnlockMinimapAngle();
            ClearFocus();
            Game.Player.CanControlCharacter = true;
        }

        private const float USER_YAW_RETURN_INTERPOLATION = 0.015f;
        private static float yawReturnTimer = 0f;

        /// <summary>
        /// Additional Update function, currently takes care
        /// of user's analog stick up and down movement to
        /// control the camera tilt
        /// </summary>
        /// <returns></returns>
        private async Task GeneralUpdate()
        {
            if (Menu != null)
            {
                if (CustomCam.LeadCam || CustomCam.ChaseCam)
                {
                    // User controls the tilt offset
                    float tiltControl = ((float)(GetControlValue(1, 2) / 256f) - 0.5f);
                    float yawControl = ((float)(GetControlValue(1, 1) / 256f) - 0.5f);
                    userLookBehind = IsControlPressed(1, 26);

                    if ((Math.Abs(tiltControl) > 0.01f) || (Math.Abs(yawControl) > 0.01f))
                    {
                        //Account for difference in gamepad and mouse acceleration
                        if (IsInputDisabled(1))
                        {
                            userTilt -= tiltControl * 12f;
                            userYaw -= yawControl * 32;
                        }
                        else
                        {
                            userTilt -= tiltControl;
                            userYaw -= yawControl * 4f;
                        }
                        userTilt = (Math.Abs(userTilt) > 80f) ? (Math.Sign(userTilt) * 80f) : (userTilt);

                        userYaw = (CamMath.Fmod((userYaw + 180.0f), 360.0f) - 180.0f);
                        yawReturnTimer = 1f;    // Set the timer before yaw starts to return to 0f

                        // Slow return of user yaw to 0f
                    }
                    else if ((Math.Abs(yawControl) <= 0.01f) && (Math.Abs(userYaw) > (USER_YAW_RETURN_INTERPOLATION + 0.01f)))
                    {
                        // Only return to 0f if user is not moving
                        int vehicleEntity = GetVehiclePedIsIn(PlayerPedId(), false);
                        if (yawReturnTimer <= 0f)
                        {
                            float speedModifier = (Math.Abs(GetEntityVelocity(vehicleEntity).Length()) < 3f) ? (Math.Abs(GetEntityVelocity(vehicleEntity).Length()) / 3f) : (1f);
                            userYaw = Math.Sign(userYaw) * CamMath.Lerp(Math.Abs(userYaw), 0f, USER_YAW_RETURN_INTERPOLATION * speedModifier);
                        }
                        else
                        {
                            yawReturnTimer -= USER_YAW_RETURN_INTERPOLATION;
                        }
                    }
                }
            }
            else
            {
                await Delay(1);
            }
        }

        private async Task SlowUpdate()
        {
            // Refocus render distance of the camera (too heavy for normal update)
            if (Menu != null)
            {
                if (DroneCam.DroneCamVar)
                {
                    if (droneCamera != null)
                    {
                        SetFocusArea(droneCamera.Position.X, droneCamera.Position.Y, droneCamera.Position.Z, 0, 0, 0);
                        await Delay(250);
                    }
                }
            }
            else
            {
                await Delay(1);
            }
        }

        #endregion

        #region other functions

        public static void Notify(string message)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentString3(message);
            AddTextComponentSubstringPlayerName("Enhanced Camera");
            DrawNotification(false, false);
        }

        private static Dictionary<string, string> LoadConfig(string filename = "config.ini") {
            string stringEntries = null;
            stringEntries = LoadResourceFile("enhancedcamera", filename);
            Dictionary<string, string> entries = new Dictionary<string, string>();

            var splitted = stringEntries
                .Split('\n')
                .Where((line) => !line.Trim().StartsWith("#"))
                .Select((line) => line.Trim().Split('='))
                .Where((line) => line.Length == 2);

            foreach (var tuple in splitted) {
                entries.Add(tuple[0], tuple[1]);
            }
            return entries;
        }

        private static void SetConfigParameters() {
            Dictionary<string, string> config = LoadConfig();

            // Set menu key
            config.TryGetValue("toggleMenu", out string value);
            if (int.TryParse(value, out int result)) {
                MenuToggleControl = (Control)result;
            } else {
                MenuToggleControl = (Control)344;
            }
            MenuController.MenuToggleKey = MenuToggleControl;

            // Set chase and drone camera bools
            config.TryGetValue("chaseCameraEnabled", out string chaseCamEnabledStr);
            if (int.TryParse(chaseCamEnabledStr, out int chaseCamEnabled)) {
                chaseCameraConfigEnabled = (chaseCamEnabled==1)?(true):(false);
            } else {
                chaseCameraConfigEnabled = true;
            }

            config.TryGetValue("droneCameraEnabled", out string droneCameraEnabledStr);
            if (int.TryParse(droneCameraEnabledStr, out int droneCameraEnabled)) {
                droneCameraConfigEnabled = (droneCameraEnabled == 1) ? (true) : (false);
            } else {
                droneCameraConfigEnabled = true;
            }

        }

        public static async Task<string> GetUserInput(string windowTitle, string defaultText, int maxInputLength)
        {
            // Create the window title string.
            var spacer = "\t";
            AddTextEntry($"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", $"{windowTitle ?? "Enter"}:{spacer}(MAX {maxInputLength.ToString()} Characters)");

            // Display the input box.
            DisplayOnscreenKeyboard(1, $"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", "", defaultText ?? "", "", "", "", maxInputLength);
            await Delay(0);

            // Wait for a result.
            while (true){
                int keyboardStatus = UpdateOnscreenKeyboard();
                switch (keyboardStatus){
                    case 3: // not displaying input field anymore somehow
                    case 2: // cancelled
                        return null;
                    case 1: // finished editing
                        return GetOnscreenKeyboardResult();
                    default:
                        await Delay(0);
                        break;
                }
            }
        }

        private static string _t(string key) {
            return Language.get(key);
        }

        #endregion
    }
}
