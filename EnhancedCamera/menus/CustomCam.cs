using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using MenuAPI;
using Newtonsoft.Json;

namespace CustomCamera
{
    public static class CameraConstraints
    {
        public const float ROLL_MIN = (-50f);
        public const float ROLL_MAX = (50f);
        public const float PITCH_MIN = (-65f);
        public const float PITCH_MAX = (65f);

        public static float ClampRoll(float roll)
        {
            roll = (roll < ROLL_MIN) ? (ROLL_MIN) : (roll);
            roll = (roll > ROLL_MAX) ? (ROLL_MAX) : (roll);
            return roll;
        }
        public static float ClampPitch(float pitch)
        {
            pitch = (pitch < PITCH_MIN) ? (PITCH_MIN) : (pitch);
            pitch = (pitch > PITCH_MAX) ? (PITCH_MAX) : (pitch);
            return pitch;
        }
        public static bool OverClampCheck(float roll, float pitch)
        {
            return ((roll < ROLL_MIN) ||
                        (roll > ROLL_MAX) ||
                        (pitch < PITCH_MIN) ||
                        (pitch > PITCH_MAX));
        }
        public static bool CrashCheck(int veh)
        {
            return ((GetEntityRoll(veh) < ROLL_MIN) ||
                        (GetEntityRoll(veh) > ROLL_MAX) ||
                        (GetEntityPitch(veh) < PITCH_MIN) ||
                        (GetEntityPitch(veh) > PITCH_MAX));
        }
    }

    public class CustomCam : BaseScript
    {
        #region variables

        // Public variables
        public static bool LeadCam = false;
        public static bool ChaseCam = false;

        // Private variables
        private Menu menu;
        private Dictionary<MenuItem, KeyValuePair<string, CameraInfo>> scMenuItems = new Dictionary<MenuItem, KeyValuePair<string, CameraInfo>>();
        private Menu savedCamerasMenu;
        private Menu selectedCameraMenu = new Menu("Manage Camera", "Manage this saved camera.");
        private static KeyValuePair<string, CameraInfo> currentlySelectedCamera = new KeyValuePair<string, CameraInfo>();

        #endregion

        #region GUI updating

        // GUI parameters
        private MenuCheckboxItem lockPosOffsetCheckbox;
        private MenuCheckboxItem linearPosCheckbox;
        private MenuCheckboxItem pedLockCheckbox;
        private MenuListItem angCamModifierList;
        private MenuListItem angCamInterpolationList;
        private MenuListItem rollInterpolationList;
        private MenuListItem pitchInterpolationList;
        private MenuListItem chaseCamOffsetList;
        private MenuListItem posInterpolationList;
        private MenuListItem customCamFOVList;
        private MenuListItem customCamForwardOffsetList;
        private MenuListItem customCamSideOffsetList;
        private MenuListItem customCamUpOffsetList;
        private MenuListItem chaseCamMaxAngleList;

        // Update params
        private void UpdateParams()
        {
            // Reset camera to update params
            MainMenu.ResetCameras();
            if (LeadCam)
            {
                MainMenu.driftCamera = MainMenu.CreateNonAttachedCamera();
                MainMenu.driftCamera.IsActive = true;
            }
            else if (ChaseCam)
            {
                MainMenu.chaseCamera = MainMenu.CreateNonAttachedCamera();
                MainMenu.chaseCamera.IsActive = true;
            }
            // Update GUI params
            angCamModifierList.ListIndex = (int)((angCamModifier - 0.0001f + 1f) / 0.025f);
            angCamInterpolationList.ListIndex = (int)((angCamInterpolation) / 0.005f);
            chaseCamOffsetList.ListIndex = (chaseCamOffset);
            posInterpolationList.ListIndex = (int)((posInterpolation) / 0.01f);
            rollInterpolationList.ListIndex = (int)((cameraRollInterpolation) / 0.005f);
            pitchInterpolationList.ListIndex = (int)((cameraPitchInterpolation) / 0.005f);
            chaseCamMaxAngleList.ListIndex = (int)((maxAngle - 25f) / 5f);
            customCamFOVList.ListIndex = (int)(fov - 20.0f);
            customCamForwardOffsetList.ListIndex = (int)((forwardOffset + 8f) / 0.025f);
            customCamUpOffsetList.ListIndex = (int)((upOffset + 5f) / 0.025f);
            customCamSideOffsetList.ListIndex = (int)((sideOffset + 5f) / 0.025f);
            lockPosOffsetCheckbox.Checked = lockOffsetPos;
            linearPosCheckbox.Checked = linearPosOffset;
            pedLockCheckbox.Checked = pedLock;

            menu.RefreshIndex();
        }

        #endregion

        // Constructor
        public CustomCam()
        {
            Tick += RunDriftCam;
            Tick += RunChaseCam;
        }

