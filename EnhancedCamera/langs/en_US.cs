using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCamera.langs {
    class en_US {
        public Hashtable data() {
            
            Hashtable ht = new Hashtable();
            
            // Main Menu
            ht.Add("MAIN_MENU_TITLE", "Enhanced camera");
            ht.Add("MAIN_MENU_DESC", "Lead, chase and drone camera options");
            ht.Add("MAIN_MENU_ENABLE_LEAD", "Enable lead camera");
            ht.Add("MAIN_MENU_ENABLE_CHASE", "Enable chase camera");
            ht.Add("MAIN_MENU_ENABLE_DRONE", "Enable drone camera");
            ht.Add("MAIN_MENU_ENABLE_LEAD_DESC", "Main camera, behaviour dependant on angular velocity of the car.");
            ht.Add("MAIN_MENU_ENABLE_CHASE_DESC", "Locks to a target in front, switches to regular cam if target not in range.");
            ht.Add("MAIN_MENU_ENABLE_DRONE_DESC", "Free drone camera to spectate/fly around. Different modes available.");
            ht.Add("MAIN_MENU_LEAD_CHASE_CONF", "Lead/chase cam parameters");
            ht.Add("MAIN_MENU_DRONE_CONF", "Drone cam parameters");
            ht.Add("MAIN_MENU_LEAD_CHASE_CONF_DESC", "Tune parameters for lead and chase camera");
            ht.Add("MAIN_MENU_DRONE_CONF_DESC", "Tune parameters for drone camera");
            ht.Add("MAIN_MENU_CREDITS", "Credits");
            ht.Add("MAIN_MENU_CREDITS_DESC", "~g~Shrimp~s~ - idea and execution\n" +
                                                        "~g~Tom Grobbe~s~ - MenuAPI used for GUI, code snippets for saving/loading\n" +
                                                        "~g~QuadrupleTurbo~s~ - Help with ideas and testing\n" +
                                                        "~y~No Name Drift~s~ and ~y~Velocity~s~ drift servers - playtesting and feedback\n" +
                                                        "~r~ZeroDream~s~ - Implement menu localization and Chinese translate\n");
            
            // Custom Camera
            ht.Add("CUSTOM_CAM_MANAGE", "Manage Camera");
            ht.Add("CUSTOM_CAM_MANAGE_DESC", "Manage this saved camera.");
            ht.Add("CUSTOM_CAM_TITLE", "Enhanced camera");
            ht.Add("CUSTOM_CAM_DESC", "Lead/chase camera parameters");
            ht.Add("CUSTOM_CAM_LOCK_POSITION", "Lock position offset");
            ht.Add("CUSTOM_CAM_LOCK_POSITION_DESC", "Locks position offset, useful when sticking camera to the car - on top of hood, as FPV cam, etc.");
            ht.Add("CUSTOM_CAM_LINEAR", "Linear position offset");
            ht.Add("CUSTOM_CAM_LINEAR_DESC", "Instead of circular motion around the car, the camera moves along car's X axis. Dope for cinematic shots.");
            ht.Add("CUSTOM_CAM_LOCK_ROTATE", "Lock rotation to camera plane");
            ht.Add("CUSTOM_CAM_LOCK_ROTATE_DESC", "Changes the way that camera rotates around car (mostly visible on uneven ground).");
            ht.Add("CUSTOM_CAM_MODIFIER", "Modifier");
            ht.Add("CUSTOM_CAM_MODIFIER_DESC", "This modifier * angular velocity = target rotation. Higher values make camera move further from lock. (-1,1)");
            ht.Add("CUSTOM_CAM_YAW", "Yaw interpolation");
            ht.Add("CUSTOM_CAM_YAW_DESC", "Lower values - smoother movement. WARNING: Slider is inversed for chase camera - 0 is max interpolation, 1 is complete lock. (0,1)");
            ht.Add("CUSTOM_CAM_ROLL", "Roll interpolation");
            ht.Add("CUSTOM_CAM_ROLL_DESC", "Lower values - smoother movement. (0,1)");
            ht.Add("CUSTOM_CAM_PITCH", "Pitch interpolation");
            ht.Add("CUSTOM_CAM_PITCH_DESC", "Lower values - smoother movement. (0,1)");
            ht.Add("CUSTOM_CAM_OFFSET", "Camera offset");
            ht.Add("CUSTOM_CAM_OFFSET_DESC", "Offsets chase camera target towards its velocity vector. (0,5)");
            ht.Add("CUSTOM_CAM_POSITION", "Position interpolation");
            ht.Add("CUSTOM_CAM_POSITION_DESC", "Lower values - smoother movement, higher delay. (0,1)");
            ht.Add("CUSTOM_CAM_FOV", "FOV");
            ht.Add("CUSTOM_CAM_FOV_DESC", "Change custom camera's FOV. (20,120)");
            ht.Add("CUSTOM_CAM_Y_OFFSET", "Y offset");
            ht.Add("CUSTOM_CAM_Y_OFFSET_DESC", "Custom camera offset in forward direction. (-8,8)");
            ht.Add("CUSTOM_CAM_X_OFFSET", "X offset");
            ht.Add("CUSTOM_CAM_X_OFFSET_DESC", "Custom camera offset in side direction. (-5,8)");
            ht.Add("CUSTOM_CAM_Z_OFFSET", "Z offset");
            ht.Add("CUSTOM_CAM_Z_OFFSET_DESC", "Custom camera offset in up direction. (-5,8)");
            ht.Add("CUSTOM_CAM_MAX_ANGLE", "Max angle to lock.");
            ht.Add("CUSTOM_CAM_MAX_ANGLE_DESC", "Max angle from velocity vector to keep the lock on, if angle exceeds this limit, camera switches back to normal. (25,360)");
            ht.Add("CUSTOM_CAM_PRESETS", "Presets");
            ht.Add("CUSTOM_CAM_PRESETS_DESC", "Spawn camera presets");
            ht.Add("CUSTOM_CAM_PRESETS_TANDEM", "Tandem Camera 1.0");
            ht.Add("CUSTOM_CAM_PRESETS_TANDEM_DESC", "Chef's specialty");
            ht.Add("CUSTOM_CAM_PRESETS_FPV", "FPV camera base");
            ht.Add("CUSTOM_CAM_PRESETS_FPV_DESC", "Best to use with chicken model");
            ht.Add("CUSTOM_CAM_PRESETS_NFS", "NFS camera");
            ht.Add("CUSTOM_CAM_PRESETS_NFS_DESC", "Arcade game experience");
            ht.Add("CUSTOM_CAM_PRESETS_MENU", "Presets menu");
            ht.Add("CUSTOM_CAM_PRESETS_MENU_DESC", "Get started here");
            ht.Add("CUSTOM_CAM_SAVED", "Saved cameras");
            ht.Add("CUSTOM_CAM_SAVED_DESC", "User created cameras");
            ht.Add("CUSTOM_CAM_SAVE_CURRENT", "Save Current Camera");
            ht.Add("CUSTOM_CAM_SAVE_CURRENT_DESC", "Save the current camera.");
            ht.Add("CUSTOM_CAM_SPAWN", "Spawn Camera");
            ht.Add("CUSTOM_CAM_SPAWN_DESC", "Spawn this saved camera.");
            ht.Add("CUSTOM_CAM_RENAME", "Rename Camera");
            ht.Add("CUSTOM_CAM_RENAME_DESC", "Rename your saved camera.");
            ht.Add("CUSTOM_CAM_DELETE", "~r~Delete Camera");
            ht.Add("CUSTOM_CAM_DELETE_DESC", "~r~This will delete your saved camera. Warning: this can NOT be undone!");

            // Drones Menu
            ht.Add("DRONE_MANAGE_TITLE", "Manage Drone");
            ht.Add("DRONE_MANAGE_DESC", "Manage this saved drone parameters.");
            ht.Add("DRONE_TITLE", "Enhanced camera");
            ht.Add("DRONE_DESC", "Drone Camera parameters");
            ht.Add("DRONE_RACE", "Race drone");
            ht.Add("DRONE_ZERO_G", "Zero-G drone");
            ht.Add("DRONE_SPECTATOR", "Spectator drone");
            ht.Add("DRONE_HOMING", "Homing drone");
            ht.Add("DRONE_INVERT_PITCH", "Invert pitch");
            ht.Add("DRONE_INVERT_PITCH_DESC", "Inverts user input in pitch axis.");
            ht.Add("DRONE_INVERT_ROLL", "Invert roll");
            ht.Add("DRONE_INVERT_ROLL_DESC", "Inverts user input in roll axis.");
            ht.Add("DRONE_GRAVITY", "Gravity multiplier");
            ht.Add("DRONE_GRAVITY_DESC", "Modifies gravity constant, higher values makes drone fall quicker during freefall.");
            ht.Add("DRONE_TIMESTEP", "Timestep multiplier");
            ht.Add("DRONE_TIMESTEP_DESC", "Affects gravity and drone responsiveness.");
            ht.Add("DRONE_DRAG", "Drag multiplier");
            ht.Add("DRONE_DRAG_DESC", "How much air ressistance there is - higher values make drone lose velocity quicker.");
            ht.Add("DRONE_ACCELE", "Acceleration multiplier");
            ht.Add("DRONE_ACCELE_DESC", "How responsive drone is in terms of acceleration.");
            ht.Add("DRONE_PITCH", "Pitch multiplier");
            ht.Add("DRONE_PITCH_DESC", "How responsive drone is in terms of rotation (pitch).");
            ht.Add("DRONE_ROLL", "Roll multiplier");
            ht.Add("DRONE_ROLL_DESC", "How responsive drone is in terms of rotation (roll).");
            ht.Add("DRONE_YAW", "Yaw multiplier");
            ht.Add("DRONE_YAW_DESC", "How responsive drone is in terms of rotation (yaw).");
            ht.Add("DRONE_TILT", "Tilt angle");
            ht.Add("DRONE_TILT_DESC", "Defines how much is camera tilted relative to the drone.");
            ht.Add("DRONE_FOV", "FOV");
            ht.Add("DRONE_FOV_DESC", "Field of view of the camera");
            ht.Add("DRONE_MAX_VELOCITY", "Max velocity");
            ht.Add("DRONE_MAX_VELOCITY_DESC", "Max velocity of the drone");
            ht.Add("DRONE_SAVED", "Saved drones");
            ht.Add("DRONE_SAVED_DESC", "User created drone params");
            ht.Add("DRONE_SAVED_TITLE", "Saved drone params");
            ht.Add("DRONE_SAVE_CURRENT", "Save Current Drone");
            ht.Add("DRONE_SAVE_CURRENT_DESC", "Save the current drone parameters.");
            ht.Add("DRONE_SPAWN", "Spawn Drone");
            ht.Add("DRONE_SPAWN_DESC", "Spawn this saved drone.");
            ht.Add("DRONE_RENAME", "Rename Drone");
            ht.Add("DRONE_RENAME_DESC", "Rename your saved drone.");
            ht.Add("DRONE_DELETE", "~r~Delete Drone");
            ht.Add("DRONE_DELETE_DESC", "~r~This will delete your saved drone. Warning: this can NOT be undone!");
            ht.Add("DRONE_MODE", "Mode");
            ht.Add("DRONE_MODE_DESC", "Select drone flight mode.\n" +
                                                                    "~r~Race drone~s~ - regular, gravity based drone cam\n" +
                                                                    "~g~Zero-G drone~s~ - gravity set to 0, added deceleration\n" +
                                                                    "~b~Spectator drone~s~ - easy to operate for spectating\n" +
                                                                    "~y~Homing drone~s~ - acquire target and keep it in center");
            return ht;
        }
    }
}
