using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using MenuAPI;
using Newtonsoft.Json;

namespace CustomCamera
{
    public class DroneCam : BaseScript
    {
        #region variables

        // Public variables
        public static bool DroneCamVar = false;

        // Private variables
        private Menu menu;
        private Menu savedDronesMenu;
        private Menu selectedDroneMenu = new Menu(_t("DRONE_MANAGE_TITLE"), _t("DRONE_MANAGE_DESC"));
        private MenuListItem modeList;
        private Dictionary<MenuItem, KeyValuePair<string, DroneSaveInfo>> sdMenuItems = new Dictionary<MenuItem, KeyValuePair<string, DroneSaveInfo>>();
        private static KeyValuePair<string, DroneSaveInfo> currentlySelectedDrone = new KeyValuePair<string, DroneSaveInfo>();
        private static int droneMode = 0;
        private static bool invertedPitch = false;
        private static bool invertedRoll = false;
        private static Vehicle homingTarget = null;

        #endregion

        #region GUI updating

        // GUI parameters
        MenuListItem gravityMultList;
        MenuListItem timestepMultList;
        MenuListItem dragMultList;
        MenuListItem accelerationMultList;
        MenuListItem rotationMultXList;
        MenuListItem rotationMultYList;
        MenuListItem rotationMultZList;
        MenuListItem tiltAngleList;
        MenuListItem fovList;
        MenuListItem maxVelList;

        MenuCheckboxItem invertPitch;
        MenuCheckboxItem invertRoll;

        // Update params
        private void UpdateParams()
        {
            // Reset camera to update params
            MainMenu.ResetCameras();
            if (DroneCamVar)
            {
                CreateDroneCamera();
            }
            // Update GUI params
            gravityMultList.ListIndex = (int)((gravityMult - 0.5f) / 0.05f);
            timestepMultList.ListIndex = (int)((timestepMult - 0.5f) / 0.05f);
            dragMultList.ListIndex = (int)((dragMult) / 0.05f);
            accelerationMultList.ListIndex = (int)((accelerationMult - 0.5f) / 0.05f);
            rotationMultXList.ListIndex = (int)((rotationMult.X - 0.5f) / 0.05f);
            rotationMultYList.ListIndex = (int)((rotationMult.Y - 0.5f) / 0.05f);
            rotationMultZList.ListIndex = (int)((rotationMult.Z - 0.5f) / 0.05f);
            maxVelList.ListIndex = (int)(maxVel - 10f);
            tiltAngleList.ListIndex = (int)((tiltAngle) / 5.0f);
            fovList.ListIndex = (int)((droneFov - 30.0f) / 5.0f);

            menu.RefreshIndex();
        }

        #endregion

        // Constructor
        public DroneCam()
        {
            Tick += RunDroneCam;
            Tick += AntiAfk;
        }