        private void CreateMenu()
        {
            menu = new Menu("Enhanced camera", "Lead/chase camera parameters");

            #region checkbox items

            // Lock position offset
            lockPosOffsetCheckbox = new MenuCheckboxItem("Lock position offset", "Locks position offset, useful when sticking camera to the car - on top of hood, as FPV cam, etc.", false);
            // Linear position offset
            linearPosCheckbox = new MenuCheckboxItem("Linear position offset", "Instead of circular motion around the car, the camera moves along car's X axis. Dope for cinematic shots.", false);
            // Lock to ped
            pedLockCheckbox = new MenuCheckboxItem("Lock rotation to camera plane", "Changes the way that camera rotates around car (mostly visible on uneven ground).", false);

            #endregion

            #region main parameters

            // Angular velocity modifier
            List<string> angCamModifierValues = new List<string>();
            for (float i = -1f; i < 1f; i += 0.025f)
            {
                angCamModifierValues.Add(i.ToString("0.000"));
            }
            angCamModifierList = new MenuListItem("Modifier", angCamModifierValues, 48, "This modifier * angular velocity = target rotation. Higher values make camera move further from lock. (-1,1)")
            {
                ShowColorPanel = false
            };

            // Yaw interpolation modifier
            List<string> angCamInterpolationValues = new List<string>();
            for (float i = 0.0f; i < 1f; i += 0.005f)
            {
                angCamInterpolationValues.Add(i.ToString("0.000"));
            }
            angCamInterpolationList = new MenuListItem("Yaw interpolation", angCamInterpolationValues, 4, "Lower values - smoother movement. WARNING: Slider is inversed for chase camera - 0 is max interpolation, 1 is complete lock. (0,1)")
            {
                ShowColorPanel = false
            };

            // Roll interpolation modifier
            List<string> rollInterpolationValues = new List<string>();
            for (float i = 0.0f; i < 1f; i += 0.005f)
            {
                rollInterpolationValues.Add(i.ToString("0.000"));
            }
            rollInterpolationList = new MenuListItem("Roll interpolation", rollInterpolationValues, 20, "Lower values - smoother movement. (0,1)")
            {
                ShowColorPanel = false
            };

            // Roll interpolation modifier
            List<string> pitchInterpolationValues = new List<string>();
            for (float i = 0.0f; i < 1f; i += 0.005f)
            {
                pitchInterpolationValues.Add(i.ToString("0.000"));
            }
            pitchInterpolationList = new MenuListItem("Pitch interpolation", pitchInterpolationValues, 20, "Lower values - smoother movement. (0,1)")
            {
                ShowColorPanel = false
            };

            // Chase cam offset modifier
            List<string> chaseCamOffsetValues = new List<string>();
            for (float i = 0; i <= 5; i += 0.125f)
            {
                chaseCamOffsetValues.Add((i).ToString("0.000"));
            }
            chaseCamOffsetList = new MenuListItem("Camera offset", chaseCamOffsetValues, 0, "Offsets chase camera target towards its velocity vector. (0,5)")
            {
                ShowColorPanel = false
            };

            // Camera x position offset interpolation modifier
            List<string> posInterpolationValues = new List<string>();
            for (float i = 0.0f; i < 1f; i += 0.01f)
            {
                posInterpolationValues.Add(i.ToString("0.00"));
            }
            posInterpolationList = new MenuListItem("Position interpolation", posInterpolationValues, 100, "Lower values - smoother movement, higher delay. (0,1)")
            {
                ShowColorPanel = false
            };

            // FOV modifier
            List<string> customCamFOVValues = new List<string>();
            for (float i = 20; i <= 120; i += 1f)
            {
                customCamFOVValues.Add((i).ToString());
            }
            customCamFOVList = new MenuListItem("FOV", customCamFOVValues, 43, "Change custom camera's FOV. (20,120)")
            {
                ShowColorPanel = false
            };

            // Custom cam forward offset
            List<string> customCamForwardOffsetValues = new List<string>();
            for (float i = -8; i <= 8; i += 0.025f)
            {
                customCamForwardOffsetValues.Add((i).ToString("0.000"));
            }
            customCamForwardOffsetList = new MenuListItem("Y offset", customCamForwardOffsetValues, 130, "Custom camera offset in forward direction. (-8,8)")
            {
                ShowColorPanel = false
            };
            // Custom cam side offset
            List<string> customCamSideOffsetValues = new List<string>();
            for (float i = -5; i <= 8; i += 0.025f)
            {
                customCamSideOffsetValues.Add((i).ToString("0.000"));
            }
            customCamSideOffsetList = new MenuListItem("X offset", customCamSideOffsetValues, 200, "Custom camera offset in side direction. (-5,8)")
            {
                ShowColorPanel = false
            };
            // Custom cam up offset
            List<string> customCamUpOffsetValues = new List<string>();
            for (float i = -5; i <= 8; i += 0.025f)
            {
                customCamUpOffsetValues.Add((i).ToString("0.000"));
            }
            customCamUpOffsetList = new MenuListItem("Z offset", customCamUpOffsetValues, 282, "Custom camera offset in up direction. (-5,8)")
            {
                ShowColorPanel = false
            };

            List<string> chaseCamMaxAngleValues = new List<string>();
            for (float i = 25; i <= 360; i += 5)
            {
                chaseCamMaxAngleValues.Add(i.ToString());
            }
            chaseCamMaxAngleList = new MenuListItem("Max angle to lock.", chaseCamMaxAngleValues, 67, "Max angle from velocity vector to keep the lock on, if angle exceeds this limit, camera switches back to normal. (25,360)")
            {
                ShowColorPanel = false
            };

            #endregion

            #region adding menu items
            // Checkboxes
            menu.AddMenuItem(lockPosOffsetCheckbox);
            menu.AddMenuItem(linearPosCheckbox);
            menu.AddMenuItem(pedLockCheckbox);
            // Main modifier
            menu.AddMenuItem(angCamModifierList);
            // Interpolation sliders
            menu.AddMenuItem(angCamInterpolationList);
            menu.AddMenuItem(rollInterpolationList);
            menu.AddMenuItem(pitchInterpolationList);
            menu.AddMenuItem(posInterpolationList);
            // Chase camera
            menu.AddMenuItem(chaseCamOffsetList);
            menu.AddMenuItem(chaseCamMaxAngleList);
            // FOV and offset
            menu.AddMenuItem(customCamFOVList);
            menu.AddMenuItem(customCamForwardOffsetList);
            menu.AddMenuItem(customCamUpOffsetList);
            menu.AddMenuItem(customCamSideOffsetList);

            // Presets
            Menu presetsMenu = new Menu("Presets", "Spawn camera presets");
            MenuItem tandemCamPreset = new MenuItem("Tandem Camera 1.0", "Chef's specialty")
            {
                Label = $"→→→"
            };
            MenuItem fpvCamPreset = new MenuItem("FPV camera base", "Best to use with chicken model")
            {
                Label = $"→→→"
            };
            MenuItem NFSCamPreset = new MenuItem("NFS camera", "Arcade game experience")
            {
                Label = $"→→→"
            };
            presetsMenu.AddMenuItem(tandemCamPreset);
            presetsMenu.AddMenuItem(fpvCamPreset);
            presetsMenu.AddMenuItem(NFSCamPreset);

            presetsMenu.OnItemSelect += (sender, item, index) => {
                if (item == tandemCamPreset)
                {
                    MainMenu.Notify("~g~~h~Info~h~~s~: Switching to Tandem Camera 1.0. Tune XYZ offsets to your car.");

                    currentlySelectedCamera = new KeyValuePair<string, CameraInfo>("_1__", CustomCamPresets.tandemCam1);
                    SpawnSavedCamera();

                    // Update menu stuff according to loaded values
                    UpdateParams();
                    presetsMenu.GoBack();
                }
                if (item == fpvCamPreset)
                {
                    MainMenu.Notify("~g~~h~Info~h~~s~: Switching to FPV camera base. Tune XYZ offsets to your car.");

                    currentlySelectedCamera = new KeyValuePair<string, CameraInfo>("_2__", CustomCamPresets.fpvCam1);
                    SpawnSavedCamera();

                    // Update menu stuff according to loaded values
                    UpdateParams();
                    presetsMenu.GoBack();
                }
                if (item == NFSCamPreset)
                {
                    MainMenu.Notify("~g~~h~Info~h~~s~: Switching to NFS camera. Tune XYZ offsets to your car.");

                    currentlySelectedCamera = new KeyValuePair<string, CameraInfo>("_3__", CustomCamPresets.NFSCam);
                    SpawnSavedCamera();

                    // Update menu stuff according to loaded values
                    UpdateParams();
                    presetsMenu.GoBack();
                }
            };

            MenuItem buttonPresets = new MenuItem("Presets menu", "Get started here")
            {
                Label = "→→→"
            };
            menu.AddMenuItem(buttonPresets);
            MenuController.AddSubmenu(menu, presetsMenu);
            MenuController.BindMenuItem(menu, presetsMenu, buttonPresets);
            presetsMenu.RefreshIndex();

            #endregion

            #region managing save/load camera stuff

            // Saving/Loading cameras
            MenuItem savedCamerasButton = new MenuItem("Saved cameras", "User created cameras");
            savedCamerasMenu = new Menu("Saved cameras");
            MenuController.AddSubmenu(menu, savedCamerasMenu);
            menu.AddMenuItem(savedCamerasButton);
            savedCamerasButton.Label = "→→→";
            MenuController.BindMenuItem(menu, savedCamerasMenu, savedCamerasButton);

            MenuItem saveCamera = new MenuItem("Save Current Camera", "Save the current camera.");
            savedCamerasMenu.AddMenuItem(saveCamera);
            savedCamerasMenu.OnMenuOpen += (sender) => {
                savedCamerasMenu.ClearMenuItems();
                savedCamerasMenu.AddMenuItem(saveCamera);
                LoadCameras();
            };

            savedCamerasMenu.OnItemSelect += (sender, item, index) => {
                if (item == saveCamera)
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        SaveCamera();
                        savedCamerasMenu.GoBack();
                    }
                    else
                    {
                        MainMenu.Notify("~g~~h~Info~h~~s~: You are currently not in any vehicle. Please enter a vehicle before trying to save the camera.");
                    }
                }
                else
                {
                    UpdateSelectedCameraMenu(item, sender);
                }
            };

