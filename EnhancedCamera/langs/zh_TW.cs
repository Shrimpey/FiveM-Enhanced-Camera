using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCamera.langs {
    class zh_TW {
        public Hashtable data() {

            Hashtable ht = new Hashtable();

            // Main Menu
            ht.Add("MAIN_MENU_TITLE", "增強型遊戲鏡頭");
            ht.Add("MAIN_MENU_DESC", "高級鏡頭和無人機鏡頭設置");
            ht.Add("MAIN_MENU_ENABLE_LEAD", "啟用漂移鏡頭");
            ht.Add("MAIN_MENU_ENABLE_CHASE", "啟用追踪鏡頭");
            ht.Add("MAIN_MENU_ENABLE_DRONE", "啟用無人機鏡頭");
            ht.Add("MAIN_MENU_ENABLE_LEAD_DESC", "主鏡頭，行為取決於汽車的漂移角度和速度");
            ht.Add("MAIN_MENU_ENABLE_CHASE_DESC", "鎖定前方目標，如果目標不在範圍內，則切換到普通鏡頭");
            ht.Add("MAIN_MENU_ENABLE_DRONE_DESC", "自由無人機鏡頭，提供不同的模式，例如穿梭機和無重力飛行");
            ht.Add("MAIN_MENU_LEAD_CHASE_CONF", "漂移和追踪鏡頭設定");
            ht.Add("MAIN_MENU_DRONE_CONF", "無人機鏡頭設定");
            ht.Add("MAIN_MENU_LEAD_CHASE_CONF_DESC", "修改漂移或追踪鏡頭的參數設定");
            ht.Add("MAIN_MENU_DRONE_CONF_DESC", "修改無人機鏡頭的參數設定");
            ht.Add("MAIN_MENU_CREDITS", "鳴謝");
            ht.Add("MAIN_MENU_CREDITS_DESC", "~g~Shrimp~s~ - 提供想法並實現\n" +
                                                        "~g~Tom Grobbe~s~ - MenuAPI 界面庫的作者，以及保存和加載設定的代碼\n" +
                                                        "~g~QuadrupleTurbo~s~ - 提供想法並幫助測試\n" +
                                                        "~y~No Name Drift~s~ 和 ~y~Velocity~s~ 漂移服務器進行遊戲測試和反饋\n" +
                                                        "~y~ZeroDream~s~ 增加多語言支持以及中文翻譯\n");

            // Custom Camera
            ht.Add("CUSTOM_CAM_MANAGE", "管理鏡頭");
            ht.Add("CUSTOM_CAM_MANAGE_DESC", "管理此已保存的鏡頭");
            ht.Add("CUSTOM_CAM_TITLE", "自定義攝像機");
            ht.Add("CUSTOM_CAM_DESC", "漂移和追踪鏡頭設定");
            ht.Add("CUSTOM_CAM_LOCK_POSITION", "鎖定位置偏移");
            ht.Add("CUSTOM_CAM_LOCK_POSITION_DESC", "鎖定鏡頭的位置，在用於固定鏡頭和第一人稱鏡頭時非常有用");
            ht.Add("CUSTOM_CAM_LINEAR", "線性位置偏移");
            ht.Add("CUSTOM_CAM_LINEAR_DESC", "鏡頭不是圍繞車輛做圓周運動，而是沿著汽車的 X 軸移動，適合拍電影");
            ht.Add("CUSTOM_CAM_LOCK_ROTATE", "鎖定鏡頭的水平旋轉");
            ht.Add("CUSTOM_CAM_LOCK_ROTATE_DESC", "修改了鏡頭圍繞車輛旋轉的方式");
            ht.Add("CUSTOM_CAM_MODIFIER", "修改器");
            ht.Add("CUSTOM_CAM_MODIFIER_DESC", "這個修改器 * 角速度 = 旋轉目標，數值越高，則相機越偏離鎖定值 (-1,1)");
            ht.Add("CUSTOM_CAM_YAW", "偏航插值");
            ht.Add("CUSTOM_CAM_YAW_DESC", "數值越低，運動更平穩。警告：追踪鏡頭的設​​定是相反的，0 為最大值，1 為完全鎖定 (0,1)");
            ht.Add("CUSTOM_CAM_ROLL", "滾動插值");
            ht.Add("CUSTOM_CAM_ROLL_DESC", "數值越低，運動更平穩 (0,1)");
            ht.Add("CUSTOM_CAM_PITCH", "俯仰插值");
            ht.Add("CUSTOM_CAM_PITCH_DESC", "數值越低，運動更平穩 (0,1)");
            ht.Add("CUSTOM_CAM_OFFSET", "鏡頭偏移量");
            ht.Add("CUSTOM_CAM_OFFSET_DESC", "設定追踪鏡頭向目標移動的速度矢量偏移 (0,5)");
            ht.Add("CUSTOM_CAM_POSITION", "位置插值");
            ht.Add("CUSTOM_CAM_POSITION_DESC", "數值越低，運動更平穩，但是延遲更高 (0,1)");
            ht.Add("CUSTOM_CAM_FOV", "FOV 視場");
            ht.Add("CUSTOM_CAM_FOV_DESC", "修改鏡頭的視場 (20,120)");
            ht.Add("CUSTOM_CAM_Y_OFFSET", "Y 偏移");
            ht.Add("CUSTOM_CAM_Y_OFFSET_DESC", "修改鏡頭向前的偏移量 (-8,8)");
            ht.Add("CUSTOM_CAM_X_OFFSET", "X 偏移");
            ht.Add("CUSTOM_CAM_X_OFFSET_DESC", "修改相機左右的偏移量 (-5,8)");
            ht.Add("CUSTOM_CAM_Z_OFFSET", "Z 偏移");
            ht.Add("CUSTOM_CAM_Z_OFFSET_DESC", "修改相機上下的偏移量 (-5,8)");
            ht.Add("CUSTOM_CAM_MAX_ANGLE", "最大鎖定角度");
            ht.Add("CUSTOM_CAM_MAX_ANGLE_DESC", "保持鎖定的速度矢量最大角度，超過此角度後將會切換回普通鏡頭");
            ht.Add("CUSTOM_CAM_PRESETS", "預設");
            ht.Add("CUSTOM_CAM_PRESETS_DESC", "應用鏡頭預設");
            ht.Add("CUSTOM_CAM_PRESETS_TANDEM", "追走鏡頭 1.0");
            ht.Add("CUSTOM_CAM_PRESETS_TANDEM_DESC", "適用於漂移追走時使用的鏡頭");
            ht.Add("CUSTOM_CAM_PRESETS_FPV", "FPV 第一人稱");
            ht.Add("CUSTOM_CAM_PRESETS_FPV_DESC", "適用於第一人稱漂移時使用的鏡頭");
            ht.Add("CUSTOM_CAM_PRESETS_NFS", "NFS 鏡頭");
            ht.Add("CUSTOM_CAM_PRESETS_NFS_DESC", "街機遊戲一樣的畫面體驗");
            ht.Add("CUSTOM_CAM_PRESETS_MENU", "預設菜單");
            ht.Add("CUSTOM_CAM_PRESETS_MENU_DESC", "從這裡開始");
            ht.Add("CUSTOM_CAM_SAVED", "保存的鏡頭");
            ht.Add("CUSTOM_CAM_SAVED_DESC", "用戶創建的鏡頭");
            ht.Add("CUSTOM_CAM_SAVE_CURRENT", "保存當前鏡頭");
            ht.Add("CUSTOM_CAM_SAVE_CURRENT_DESC", "將當前的鏡頭設定儲存為預設");
            ht.Add("CUSTOM_CAM_SPAWN", "應用鏡頭預設");
            ht.Add("CUSTOM_CAM_SPAWN_DESC", "應用此保存的鏡頭預設");
            ht.Add("CUSTOM_CAM_RENAME", "重命名預設");
            ht.Add("CUSTOM_CAM_RENAME_DESC", "重命名你已保存的鏡頭預設");
            ht.Add("CUSTOM_CAM_DELETE", "~r~刪除預設");
            ht.Add("CUSTOM_CAM_DELETE_DESC", "~r~這將會刪除此預設。警告：這是不可撤銷的操作");

            // Drones Menu
            ht.Add("DRONE_MANAGE_TITLE", "管理無人機");
            ht.Add("DRONE_MANAGE_DESC", "管理已保存的無人機設定");
            ht.Add("DRONE_TITLE", "無人機");
            ht.Add("DRONE_DESC", "無人機鏡頭參數設定");
            ht.Add("DRONE_RACE", "穿梭機");
            ht.Add("DRONE_ZERO_G", "無重力");
            ht.Add("DRONE_SPECTATOR", "旁觀者");
            ht.Add("DRONE_HOMING", "返航無人機");
            ht.Add("DRONE_INVERT_PITCH", "反向俯仰");
            ht.Add("DRONE_INVERT_PITCH_DESC", "調換俯仰控制的按鍵");
            ht.Add("DRONE_INVERT_ROLL", "反向滾動");
            ht.Add("DRONE_INVERT_ROLL_DESC", "調換滾動控制的按鍵");
            ht.Add("DRONE_GRAVITY", "重力倍數");
            ht.Add("DRONE_GRAVITY_DESC", "修改重力常數，更高的值會使無人機在自由落體運動中下降得更快");
            ht.Add("DRONE_TIMESTEP", "時間倍數");
            ht.Add("DRONE_TIMESTEP_DESC", "影響重力和無人機的響應能力");
            ht.Add("DRONE_DRAG", "阻力倍數");
            ht.Add("DRONE_DRAG_DESC", "設定空氣阻力，越高的值會使無人機更快地失去速度");
            ht.Add("DRONE_ACCELE", "加速倍數");
            ht.Add("DRONE_ACCELE_DESC", "設定無人機的加速性能");
            ht.Add("DRONE_PITCH", "俯仰倍數");
            ht.Add("DRONE_PITCH_DESC", "設定無人機的俯仰響應速度 (pitch).");
            ht.Add("DRONE_ROLL", "滾動倍數");
            ht.Add("DRONE_ROLL_DESC", "設定無人機的滾動響應速度 (roll).");
            ht.Add("DRONE_YAW", "偏航倍數");
            ht.Add("DRONE_YAW_DESC", "設定無人機的偏航響應速度 (yaw).");
            ht.Add("DRONE_TILT", "傾斜角度");
            ht.Add("DRONE_TILT_DESC", "設定鏡頭相對於無人機的傾斜角度");
            ht.Add("DRONE_FOV", "FOV 視場");
            ht.Add("DRONE_FOV_DESC", "設定鏡頭的視場");
            ht.Add("DRONE_MAX_VELOCITY", "最高速度");
            ht.Add("DRONE_MAX_VELOCITY_DESC", "設定無人機的最高速度");
            ht.Add("DRONE_SAVED", "已保存的無人機");
            ht.Add("DRONE_SAVED_DESC", "用戶創建的無人機");
            ht.Add("DRONE_SAVED_TITLE", "已保存的無人機");
            ht.Add("DRONE_SAVE_CURRENT", "保存當前無人機");
            ht.Add("DRONE_SAVE_CURRENT_DESC", "保存當前的無人機參數");
            ht.Add("DRONE_SPAWN", "應用無人機參數");
            ht.Add("DRONE_SPAWN_DESC", "應用此已保存的無人機參數");
            ht.Add("DRONE_RENAME", "重命名無人機");
            ht.Add("DRONE_RENAME_DESC", "重命名你已保存的無人機");
            ht.Add("DRONE_DELETE", "~r~刪除無人機");
            ht.Add("DRONE_DELETE_DESC", "~r~這將會刪除你已保存的無人機。警告：這是不可撤銷的操作");
            ht.Add("DRONE_MODE", "模式");
            ht.Add("DRONE_MODE_DESC", "選擇無人機的飛行模式\n" +
                                                                    "~r~Race 穿梭機~s~ - 普通，帶有重力的無人機\n" +
                                                                    "~g~Zero-G 無重力~s~ - 沒有重力的無人機，增加了減速\n" +
                                                                    "~b~Spectator 旁觀者~s~ - 方便操作進行旁觀\n" +
                                                                    "~y~Homing 返航機~s~ - 獲取目標並將其作為中心點");
            return ht;
        }
    }
}