        private void CreateMenu()
        {
            menu = new Menu(_t("DRONE_TITLE"), _t("DRONE_DESC"));

            #region main parameters

            // Drone modes
            List<string> modeListData = new List<string>() { _t("DRONE_RACE"), _t("DRONE_ZERO_G"), _t("DRONE_SPECTATOR"), _t("DRONE_HOMING") };
            modeList = new MenuListItem(_t("DRONE_MODE"), modeListData, 0, _t("DRONE_MODE_DESC"));

            // Invert input
            invertPitch = new MenuCheckboxItem(_t("DRONE_INVERT_PITCH"), _t("DRONE_INVERT_PITCH_DESC"), false);
            invertRoll = new MenuCheckboxItem(_t("DRONE_INVERT_ROLL"), _t("DRONE_INVERT_ROLL_DESC"), false);

            // Gravity multiplier
            List<string> gravityMultValues = new List<string>();
            for (float i = 0.5f; i <= 4.0f; i += 0.05f)
            {
                gravityMultValues.Add(i.ToString("0.00"));
            }
            gravityMultList = new MenuListItem(_t("DRONE_GRAVITY"), gravityMultValues, 10, _t("DRONE_GRAVITY_DESC"))
            {
                ShowColorPanel = false
            };

            // Timestep multiplier
            List<string> timestepValues = new List<string>();
            for (float i = 0.5f; i <= 4.0f; i += 0.05f)
            {
                timestepValues.Add(i.ToString("0.00"));
            }
            timestepMultList = new MenuListItem(_t("DRONE_TIMESTEP"), timestepValues, 10, _t("DRONE_TIMESTEP_DESC"))
            {
                ShowColorPanel = false
            };

            // Drag multiplier
            List<string> dragMultValues = new List<string>();
            for (float i = 0.0f; i <= 4.0f; i += 0.05f)
            {
                dragMultValues.Add(i.ToString("0.00"));
            }
            dragMultList = new MenuListItem(_t("DRONE_DRAG"), dragMultValues, 20, _t("DRONE_DRAG_DESC"))
            {
                ShowColorPanel = false
            };

            // Acceleration multiplier
            List<string> accelerationMultValues = new List<string>();
            for (float i = 0.5f; i <= 4.0f; i += 0.05f)
            {
                accelerationMultValues.Add(i.ToString("0.00"));
            }
            accelerationMultList = new MenuListItem(_t("DRONE_ACCELE"), accelerationMultValues, 10, _t("DRONE_ACCELE_DESC"))
            {
                ShowColorPanel = false
            };

            // Rotation multipliers
            List<string> rotationMultXValues = new List<string>();
            for (float i = 0.5f; i <= 4.0f; i += 0.05f)
            {
                rotationMultXValues.Add(i.ToString("0.00"));
            }
            rotationMultXList = new MenuListItem(_t("DRONE_PITCH"), rotationMultXValues, 10, _t("DRONE_PITCH_DESC"))
            {
                ShowColorPanel = false
            };
            List<string> rotationMultYValues = new List<string>();
            for (float i = 0.5f; i <= 4.0f; i += 0.05f)
            {
                rotationMultYValues.Add(i.ToString("0.00"));
            }
            rotationMultYList = new MenuListItem(_t("DRONE_ROLL"), rotationMultYValues, 10, _t("DRONE_ROLL_DESC"))
            {
                ShowColorPanel = false
            };
            List<string> rotationMultZValues = new List<string>();
            for (float i = 0.5f; i <= 4.0f; i += 0.05f)
            {
                rotationMultZValues.Add(i.ToString("0.00"));
            }
            rotationMultZList = new MenuListItem(_t("DRONE_YAW"), rotationMultZValues, 10, _t("DRONE_YAW_DESC"))
            {
                ShowColorPanel = false
            };
            // Tilt angle
            List<string> tiltAngleValues = new List<string>();
            for (float i = 0.0f; i <= 80.0f; i += 5f)
            {
                tiltAngleValues.Add(i.ToString("0.0"));
            }
            tiltAngleList = new MenuListItem(_t("DRONE_TILT"), tiltAngleValues, 9, _t("DRONE_TILT_DESC"))
            {
                ShowColorPanel = false
            };
            // FOV
            List<string> fovValues = new List<string>();
            for (float i = 30.0f; i <= 120.0f; i += 5f)
            {
                fovValues.Add(i.ToString("0.0"));
            }
            fovList = new MenuListItem(_t("DRONE_FOV"), fovValues, 10, _t("DRONE_FOV_DESC"))
            {
                ShowColorPanel = false
            };
            // Max velocity
            List<string> maxVelValues = new List<string>();
            for (float i = 10.0f; i <= 50.0f; i += 1f)
            {
                maxVelValues.Add(i.ToString("0.0"));
            }
            maxVelList = new MenuListItem(_t("DRONE_MAX_VELOCITY"), maxVelValues, 20, _t("DRONE_MAX_VELOCITY_DESC"))
            {
                ShowColorPanel = false
            };

            #endregion

            #region adding menu items

            menu.AddMenuItem(modeList);
            menu.AddMenuItem(invertPitch);
            menu.AddMenuItem(invertRoll);
            menu.AddMenuItem(gravityMultList);
            menu.AddMenuItem(timestepMultList);
            menu.AddMenuItem(dragMultList);
            menu.AddMenuItem(accelerationMultList);
            menu.AddMenuItem(maxVelList);
            menu.AddMenuItem(rotationMultXList);
            menu.AddMenuItem(rotationMultYList);
            menu.AddMenuItem(rotationMultZList);
            menu.AddMenuItem(tiltAngleList);
            menu.AddMenuItem(fovList);

            #endregion

            #region managing save/load camera stuff

            // Saving/Loading cameras
            MenuItem savedDronesButton = new MenuItem(_t("DRONE_SAVED"), _t("DRONE_SAVED_DESC"));
            savedDronesMenu = new Menu(_t("DRONE_SAVED_TITLE"));
            MenuController.AddSubmenu(menu, savedDronesMenu);
            menu.AddMenuItem(savedDronesButton);
            savedDronesButton.Label = "→→→";
            MenuController.BindMenuItem(menu, savedDronesMenu, savedDronesButton);

            MenuItem saveDrone = new MenuItem(_t("DRONE_SAVE_CURRENT"), _t("DRONE_SAVE_CURRENT_DESC"));
            savedDronesMenu.AddMenuItem(saveDrone);
            savedDronesMenu.OnMenuOpen += (sender) => {
                savedDronesMenu.ClearMenuItems();
                savedDronesMenu.AddMenuItem(saveDrone);
                LoadDroneCameras();
            };

            savedDronesMenu.OnItemSelect += (sender, item, index) => {
                if (item == saveDrone)
                {
                    SaveCamera();
                    savedDronesMenu.GoBack();
                }
                else
                {
                    UpdateSelectedCameraMenu(item, sender);
                }
            };

            MenuController.AddMenu(selectedDroneMenu);
            MenuItem spawnCamera = new MenuItem(_t("DRONE_SPAWN"), _t("DRONE_SPAWN_DESC"));
            MenuItem renameCamera = new MenuItem(_t("DRONE_RENAME"), _t("DRONE_RENAME_DESC"));
            MenuItem deleteCamera = new MenuItem(_t("DRONE_DELETE"), _t("DRONE_DELETE_DESC"));
            selectedDroneMenu.AddMenuItem(spawnCamera);
            selectedDroneMenu.AddMenuItem(renameCamera);
            selectedDroneMenu.AddMenuItem(deleteCamera);

            selectedDroneMenu.OnMenuClose += (sender) => {
                selectedDroneMenu.RefreshIndex();
            };

            selectedDroneMenu.OnItemSelect += async (sender, item, index) => {
                if (item == spawnCamera)
                {
                    MainMenu.ResetCameras();
                    SpawnSavedCamera();
                    UpdateParams();
                    selectedDroneMenu.GoBack();
                    savedDronesMenu.RefreshIndex();

                }
                else if (item == deleteCamera)
                {
                    item.Label = "";
                    DeleteResourceKvp(currentlySelectedDrone.Key);
                    selectedDroneMenu.GoBack();
                    savedDronesMenu.RefreshIndex();
                    MainMenu.Notify("~g~~h~Info~h~~s~: Your saved drone has been deleted.");
                }
                else if (item == renameCamera)
                {
                    string newName = await MainMenu.GetUserInput(windowTitle: "Enter a new name for this drone.", defaultText: null, maxInputLength: 30);
                    if (string.IsNullOrEmpty(newName))
                    {
                        MainMenu.Notify("~r~~h~Error~h~~s~: Invalid input");
                    }
                    else
                    {
                        if (SaveCameraInfo("xdm_" + newName, currentlySelectedDrone.Value, false))
                        {
                            DeleteResourceKvp(currentlySelectedDrone.Key);
                            while (!selectedDroneMenu.Visible)
                            {
                                await BaseScript.Delay(0);
                            }
                            MainMenu.Notify("~g~~h~Info~h~~s~: Your drone has successfully been renamed.");
                            selectedDroneMenu.GoBack();
                            currentlySelectedDrone = new KeyValuePair<string, DroneSaveInfo>();
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

            // Handle checkbox changes
            menu.OnCheckboxChange += (_menu, _item, _index, _checked) => {
                if (_item == invertPitch)
                {
                    invertedPitch = _checked;
                }
                else if (_item == invertRoll)
                {
                    invertedRoll = _checked;
                }
            };

            // Handle sliders
            menu.OnListIndexChange += (_menu, _listItem, _oldIndex, _newIndex, _itemIndex) => {
                if (_listItem == modeList)
                {
                    droneMode = _newIndex;
                    if (droneMode == 3)
                    {
                        homingTarget = GetClosestVehicleToDrone(2000);
                    }
                }

                if (_listItem == gravityMultList)
                {
                    gravityMult = _newIndex * 0.05f + 0.5f;
                }
                if (_listItem == timestepMultList)
                {
                    timestepMult = _newIndex * 0.05f + 0.5f;
                }
                if (_listItem == dragMultList)
                {
                    dragMult = _newIndex * 0.05f;
                }
                if (_listItem == accelerationMultList)
                {
                    accelerationMult = _newIndex * 0.05f + 0.5f;
                }

                if (_listItem == rotationMultXList)
                {
                    rotationMult.X = _newIndex * 0.05f + 0.5f;
                }
                if (_listItem == rotationMultYList)
                {
                    rotationMult.Y = _newIndex * 0.05f + 0.5f;
                }
                if (_listItem == rotationMultZList)
                {
                    rotationMult.Z = _newIndex * 0.05f + 0.5f;
                }

                if (_listItem == maxVelList)
                {
                    maxVel = _newIndex * 1f + 10f;
                }

                if (_listItem == tiltAngleList)
                {
                    tiltAngle = _newIndex * 5.0f;
                }
                if (_listItem == fovList)
                {
                    droneFov = _newIndex * 5.0f + 30f;
                    if (MainMenu.droneCamera != null)
                    {
                        SetCamFov(MainMenu.droneCamera.Handle, droneFov);
                    }
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
        
        #region params

        private DroneInfo drone;

        // Parameters for user to tune
        private static float gravityMult = 1.0f;
        private static float timestepMult = 1.0f;
        private static float dragMult = 1.0f;
        private static Vector3 rotationMult = new Vector3(1f, 1f, 1f);
        private static float accelerationMult = 1f;
        private static float tiltAngle = 45.0f;
        private static float droneFov = 80.0f;
        private static float maxVel = 30.0f;

        // Const drone parameters
        private const float GRAVITY_CONST = 9.8f;       // Gravity force constant
        private const float TIMESTEP_DELIMITER = 90.15f;   // Less - gravity is stronger
        private const float DRONE_DRAG = 0.0020f;        // Air resistance
        private const float DRONE_AGILITY_ROT = 55000f;   // How quick is rotational response of the drone
        private const float DRONE_AGILITY_VEL = 210f; // How quick is velocity and acceleration response
        private const float DRONE_MAX_VELOCITY = 0.01f; // Max velocity of the drone

        #endregion

        #region main functions

        /// <summary>
        /// Changes main render camera behaviour, creates a free camera controlled
        /// like a drone.
        /// </summary>
        /// <returns></returns>
        private async Task RunDroneCam()
        {
            if (DroneCamVar)
            {
                if (MainMenu.droneCamera != null)
                {
                    // Get user input
                    UpdateDroneControls();

                    // Update camera properties
                    if (droneMode == 2)
                    {   // Spectate mode
                        UpdateDronePositionSpectate();
                        UpdateDroneRotationSpectate();
                    }
                    else if (droneMode == 3)
                    {    // Homing mode
                        UpdateDronePositionSpectate();
                        UpdateDroneRotationHoming();
                    }
                    else
                    {
                        UpdateDronePosition();
                        UpdateDroneRotation();
                    }
                }
                else
                {
                    CreateDroneCamera();
                }
            }
            else
            {
                await Delay(0);
            }
        }

        /// <summary>
        /// Move player a bit every 250 seconds to avoid AFK kick
        /// when using drone.
        /// </summary>
        /// <returns></returns>
        private async Task AntiAfk()
        {
            if (menu != null)
            {
                if (DroneCamVar)
                {
                    SimulatePlayerInputGait(Game.Player.Handle, 1.0f, 100, 0.2f, true, false);
                    await Delay(250000);
                }
            }
            else
            {
                await Delay(0);
            }
        }

        private void CreateDroneCamera()
        {
            MainMenu.ResetCameras();
            MainMenu.droneCamera = MainMenu.CreateNonAttachedCamera();
            MainMenu.droneCamera.FieldOfView = droneFov;
            MainMenu.droneCamera.IsActive = true;
            drone = new DroneInfo
            {
                velocity = Vector3.Zero,
                downVelocity = 0f,
                rotation = new Quaternion(0f, 0f, 0f, 1f)
            };
            Game.Player.CanControlCharacter = false;
        }

        // Struct containing all the necessary info for tracking drone
        // movement.
        private struct DroneInfo
        {
            // User input
            public float acceleration;
            public float deceleration;
            public float controlPitch;
            public float controlYaw;
            public float controlRoll;
            // Current values
            public Vector3 velocity;        // Drone's velocity in all directions
            public float downVelocity;      // Velocity caused by gravity
            public Quaternion rotation;     // Drone rotation in quaternion
        }

        private void DumpDebug()
        {
            Debug.WriteLine(drone.acceleration.ToString() +
                            drone.controlPitch.ToString() +
                            drone.controlYaw.ToString() +
                            drone.controlRoll.ToString() +
                            drone.velocity.ToString() +
                            drone.downVelocity.ToString() +
                            drone.rotation.X.ToString() +
                            drone.rotation.Y.ToString() +
                            drone.rotation.Z.ToString() +
                            drone.rotation.W.ToString() +
                            MainMenu.droneCamera.Position.ToString()
                            );
        }

        // Get user input for drone camera
        private void UpdateDroneControls()
        {
            drone.acceleration = ((GetDisabledControlNormal(0, 71)) / 2f);
            drone.deceleration = ((GetDisabledControlNormal(0, 72)) / 2f);
            drone.controlPitch = ((GetDisabledControlNormal(1, 2)) / 2f);
            drone.controlYaw = -((GetDisabledControlNormal(1, 9)) / 2f);
            drone.controlRoll = ((GetDisabledControlNormal(1, 1)) / 2f);

            // Account for mouse controls
            if (IsInputDisabled(1))
            {
                drone.controlPitch *= 3.5f;
                drone.controlYaw *= 0.55f;
                drone.controlRoll *= 4.5f;
            }
        }

        #endregion

        #region race mode

        // Update drone's rotation based on input
        private void UpdateDroneRotation()
        {
            float deltaTime = timestepMult * Timestep() / TIMESTEP_DELIMITER;

            // Calculate delta of rotation based on user input
            float deltaPitch = drone.controlPitch * DRONE_AGILITY_ROT * 0.70f * rotationMult.X * deltaTime;
            float deltaYaw = drone.controlYaw * DRONE_AGILITY_ROT * 0.6f * rotationMult.Z * deltaTime;
            float deltaRoll = drone.controlRoll * DRONE_AGILITY_ROT * 0.75f * rotationMult.Y * deltaTime;

            // Account for inverted axes
            deltaPitch *= (invertedPitch) ? (-1f) : (1f);
            deltaRoll *= (invertedRoll) ? (-1f) : (1f);

            // Rotate quaternion
            drone.rotation *= Quaternion.RotationAxis(Vector3.Up, deltaRoll * MainMenu.CamMath.DegToRad);
            drone.rotation *= Quaternion.RotationAxis(Vector3.Right, deltaPitch * MainMenu.CamMath.DegToRad);
            drone.rotation *= Quaternion.RotationAxis(Vector3.ForwardLH, deltaYaw * MainMenu.CamMath.DegToRad);

            // Update camera rotation based on values
            Vector3 eulerRot = MainMenu.CamMath.QuaternionToEuler(drone.rotation);
            SetCamRot(MainMenu.droneCamera.Handle, eulerRot.X, eulerRot.Y, eulerRot.Z, 2);
        }

        // Implementation of drone's physics engine
        private void UpdateDronePosition()
        {
            // For dividing velocity into two vectors based on camera tilt
            // compared to drone itself
            float staticTilt = Tan(tiltAngle);

            // Timeframe used for calculations
            float deltaTime = timestepMult * Timestep() / TIMESTEP_DELIMITER;

            // Calculate impact of gravity force
            float deltaDownForce = GRAVITY_CONST * gravityMult;      // F = m*a = m*g

            // Calculate velocity based on acceleration
            // Drone is tilted compared to camera, so there are two vectors
            // Forward and up are opposite due to naming conventions mismatch
            float deltaVelocityForward = drone.acceleration * DRONE_AGILITY_VEL * accelerationMult * 0.5f * deltaTime;          // dV = a*dt
            float deltaVelocityUp = drone.acceleration * DRONE_AGILITY_VEL * accelerationMult * (staticTilt / 2f) * deltaTime;  // dV = a*dt
            // Enable deceleration when in zero-G mode and get rid of gravity force
            if (droneMode == 1)
            {
                deltaVelocityForward -= drone.deceleration * DRONE_AGILITY_VEL * accelerationMult * 0.5f * deltaTime;
                deltaVelocityUp += drone.deceleration * DRONE_AGILITY_VEL * accelerationMult * (staticTilt / 2f) * deltaTime;
                deltaDownForce = 0f;
            }

            // Additional 2x boost on spacebar/R1
            float boost = (GetDisabledControlNormal(1, 102) + 1f);

            drone.velocity += MainMenu.droneCamera.ForwardVector * deltaVelocityForward * boost;    // V1 = V0 + dV
            drone.velocity -= MainMenu.droneCamera.UpVector * deltaVelocityUp * boost;              // V1 = V0 + dV
            // Account for air resistance
            drone.velocity -= drone.velocity * DRONE_DRAG * dragMult;
            drone.velocity += Vector3.ForwardLH * deltaDownForce * deltaTime;

            // Clamp velocity to maximum with some smoothing
            if (Math.Abs(drone.velocity.Length()) > boost * maxVel * DRONE_MAX_VELOCITY)
            {
                drone.velocity = Vector3.Lerp(drone.velocity, drone.velocity * boost * maxVel * DRONE_MAX_VELOCITY / drone.velocity.Length(), 0.08f);
            }

            // Update camera position based on velocity values
            MainMenu.droneCamera.Position -= drone.velocity;
        }

        #endregion

        #region spectator mode

        // Special update functions for spectator mode drone
        private void UpdateDroneRotationSpectate()
        {
            float deltaTime = timestepMult * Timestep() / TIMESTEP_DELIMITER;

            // Calculate delta of rotation based on user input
            float deltaPitch = -drone.controlPitch * DRONE_AGILITY_ROT * 0.70f * rotationMult.X * deltaTime;
            float deltaYaw = -drone.controlRoll * DRONE_AGILITY_ROT * 0.6f * rotationMult.Z * deltaTime;

            // Account for inverted axes
            deltaPitch *= (invertedPitch) ? (-1f) : (1f);

            // Update camera rotation based on values
            SetCamRot(MainMenu.droneCamera.Handle,
                Math.Abs(MainMenu.droneCamera.Rotation.X + deltaPitch) < 89f ? (MainMenu.droneCamera.Rotation.X + deltaPitch)
                                                                                           : (Math.Sign(MainMenu.droneCamera.Rotation.X) * 88.9f),
                0f,
                MainMenu.droneCamera.Rotation.Z + deltaYaw,
                2);
        }

        private void UpdateDronePositionSpectate()
        {
            float deltaTime = timestepMult * Timestep() / TIMESTEP_DELIMITER;

            float deltaForward = -((GetDisabledControlNormal(1, 31)) / 2f) * deltaTime * DRONE_AGILITY_VEL * accelerationMult / 2f;
            float deltaSide = ((GetDisabledControlNormal(1, 30)) / 2f) * deltaTime * DRONE_AGILITY_VEL * accelerationMult / 2f;
            float deltaUp = ((GetDisabledControlNormal(1, 92)) / 2f) * deltaTime * DRONE_AGILITY_VEL * accelerationMult;
            float deltaDown = ((GetDisabledControlNormal(1, 91)) / 2f) * deltaTime * DRONE_AGILITY_VEL * accelerationMult;

            // Additional 2x boost on spacebar/R1
            float boost = (GetDisabledControlNormal(1, 102) + 1f);

            Vector3 dir = MainMenu.CamMath.RotateAroundAxis(MainMenu.droneCamera.Direction, MainMenu.droneCamera.RightVector, 90f * MainMenu.CamMath.DegToRad);

            drone.velocity -= Vector3.Normalize(new Vector3(dir.X,
                                          dir.Y,
                                          0f)) * deltaForward * boost;
            drone.velocity += MainMenu.CamMath.RotateAroundAxis(
                                Vector3.Normalize(new Vector3(dir.X,
                                                                dir.Y,
                                                                0f)),
                                    Vector3.ForwardLH,
                                    90f * MainMenu.CamMath.DegToRad
                                ) * deltaSide * boost;


            // Account for air ressistance
            drone.velocity -= drone.velocity * DRONE_DRAG * dragMult;

            drone.velocity -= Vector3.ForwardLH * deltaUp;
            drone.velocity += Vector3.ForwardLH * deltaDown;

            // Clamp velocity to maximum with some smoothing
            if (Math.Abs(drone.velocity.Length()) > maxVel * boost * DRONE_MAX_VELOCITY)
            {
                drone.velocity = Vector3.Lerp(drone.velocity, drone.velocity * boost * maxVel * DRONE_MAX_VELOCITY / drone.velocity.Length(), 0.08f);
            }

            // Update camera position based on velocity values
            MainMenu.droneCamera.Position -= drone.velocity;
        }

        #endregion

        #region homing mode

        /// <summary>
        /// Gets closest vehicle to Camera
        /// </summary>
        /// <returns>closest vehicle</returns>
        private Vehicle GetClosestVehicleToDrone(int maxDistance)
        {
            float smallestDistance = (float)maxDistance;
            Vehicle[] vehs = World.GetAllVehicles();
            Vehicle closestVeh = null;

            if (vehs != null)
            {
                foreach (Vehicle veh in vehs)
                {
                    float distance = Vector3.Distance(GetEntityCoords(veh.Handle, true), MainMenu.droneCamera.Position);
                    if ((distance <= smallestDistance) && (veh != null))
                    {
                        smallestDistance = distance;
                        closestVeh = veh;
                    }
                }
            }
            return closestVeh;
        }

        private void UpdateDroneRotationHoming()
        {
            float deltaTime = timestepMult * Timestep() / TIMESTEP_DELIMITER;

            if (homingTarget != null)
            {
                Vector3 targetDir = homingTarget.Position - MainMenu.droneCamera.Position;

                MainMenu.droneCamera.Direction = targetDir;
            }
        }

        #endregion
        
        /// ---
        /// Save/load functions originally made by Vespura (https://www.tomgrobbe.com/) for vMenu.
        /// Snippets of the code were slightly modified to suit camera needs and added here.
        /// ---
        #region save/load

        public struct DroneSaveInfo
        {
            public float gravityMult_;
            public float timestepMult_;
            public float dragMult_;
            public float accelerationMult_;
            public Vector3 rotationMult_;
            public float maxVel_;
            public float tiltAngle_;
            public float droneFov_;
        }

        private bool UpdateSelectedCameraMenu(MenuItem selectedItem, Menu parentMenu = null)
        {
            if (!sdMenuItems.ContainsKey(selectedItem))
            {
                MainMenu.Notify("~r~~h~Error~h~~s~: In some very strange way, you've managed to select a button, that does not exist according to this list. So your vehicle could not be loaded. :( Maybe your save files are broken?");
                return false;
            }
            var camInfo = sdMenuItems[selectedItem];
            currentlySelectedDrone = camInfo;
            selectedDroneMenu.MenuSubtitle = $"{camInfo.Key.Substring(4)}";
            MenuController.CloseAllMenus();
            selectedDroneMenu.OpenMenu();
            if (parentMenu != null)
            {
                MenuController.AddSubmenu(parentMenu, selectedDroneMenu);
            }
            return true;
        }

        private bool SpawnSavedCamera()
        {
            if (currentlySelectedDrone.Key != null)
            {
                gravityMult = currentlySelectedDrone.Value.gravityMult_;
                timestepMult = currentlySelectedDrone.Value.timestepMult_;
                dragMult = currentlySelectedDrone.Value.dragMult_;
                accelerationMult = currentlySelectedDrone.Value.accelerationMult_;
                rotationMult = currentlySelectedDrone.Value.rotationMult_;
                maxVel = currentlySelectedDrone.Value.maxVel_;
                tiltAngle = currentlySelectedDrone.Value.tiltAngle_;
                droneFov = currentlySelectedDrone.Value.droneFov_;
            }
            else
            {
                MainMenu.Notify("~r~~h~Error~h~~s~: It seems that this slot got corrupted in some way, you need to delete it.");
                return false;
            }
            return true;
        }

        private bool SaveCameraInfo(string saveName, DroneSaveInfo cameraInfo, bool overrideOldVersion)
        {
            if (string.IsNullOrEmpty(GetResourceKvpString(saveName)) || overrideOldVersion)
            {
                if (!string.IsNullOrEmpty(saveName) && saveName.Length > 4)
                {
                    // convert
                    string json = JsonConvert.SerializeObject(cameraInfo);

                    // log
                    Debug.WriteLine($"Saving!\nName: {saveName}\nDrone Data: {json}\n");

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
            DroneSaveInfo ci = new DroneSaveInfo()
            {
                gravityMult_ = gravityMult,
                timestepMult_ = timestepMult,
                dragMult_ = dragMult,
                accelerationMult_ = accelerationMult,
                rotationMult_ = rotationMult,
                maxVel_ = maxVel,
                tiltAngle_ = tiltAngle,
                droneFov_ = droneFov
            };

            if (updateExistingSavedCameraName == null)
            {
                var saveName = await MainMenu.GetUserInput(windowTitle: "Enter a save name", defaultText: null, maxInputLength: 30);
                // If the name is not invalid.
                if (!string.IsNullOrEmpty(saveName))
                {
                    // Save everything from the dictionary into the client's kvp storage.
                    // If the save was successfull:
                    if (SaveCameraInfo("xdm_" + saveName, ci, false))
                    {
                        MainMenu.Notify($"~g~~h~Info~h~~s~: Drone {saveName} saved.");
                        LoadDroneCameras();
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
                SaveCameraInfo("xdm_" + updateExistingSavedCameraName, ci, true);
            }
        }

        private Dictionary<string, DroneSaveInfo> GetSavedCameras()
        {
            // Create a list to store all saved camera names in.
            var savedCameraNames = new List<string>();
            // Start looking for kvps starting with xcm_
            var findHandle = StartFindKvp("xdm_");
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
            var camerasList = new Dictionary<string, DroneSaveInfo>();
            // Loop through all save names (keys) from the list above, convert the string into a dictionary 
            // and add it to the dictionary above, with the camera save name as the key.
            foreach (var saveName in savedCameraNames)
            {
                camerasList.Add(saveName, JsonConvert.DeserializeObject<DroneSaveInfo>(GetResourceKvpString(saveName)));
            }
            // Return the camera dictionary containing all camera save names (keys) linked to the correct camera
            return camerasList;
        }

        private async void LoadDroneCameras()
        {
            var savedCameras = GetSavedCameras();
            sdMenuItems = new Dictionary<MenuItem, KeyValuePair<string, DroneSaveInfo>>();

            foreach (var sc in savedCameras)
            {
                MenuItem savedDroneBtn;
                if (sc.Key.Length > 4)
                {
                    savedDroneBtn = new MenuItem(sc.Key.Substring(4), $"Manage this saved drone.")
                    {
                        Label = $"→→→"
                    };
                }
                else
                {
                    savedDroneBtn = new MenuItem("NULL", $"Manage this saved drone.")
                    {
                        Label = $"→→→"
                    };
                }
                savedDronesMenu.AddMenuItem(savedDroneBtn);
                sdMenuItems.Add(savedDroneBtn, sc);
            }
            await Delay(0);
        }

        #endregion

        private static string _t(string key) {
            return Language.get(key);
        }
    }
}