            MenuController.AddMenu(selectedCameraMenu);
            MenuItem spawnCamera = new MenuItem("Spawn Camera", "Spawn this saved camera.");
            MenuItem renameCamera = new MenuItem("Rename Camera", "Rename your saved camera.");
            MenuItem deleteCamera = new MenuItem("~r~Delete Camera", "~r~This will delete your saved camera. Warning: this can NOT be undone!");
            selectedCameraMenu.AddMenuItem(spawnCamera);
            selectedCameraMenu.AddMenuItem(renameCamera);
            selectedCameraMenu.AddMenuItem(deleteCamera);

            selectedCameraMenu.OnMenuClose += (sender) => {
                selectedCameraMenu.RefreshIndex();
            };

            selectedCameraMenu.OnItemSelect += async (sender, item, index) => {
                if (item == spawnCamera)
                {
                    MainMenu.ResetCameras();
                    SpawnSavedCamera();
                    UpdateParams();
                    selectedCameraMenu.GoBack();
                    savedCamerasMenu.RefreshIndex();

                }
                else if (item == deleteCamera)
                {
                    item.Label = "";
                    DeleteResourceKvp(currentlySelectedCamera.Key);
                    selectedCameraMenu.GoBack();
                    savedCamerasMenu.RefreshIndex();
                    MainMenu.Notify("~g~~h~Info~h~~s~: Your saved camera has been deleted.");
                }
                else if (item == renameCamera)
                {
                    string newName = await MainMenu.GetUserInput(windowTitle: "Enter a new name for this camera.", defaultText: null, maxInputLength: 30);
                    if (string.IsNullOrEmpty(newName))
                    {
                        MainMenu.Notify("~r~~h~Error~h~~s~: Invalid input");
                    }
                    else
                    {
                        if (SaveCameraInfo("xcm_" + newName, currentlySelectedCamera.Value, false))
                        {
                            DeleteResourceKvp(currentlySelectedCamera.Key);
                            while (!selectedCameraMenu.Visible)
                            {
                                await BaseScript.Delay(0);
                            }
                            MainMenu.Notify("~g~~h~Info~h~~s~: Your camera has successfully been renamed.");
                            selectedCameraMenu.GoBack();
                            currentlySelectedCamera = new KeyValuePair<string, CameraInfo>();
                        }
                        else
                        {
                            MainMenu.Notify("~r~~h~Error~h~~s~: This name is already in use or something unknown failed. Contact the server owner if you believe something is wrong.");
                        }
                    }
                }
            };

            #endregion

            #region handling menu changes

            // Handle checkbox
            menu.OnCheckboxChange += (_menu, _item, _index, _checked) => {
                if (_item == linearPosCheckbox)
                {
                    linearPosOffset = _checked;
                }
                if (_item == lockPosOffsetCheckbox)
                {
                    lockOffsetPos = _checked;
                }
                if (_item == pedLockCheckbox)
                {
                    pedLock = _checked;
                }
            };

            // Handle list change
            menu.OnListIndexChange += (_menu, _listItem, _oldIndex, _newIndex, _itemIndex) => {
                if (_listItem == angCamModifierList)
                {
                    angCamModifier = _newIndex * 0.025f + 0.0001f - 1f;
                }
                if (_listItem == angCamInterpolationList)
                {
                    angCamInterpolation = ((_newIndex) * 0.005f);
                }
                if (_listItem == chaseCamOffsetList)
                {
                    chaseCamOffset = (_newIndex);
                }
                if (_listItem == posInterpolationList)
                {
                    posInterpolation = ((_newIndex) * 0.01f);
                }
                if (_listItem == rollInterpolationList)
                {
                    cameraRollInterpolation = ((_newIndex) * 0.005f);
                }
                if (_listItem == pitchInterpolationList)
                {
                    cameraPitchInterpolation = ((_newIndex) * 0.005f);
                }
                if (_listItem == chaseCamMaxAngleList)
                {
                    maxAngle = (float)(_newIndex * 5f + 25f);
                }
                if (_listItem == customCamFOVList)
                {
                    fov = (float)(_newIndex * 1f + 20.0f);
                    if (LeadCam)
                    {
                        SetCamFov(MainMenu.driftCamera.Handle, fov);
                    }
                    else if (ChaseCam)
                    {
                        SetCamFov(MainMenu.chaseCamera.Handle, fov);
                    }
                }
                if (_listItem == customCamForwardOffsetList)
                {
                    forwardOffset = (float)(_newIndex * 0.025f - 8f);
                }
                if (_listItem == customCamSideOffsetList)
                {
                    sideOffset = (float)(_newIndex * 0.025f - 5f);
                }
                if (_listItem == customCamUpOffsetList)
                {
                    upOffset = (float)(_newIndex * 0.025f - 5f);
                }
            };

            #endregion

        }

        /// <summary>
        /// Creates the menu if it doesn't exist, and then returns it.
        /// </summary>
        /// <returns>The Menu</returns>
        public Menu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }

        #region custom camera static variables

        public static float fov = 63.0f;
        private static float forwardOffset = -4.75f;
        private static float sideOffset = 0.0f;
        private static float upOffset = 2.05f;
        private static bool linearPosOffset = false;
        private static bool lockOffsetPos = false;
        private static float angCamModifier = 0.2f;
        private static float angCamInterpolation = 0.02f;
        private static float angularVelOld = 0f;
        private static float posInterpolation = 0.5f;
        private static float oldPosXOffset = 0f;
        public static float maxAngle = 360f;

        private static float cameraRollInterpolation = 0.1f;
        private static float cameraPitchInterpolation = 0.1f;

        private static bool pedLock = false;

        #endregion

        #region drift camera

        // Consts
        private const float MAX_ANG_VEL_OFFSET = 1.0f;
        private const float ROTATION_NORMALIZE = 100.0f;
        private const float TIMESTEP_DELIMITER = 0.015f;

        /// <summary>
        /// Changes main render camera behaviour, follows car with specified degree of freedom
        /// based on modifier value and interpolation value (and other variables such as
        /// angle and position offset values).
        /// </summary>
        /// <returns></returns>
        private async Task RunDriftCam()
        {
            if (LeadCam)
            {
                int vehicleEntity = GetVehiclePedIsIn(PlayerPedId(), false);
                if (vehicleEntity > 0)
                {
                    if (MainMenu.driftCamera != null)
                    {
                        // Calculate timestep to account for framerate drops
                        //float deltaTime = Timestep() / CustomCam.TIMESTEP_DELIMITER;
                        // Get vehicle's angular velocity
                        float angularVel = GetEntityRotationVelocity(vehicleEntity).Z;
                        // Keep it in reasonable range
                        angularVel = (angularVel > MAX_ANG_VEL_OFFSET) ? (MAX_ANG_VEL_OFFSET) : (angularVel);
                        // Lerp to smooth the camera transition
                        angularVel = MainMenu.CamMath.Lerp(angularVelOld, angularVel, angCamInterpolation);
                        // Save the value to lerp with it in the next frame
                        angularVelOld = angularVel;
                        // Calculating target camera rotation
                        float finalRotation = -angularVel * angCamModifier * ROTATION_NORMALIZE;

                        // Get vehicle entity for further operations
                        Vehicle veh = new Vehicle(vehicleEntity);

                        // Setting the position offset also based on angular velocity
                        if (!lockOffsetPos)
                        {
                            oldPosXOffset = MainMenu.CamMath.Lerp(oldPosXOffset, finalRotation, (posInterpolation >= 1f) ? (0.99f) : (posInterpolation));
                        }
                        else
                        {
                            oldPosXOffset = finalRotation;
                        }

                        // Get the static offset based on user's input
                        Vector3 staticPosition = Vector3.Zero;
                        if (pedLock)
                        {
                            staticPosition = veh.ForwardVector * forwardOffset +
                                                veh.RightVector * sideOffset +
                                                Vector3.ForwardLH * upOffset;
                        }
                        else
                        {
                            staticPosition = veh.ForwardVector * forwardOffset +
                                                veh.RightVector * sideOffset +
                                                veh.UpVector * upOffset;
                        }

                        // Calculate final offset taking into consideration dynamic offset (oldPosXOffset), static
                        // offset and the offset resulting from rotating the camera around the car
                        if (!linearPosOffset)
                        {
                            if (oldPosXOffset != finalRotation)
                            {
                                float rotation = oldPosXOffset + MainMenu.userYaw;
                                if (pedLock)
                                {
                                    MainMenu.driftCamera.Position = veh.Position + MainMenu.CamMath.RotateAroundAxis(staticPosition, Vector3.ForwardLH, rotation * MainMenu.CamMath.DegToRad);
                                }
                                else
                                {
                                    MainMenu.driftCamera.Position = veh.Position + MainMenu.CamMath.RotateAroundAxis(staticPosition, veh.UpVector, rotation * MainMenu.CamMath.DegToRad);
                                }
                                if (MainMenu.userLookBehind)
                                {
                                    MainMenu.driftCamera.Position = veh.Position +
                                        MainMenu.CamMath.RotateAroundAxis(staticPosition, veh.UpVector, 179f * MainMenu.CamMath.DegToRad);
                                }
                            }
                            else
                            {
                                if (MainMenu.userLookBehind)
                                {
                                    MainMenu.driftCamera.Position = veh.Position +
                                                                                    staticPosition -
                                                                                    (veh.RightVector * sideOffset) +
                                                                                    veh.ForwardVector * 3.5f +
                                                                                    veh.UpVector * 0.5f;
                                }
                                else
                                {
                                    MainMenu.driftCamera.Position = veh.Position + staticPosition;
                                }
                            }
                        }
                        else
                        {
                            MainMenu.driftCamera.Position = veh.Position + staticPosition + veh.RightVector * oldPosXOffset / 12f;
                            if (MainMenu.userLookBehind)
                            {
                                MainMenu.driftCamera.Position = veh.Position + MainMenu.CamMath.RotateAroundAxis(staticPosition, veh.UpVector, 179f * MainMenu.CamMath.DegToRad);
                            }
                        }

                        // Calculate target rotation as a heading in given range
                        Vector3 newRot = GameMath.DirectionToRotation(GameMath.HeadingToDirection((oldPosXOffset + GetEntityRotation(vehicleEntity, 2).Z + 180.0f) % 360.0f - 180.0f), GetEntityRoll(vehicleEntity));
                        float roll = 0f;
                        float pitch = 0f;
                        // Clamp values
                        if (CameraConstraints.CrashCheck(vehicleEntity))
                        {
                            staticPosition = Vector3.ForwardLH * upOffset;
                            MainMenu.driftCamera.Position = veh.Position + staticPosition;
                            roll = MainMenu.CamMath.Lerp(MainMenu.driftCamera.Rotation.Y, 0f, 0.1f);
                            pitch = MainMenu.CamMath.Lerp(MainMenu.driftCamera.Rotation.X, 0f, 0.1f);
                            pitch = CameraConstraints.ClampPitch(pitch);
                        }
                        else
                        {
                            // Calculate smooth roll and pitch rotation
                            roll = MainMenu.CamMath.Lerp(MainMenu.driftCamera.Rotation.Y, -GetEntityRoll(vehicleEntity), cameraRollInterpolation);
                            pitch = MainMenu.CamMath.Lerp(MainMenu.driftCamera.Rotation.X - MainMenu.userTilt, GetEntityRotation(vehicleEntity, 2).X, cameraPitchInterpolation);
                            roll = CameraConstraints.ClampRoll(roll);
                            pitch = CameraConstraints.ClampPitch(pitch);
                        }
                        // Finalize the rotation
                        float yaw = (MainMenu.userLookBehind) ? (GetEntityRotation(vehicleEntity, 2).Z + 179.9f) : (newRot.Z + MainMenu.userYaw);
                        SetCamRot(MainMenu.driftCamera.Handle, pitch + MainMenu.userTilt, roll, yaw, 2);

                        // Update minimap
                        LockMinimapAngle((int)(MainMenu.CamMath.Fmod(yaw, 360f)));
                    }
                    else
                    {
                        // In case the camera is null - reset the cameras and reassign this camera
                        MainMenu.ResetCameras();
                        MainMenu.driftCamera = MainMenu.CreateNonAttachedCamera();
                        MainMenu.driftCamera.IsActive = true;
                    }
                }
                else
                {
                    // Disable custom camera
                    if (LeadCam || ChaseCam)
                    {
                        MainMenu.SwitchToGameplayCam();
                        MainMenu.Notify("~g~~h~Info~h~~s~: Vehicle not found, switching to gameplay camera...");
                    }
                }
            }
            else
            {
                await Delay(0);
            }
        }

        #endregion

        #region chase camera

        /// <summary>
        /// Gets closest vehicle to Ped
        /// </summary>
        /// <returns>closest vehicle</returns>
        public static Vehicle GetClosestVehicle(int maxDistance, float requiredAngle)
        {
            float smallestDistance = (float)maxDistance;
            Vehicle[] vehs = World.GetAllVehicles();
            Vehicle closestVeh = null;

            int playerVeh = GetVehiclePedIsIn(PlayerPedId(), false);

            if (vehs != null)
            {
                foreach (Vehicle veh in vehs)
                {
                    if (veh.Handle != playerVeh)
                    {
                        float distance = Vector3.Distance(GetEntityCoords(veh.Handle, true), GetEntityCoords(playerVeh, true));
                        if ((distance <= smallestDistance) && (veh != null))
                        {
                            smallestDistance = distance;
                            Vector3 targetVec = GetOffsetFromEntityGivenWorldCoords(playerVeh, veh.Position.X, veh.Position.Y, veh.Position.Z);
                            float angle = -MainMenu.CamMath.AngleBetween(targetVec, new Vector3(0, 0.0001f, 0) + GetEntitySpeedVector(playerVeh, true));
                            // Make sure that target is in range given by angle
                            if (Math.Abs(angle) < requiredAngle)
                            {
                                closestVeh = veh;
                            }
                        }
                    }
                }
            }
            return closestVeh;
        }

        public static Vehicle target = null;
        private static int chaseCamOffset = 0;

        /// <summary>
        /// Changes main render camera behaviour, camera locks onto closest vehicle
        /// that is in front of the player (in certain degree range in front of car's
        /// velocity's magnitude).
        /// </summary>
        /// <returns></returns>
        private async Task RunChaseCam()
        {
            if (ChaseCam)
            {
                // Get player's vehicle
                int vehicleEntity = GetVehiclePedIsIn(PlayerPedId(), false);
                if (vehicleEntity > 0)
                {
                    if (MainMenu.chaseCamera != null)
                    {

                        // If target car is located
                        if (target != null)
                        {
                            // Get vector from player's car to target car offset by value
                            Vector3 targetVec = GetOffsetFromEntityGivenWorldCoords(
                                                    vehicleEntity,
                                                    target.Position.X + target.ForwardVector.X * (chaseCamOffset / 5),
                                                    target.Position.Y + target.ForwardVector.Y * (chaseCamOffset / 5),
                                                    target.Position.Z);

                            // Get rotation to target vehicle
                            float finalRotation = -MainMenu.CamMath.AngleBetween(targetVec, new Vector3(0, 10, 0));

                            if (Math.Abs(finalRotation) > maxAngle)
                            {
                                target = null;
                                MainMenu.SwitchCameraToDrift();
                                MainMenu.Notify("~g~~h~Info~h~~s~: Target exceeded angle limit, switching to Lead Camera");
                                return;
                            }

                            if (finalRotation.ToString() != "NaN")
                            {
                                // Lerp target rotation
                                // (1 - angCamInterpolation) instead of just interpolation so that camera
                                // can be changed smoothly from lead cam to chase cam
                                finalRotation = MainMenu.CamMath.Lerp(GetEntityHeading(MainMenu.chaseCamera.Handle), finalRotation, 1 - angCamInterpolation);

                                // Calculate camera's position
                                Vehicle veh = new Vehicle(vehicleEntity);

                                // Static position as an offset from the car
                                Vector3 staticPosition = Vector3.Zero;
                                if (pedLock)
                                {
                                    staticPosition = veh.ForwardVector * forwardOffset +
                                                        veh.RightVector * sideOffset +
                                                        Vector3.ForwardLH * upOffset;
                                }
                                else
                                {
                                    staticPosition = veh.ForwardVector * forwardOffset +
                                                        veh.RightVector * sideOffset +
                                                        veh.UpVector * upOffset;
                                }

                                // Calculate chase camera position
                                if (!lockOffsetPos)
                                {
                                    float rotation = finalRotation + MainMenu.userYaw;
                                    if (pedLock)
                                    {
                                        MainMenu.chaseCamera.Position = veh.Position + MainMenu.CamMath.RotateAroundAxis(staticPosition, Vector3.ForwardLH, rotation * MainMenu.CamMath.DegToRad);
                                    }
                                    else
                                    {
                                        MainMenu.chaseCamera.Position = veh.Position + MainMenu.CamMath.RotateAroundAxis(staticPosition, veh.UpVector, rotation * MainMenu.CamMath.DegToRad);
                                    }
                                    if (MainMenu.userLookBehind) { MainMenu.chaseCamera.Position = veh.Position - (veh.RightVector * sideOffset) + MainMenu.CamMath.RotateAroundAxis(staticPosition, veh.UpVector, 179f * MainMenu.CamMath.DegToRad); }
                                }
                                else
                                {
                                    MainMenu.chaseCamera.Position = veh.Position + staticPosition;
                                    if (MainMenu.userLookBehind) { MainMenu.chaseCamera.Position = veh.Position + staticPosition + veh.ForwardVector * 3f + veh.UpVector * 0.5f; }
                                }

                                // Calculate the camera rotation
                                Vector3 newRot = GameMath.DirectionToRotation(GameMath.HeadingToDirection((finalRotation + GetEntityRotation(vehicleEntity, 4).Z + 180.0f) % 360.0f - 180.0f), GetEntityRoll(vehicleEntity));

                                // Calculate smooth roll and pitch rotation
                                float roll = 0f;
                                float pitch = 0f;
                                // Clamp values
                                if (CameraConstraints.CrashCheck(vehicleEntity))
                                {
                                    staticPosition = Vector3.ForwardLH * upOffset;
                                    MainMenu.chaseCamera.Position = veh.Position + staticPosition;
                                    roll = MainMenu.CamMath.Lerp(MainMenu.chaseCamera.Rotation.Y, 0f, 0.1f);
                                    pitch = MainMenu.CamMath.Lerp(MainMenu.chaseCamera.Rotation.X, 0f, 0.1f);
                                    pitch = CameraConstraints.ClampPitch(pitch);
                                }
                                else
                                {
                                    // Calculate smooth roll and pitch rotation
                                    roll = MainMenu.CamMath.Lerp(MainMenu.chaseCamera.Rotation.Y, -GetEntityRoll(vehicleEntity), cameraRollInterpolation);
                                    pitch = MainMenu.CamMath.Lerp(MainMenu.chaseCamera.Rotation.X - MainMenu.userTilt, GetEntityPitch(vehicleEntity), cameraPitchInterpolation);
                                    roll = CameraConstraints.ClampRoll(roll);
                                    pitch = CameraConstraints.ClampPitch(pitch);
                                }
                                // Finally, set the rotation
                                float yaw = (MainMenu.userLookBehind) ? (GetEntityRotation(vehicleEntity, 2).Z + 179.9f) : (newRot.Z + MainMenu.userYaw);
                                MainMenu.chaseCamera.Rotation = new Vector3(pitch + MainMenu.userTilt, roll, yaw);

                                // Update minimap
                                LockMinimapAngle((int)(MainMenu.CamMath.Fmod(yaw, 360f)));
                            }
                        }
                        else
                        {
                            // Target car not found - switch to Lead Cam
                            MainMenu.SwitchCameraToDrift();
                            MainMenu.Notify("~g~~h~Info~h~~s~: Target not found, switching to Lead Camera");
                        }

                        // Find target and generate camera
                    }
                    else
                    {
                        MainMenu.ResetCameras();
                        MainMenu.chaseCamera = MainMenu.CreateNonAttachedCamera();
                        MainMenu.chaseCamera.IsActive = true;
                        target = GetClosestVehicle(2000, maxAngle);
                    }
                }
                else
                {
                    // Disable custom camera
                    if (LeadCam || ChaseCam)
                    {
                        MainMenu.SwitchToGameplayCam();
                        MainMenu.Notify("~g~~h~Info~h~~s~: Vehicle not found, switching to gameplay camera...");
                    }
                }
            }
            else
            {
                await Delay(0);
            }
        }

        #endregion

        /// ---
        /// Save/load functions originally made by Vespura (https://www.tomgrobbe.com/) for vMenu.
        /// Snippets of the code were slightly modified to suit camera needs and added here.
        /// ---
        #region save/load

        public struct CameraInfo
        {
            public float angCamInterpolation_;
            public float angCamModifier_;
            public float posInterpolation_;
            public float chaseCamMaxAngle_;
            public bool linearPosOffset_;
            public bool lockOffsetPos_;
            public float customCamFOV_;
            public float customCamForwardOffset_;
            public float customCamUpOffset_;
            public float customCamSideOffset_;
            public float cameraRollInterpolation_;
            public float cameraPitchInterpolation_;
            public bool pedLock_;
        }

        private bool UpdateSelectedCameraMenu(MenuItem selectedItem, Menu parentMenu = null)
        {
            if (!scMenuItems.ContainsKey(selectedItem))
            {
                MainMenu.Notify("~r~~h~Error~h~~s~: In some very strange way, you've managed to select a button, that does not exist according to this list. So your vehicle could not be loaded. :( Maybe your save files are broken?");
                return false;
            }
            var camInfo = scMenuItems[selectedItem];
            currentlySelectedCamera = camInfo;
            selectedCameraMenu.MenuSubtitle = $"{camInfo.Key.Substring(4)}";
            MenuController.CloseAllMenus();
            selectedCameraMenu.OpenMenu();
            if (parentMenu != null)
            {
                MenuController.AddSubmenu(parentMenu, selectedCameraMenu);
            }
            return true;
        }

        private bool SpawnSavedCamera()
        {
            if (currentlySelectedCamera.Key != null)
            {
                angCamInterpolation = currentlySelectedCamera.Value.angCamInterpolation_;
                angCamModifier = currentlySelectedCamera.Value.angCamModifier_;
                posInterpolation = currentlySelectedCamera.Value.posInterpolation_;
                maxAngle = currentlySelectedCamera.Value.chaseCamMaxAngle_;
                linearPosOffset = currentlySelectedCamera.Value.linearPosOffset_;
                lockOffsetPos = currentlySelectedCamera.Value.lockOffsetPos_;
                fov = currentlySelectedCamera.Value.customCamFOV_;
                forwardOffset = currentlySelectedCamera.Value.customCamForwardOffset_;
                upOffset = currentlySelectedCamera.Value.customCamUpOffset_;
                sideOffset = currentlySelectedCamera.Value.customCamSideOffset_;
                cameraRollInterpolation = currentlySelectedCamera.Value.cameraRollInterpolation_;
                cameraPitchInterpolation = currentlySelectedCamera.Value.cameraPitchInterpolation_;
                pedLock = currentlySelectedCamera.Value.pedLock_;
            }
            else
            {
                MainMenu.Notify("~r~~h~Error~h~~s~: It seems that this slot got corrupted in some way, you need to delete it.");
                return false;
            }
            return true;
        }

        private bool SaveCameraInfo(string saveName, CameraInfo cameraInfo, bool overrideOldVersion)
        {
            if (string.IsNullOrEmpty(GetResourceKvpString(saveName)) || overrideOldVersion)
            {
                if (!string.IsNullOrEmpty(saveName) && saveName.Length > 4)
                {
                    // convert
                    string json = JsonConvert.SerializeObject(cameraInfo);

                    // log
                    Debug.WriteLine($"Saving!\nName: {saveName}\nCamera Data: {json}\n");

                    // save
                    SetResourceKvp(saveName, json);

                    // confirm
                    return GetResourceKvpString(saveName) == json;
                }
            }
            // if something isn't right, then the save is aborted and return false ("failed" state).
            return false;
        }

        public async void SaveCamera(string updateExistingSavedCameraName = null)
        {
            // Only continue if the player is in a vehicle.
            if (Game.PlayerPed.IsInVehicle())
            {
                CameraInfo ci = new CameraInfo()
                {
                    angCamInterpolation_ = angCamInterpolation,
                    angCamModifier_ = angCamModifier,
                    posInterpolation_ = posInterpolation,
                    chaseCamMaxAngle_ = maxAngle,
                    linearPosOffset_ = linearPosOffset,
                    lockOffsetPos_ = lockOffsetPos,
                    customCamFOV_ = fov,
                    customCamForwardOffset_ = forwardOffset,
                    customCamUpOffset_ = upOffset,
                    customCamSideOffset_ = sideOffset,
                    cameraRollInterpolation_ = cameraRollInterpolation,
                    cameraPitchInterpolation_ = cameraPitchInterpolation,
                    pedLock_ = pedLock
                };

                if (updateExistingSavedCameraName == null)
                {
                    var saveName = await MainMenu.GetUserInput(windowTitle: "Enter a save name", defaultText: null, maxInputLength: 30);
                    // If the name is not invalid.
                    if (!string.IsNullOrEmpty(saveName))
                    {
                        // Save everything from the dictionary into the client's kvp storage.
                        // If the save was successfull:
                        if (SaveCameraInfo("xcm_" + saveName, ci, false))
                        {
                            MainMenu.Notify($"~g~~h~Info~h~~s~: Camera {saveName} saved.");
                            LoadCameras();
                        }
                        // If the save was not successfull:
                        else
                        {
                            MainMenu.Notify("~r~~h~Error~h~~s~: Save already exists: (" + saveName + ")");
                        }
                    }
                    // The user did not enter a valid name to use as a save name for this vehicle.
                    else
                    {
                        MainMenu.Notify("~r~~h~Error~h~~s~: Invalid save name");
                    }
                }
                // We need to update an existing slot.
                else
                {
                    SaveCameraInfo("xcm_" + updateExistingSavedCameraName, ci, true);
                }
            }
            // The player is not inside a vehicle.
            else
            {
                MainMenu.Notify("~g~~h~Error~h~~s~: You need to be inside the vehicle");
            }
        }

        private Dictionary<string, CameraInfo> GetSavedCameras()
        {
            // Create a list to store all saved camera names in.
            var savedCameraNames = new List<string>();
            // Start looking for kvps starting with xcm_
            var findHandle = StartFindKvp("xcm_");
            // Keep looking...
            while (true)
            {
                // Get the kvp string key.
                var camString = FindKvp(findHandle);

                // If it exists then the key to the list.
                if (camString != "" && camString != null && camString != "NULL")
                {
                    savedCameraNames.Add(camString);
                }
                // Otherwise stop.
                else
                {
                    EndFindKvp(findHandle);
                    break;
                }
            }
            var camerasList = new Dictionary<string, CameraInfo>();
            // Loop through all save names (keys) from the list above, convert the string into a dictionary 
            // and add it to the dictionary above, with the camera save name as the key.
            foreach (var saveName in savedCameraNames)
            {
                camerasList.Add(saveName, JsonConvert.DeserializeObject<CameraInfo>(GetResourceKvpString(saveName)));
            }
            // Return the camera dictionary containing all camera save names (keys) linked to the correct camera
            return camerasList;
        }

        private async void LoadCameras()
        {
            var savedCameras = GetSavedCameras();
            scMenuItems = new Dictionary<MenuItem, KeyValuePair<string, CameraInfo>>();

            foreach (var sc in savedCameras)
            {
                MenuItem savedCameraBtn;
                if (sc.Key.Length > 4)
                {
                    savedCameraBtn = new MenuItem(sc.Key.Substring(4), $"Manage this saved camera.")
                    {
                        Label = $"→→→"
                    };
                }
                else
                {
                    savedCameraBtn = new MenuItem("NULL", $"Manage this saved camera.")
                    {
                        Label = $"→→→"
                    };
                }
                savedCamerasMenu.AddMenuItem(savedCameraBtn);
                scMenuItems.Add(savedCameraBtn, sc);
            }
            await Delay(0);
        }

        #endregion

        #region presets

        private static class CustomCamPresets
        {
            public static CameraInfo tandemCam1 = new CameraInfo
            {
                angCamInterpolation_ = 0.02f,
                angCamModifier_ = 0.275f,
                posInterpolation_ = 1f,
                chaseCamMaxAngle_ = 75f,
                linearPosOffset_ = false,
                lockOffsetPos_ = false,
                customCamFOV_ = 60.0f,
                customCamForwardOffset_ = -5.6f,
                customCamUpOffset_ = 2.65f,
                customCamSideOffset_ = 0.0f,
                cameraRollInterpolation_ = 0.045f,
                cameraPitchInterpolation_ = 0.045f,
                pedLock_ = true
            };
            public static CameraInfo fpvCam1 = new CameraInfo
            {
                angCamInterpolation_ = 0.01f,
                angCamModifier_ = 0.350f,
                posInterpolation_ = 1f,
                chaseCamMaxAngle_ = 75f,
                linearPosOffset_ = false,
                lockOffsetPos_ = true,
                customCamFOV_ = 70.0f,
                customCamForwardOffset_ = -0.2f,
                customCamUpOffset_ = 0.6f,
                customCamSideOffset_ = 0.35f,
                cameraRollInterpolation_ = 0.195f,
                cameraPitchInterpolation_ = 0.5f,
                pedLock_ = false
            };
            public static CameraInfo NFSCam = new CameraInfo
            {
                angCamInterpolation_ = 0.02f,
                angCamModifier_ = 0.250f,
                posInterpolation_ = 1f,
                chaseCamMaxAngle_ = 75f,
                linearPosOffset_ = true,
                lockOffsetPos_ = false,
                customCamFOV_ = 70.0f,
                customCamForwardOffset_ = -4.05f,
                customCamUpOffset_ = 1.35f,
                customCamSideOffset_ = 0.0f,
                cameraRollInterpolation_ = 0.05f,
                cameraPitchInterpolation_ = 1.0f,
                pedLock_ = true
            };
        };

        #endregion
    }
}
