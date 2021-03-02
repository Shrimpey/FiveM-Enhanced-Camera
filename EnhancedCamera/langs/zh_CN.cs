using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCamera.langs {
    class zh_CN {
        public Hashtable data() {

            Hashtable ht = new Hashtable();

            // Main Menu
            ht.Add("MAIN_MENU_TITLE", "增强型游戏镜头");
            ht.Add("MAIN_MENU_DESC", "高级镜头和无人机镜头设置");
            ht.Add("MAIN_MENU_ENABLE_LEAD", "启用漂移镜头");
            ht.Add("MAIN_MENU_ENABLE_CHASE", "启用追踪镜头");
            ht.Add("MAIN_MENU_ENABLE_DRONE", "启用无人机镜头");
            ht.Add("MAIN_MENU_ENABLE_LEAD_DESC", "主镜头，行为取决于汽车的漂移角度和速度");
            ht.Add("MAIN_MENU_ENABLE_CHASE_DESC", "锁定前方目标，如果目标不在范围内，则切换到普通镜头");
            ht.Add("MAIN_MENU_ENABLE_DRONE_DESC", "自由无人机镜头，提供不同的模式，例如穿梭机和无重力飞行");
            ht.Add("MAIN_MENU_LEAD_CHASE_CONF", "漂移和追踪镜头设定");
            ht.Add("MAIN_MENU_DRONE_CONF", "无人机镜头设定");
            ht.Add("MAIN_MENU_LEAD_CHASE_CONF_DESC", "修改漂移或追踪镜头的参数设定");
            ht.Add("MAIN_MENU_DRONE_CONF_DESC", "修改无人机镜头的参数设定");
            ht.Add("MAIN_MENU_CREDITS", "鸣谢");
            ht.Add("MAIN_MENU_CREDITS_DESC", "~g~Shrimp~s~ - 提供想法并实现\n" +
                                                        "~g~Tom Grobbe~s~ - MenuAPI 界面库的作者，以及保存和加载设定的代码\n" +
                                                        "~g~QuadrupleTurbo~s~ - 提供想法并帮助测试\n" +
                                                        "~y~No Name Drift~s~ 和 ~y~Velocity~s~ 漂移服务器进行游戏测试和反馈\n" +
                                                        "~y~ZeroDream~s~ 增加多语言支持以及中文翻译\n");

            // Custom Camera
            ht.Add("CUSTOM_CAM_MANAGE", "管理镜头");
            ht.Add("CUSTOM_CAM_MANAGE_DESC", "管理此已保存的镜头");
            ht.Add("CUSTOM_CAM_TITLE", "自定义摄像机");
            ht.Add("CUSTOM_CAM_DESC", "漂移和追踪镜头设定");
            ht.Add("CUSTOM_CAM_LOCK_POSITION", "锁定位置偏移");
            ht.Add("CUSTOM_CAM_LOCK_POSITION_DESC", "锁定镜头的位置，在用于固定镜头和第一人称镜头时非常有用");
            ht.Add("CUSTOM_CAM_LINEAR", "线性位置偏移");
            ht.Add("CUSTOM_CAM_LINEAR_DESC", "镜头不是围绕车辆做圆周运动，而是沿着汽车的 X 轴移动，适合拍电影");
            ht.Add("CUSTOM_CAM_LOCK_ROTATE", "锁定镜头的水平旋转");
            ht.Add("CUSTOM_CAM_LOCK_ROTATE_DESC", "修改了镜头围绕车辆旋转的方式");
            ht.Add("CUSTOM_CAM_MODIFIER", "修改器");
            ht.Add("CUSTOM_CAM_MODIFIER_DESC", "这个修改器 * 角速度 = 旋转目标，数值越高，则相机越偏离锁定值 (-1,1)");
            ht.Add("CUSTOM_CAM_YAW", "偏航插值");
            ht.Add("CUSTOM_CAM_YAW_DESC", "数值越低，运动更平稳。警告：追踪镜头的设定是相反的，0 为最大值，1 为完全锁定 (0,1)");
            ht.Add("CUSTOM_CAM_ROLL", "滚动插值");
            ht.Add("CUSTOM_CAM_ROLL_DESC", "数值越低，运动更平稳 (0,1)");
            ht.Add("CUSTOM_CAM_PITCH", "俯仰插值");
            ht.Add("CUSTOM_CAM_PITCH_DESC", "数值越低，运动更平稳 (0,1)");
            ht.Add("CUSTOM_CAM_OFFSET", "镜头偏移量");
            ht.Add("CUSTOM_CAM_OFFSET_DESC", "设定追踪镜头向目标移动的速度矢量偏移 (0,5)");
            ht.Add("CUSTOM_CAM_POSITION", "位置插值");
            ht.Add("CUSTOM_CAM_POSITION_DESC", "数值越低，运动更平稳，但是延迟更高 (0,1)");
            ht.Add("CUSTOM_CAM_FOV", "FOV 视场");
            ht.Add("CUSTOM_CAM_FOV_DESC", "修改镜头的视场 (20,120)");
            ht.Add("CUSTOM_CAM_Y_OFFSET", "Y 偏移");
            ht.Add("CUSTOM_CAM_Y_OFFSET_DESC", "修改镜头向前的偏移量 (-8,8)");
            ht.Add("CUSTOM_CAM_X_OFFSET", "X 偏移");
            ht.Add("CUSTOM_CAM_X_OFFSET_DESC", "修改相机左右的偏移量 (-5,8)");
            ht.Add("CUSTOM_CAM_Z_OFFSET", "Z 偏移");
            ht.Add("CUSTOM_CAM_Z_OFFSET_DESC", "修改相机上下的偏移量 (-5,8)");
            ht.Add("CUSTOM_CAM_MAX_ANGLE", "最大锁定角度");
            ht.Add("CUSTOM_CAM_MAX_ANGLE_DESC", "保持锁定的速度矢量最大角度，超过此角度后将会切换回普通镜头");
            ht.Add("CUSTOM_CAM_PRESETS", "预设");
            ht.Add("CUSTOM_CAM_PRESETS_DESC", "应用镜头预设");
            ht.Add("CUSTOM_CAM_PRESETS_TANDEM", "追走镜头 1.0");
            ht.Add("CUSTOM_CAM_PRESETS_TANDEM_DESC", "适用于漂移追走时使用的镜头");
            ht.Add("CUSTOM_CAM_PRESETS_FPV", "FPV 第一人称");
            ht.Add("CUSTOM_CAM_PRESETS_FPV_DESC", "适用于第一人称漂移时使用的镜头");
            ht.Add("CUSTOM_CAM_PRESETS_NFS", "NFS 镜头");
            ht.Add("CUSTOM_CAM_PRESETS_NFS_DESC", "街机游戏一样的画面体验");
            ht.Add("CUSTOM_CAM_PRESETS_MENU", "预设菜单");
            ht.Add("CUSTOM_CAM_PRESETS_MENU_DESC", "从这里开始");
            ht.Add("CUSTOM_CAM_SAVED", "保存的镜头");
            ht.Add("CUSTOM_CAM_SAVED_DESC", "用户创建的镜头");
            ht.Add("CUSTOM_CAM_SAVE_CURRENT", "保存当前镜头");
            ht.Add("CUSTOM_CAM_SAVE_CURRENT_DESC", "将当前的镜头设定储存为预设");
            ht.Add("CUSTOM_CAM_SPAWN", "应用镜头预设");
            ht.Add("CUSTOM_CAM_SPAWN_DESC", "应用此保存的镜头预设");
            ht.Add("CUSTOM_CAM_RENAME", "重命名预设");
            ht.Add("CUSTOM_CAM_RENAME_DESC", "重命名你已保存的镜头预设");
            ht.Add("CUSTOM_CAM_DELETE", "~r~删除预设");
            ht.Add("CUSTOM_CAM_DELETE_DESC", "~r~这将会删除此预设。警告：这是不可撤销的操作");

            // Drones Menu
            ht.Add("DRONE_MANAGE_TITLE", "管理无人机");
            ht.Add("DRONE_MANAGE_DESC", "管理已保存的无人机设定");
            ht.Add("DRONE_TITLE", "无人机");
            ht.Add("DRONE_DESC", "无人机镜头参数设定");
            ht.Add("DRONE_RACE", "穿梭机");
            ht.Add("DRONE_ZERO_G", "无重力");
            ht.Add("DRONE_SPECTATOR", "旁观者");
            ht.Add("DRONE_HOMING", "返航无人机");
            ht.Add("DRONE_INVERT_PITCH", "反向俯仰");
            ht.Add("DRONE_INVERT_PITCH_DESC", "调换俯仰控制的按键");
            ht.Add("DRONE_INVERT_ROLL", "反向滚动");
            ht.Add("DRONE_INVERT_ROLL_DESC", "调换滚动控制的按键");
            ht.Add("DRONE_GRAVITY", "重力倍数");
            ht.Add("DRONE_GRAVITY_DESC", "修改重力常数，更高的值会使无人机在自由落体运动中下降得更快");
            ht.Add("DRONE_TIMESTEP", "时间倍数");
            ht.Add("DRONE_TIMESTEP_DESC", "影响重力和无人机的响应能力");
            ht.Add("DRONE_DRAG", "阻力倍数");
            ht.Add("DRONE_DRAG_DESC", "设定空气阻力，越高的值会使无人机更快地失去速度");
            ht.Add("DRONE_ACCELE", "加速倍数");
            ht.Add("DRONE_ACCELE_DESC", "设定无人机的加速性能");
            ht.Add("DRONE_PITCH", "俯仰倍数");
            ht.Add("DRONE_PITCH_DESC", "设定无人机的俯仰响应速度 (pitch).");
            ht.Add("DRONE_ROLL", "滚动倍数");
            ht.Add("DRONE_ROLL_DESC", "设定无人机的滚动响应速度 (roll).");
            ht.Add("DRONE_YAW", "偏航倍数");
            ht.Add("DRONE_YAW_DESC", "设定无人机的偏航响应速度 (yaw).");
            ht.Add("DRONE_TILT", "倾斜角度");
            ht.Add("DRONE_TILT_DESC", "设定镜头相对于无人机的倾斜角度");
            ht.Add("DRONE_FOV", "FOV 视场");
            ht.Add("DRONE_FOV_DESC", "设定镜头的视场");
            ht.Add("DRONE_MAX_VELOCITY", "最高速度");
            ht.Add("DRONE_MAX_VELOCITY_DESC", "设定无人机的最高速度");
            ht.Add("DRONE_SAVED", "已保存的无人机");
            ht.Add("DRONE_SAVED_DESC", "用户创建的无人机");
            ht.Add("DRONE_SAVED_TITLE", "已保存的无人机");
            ht.Add("DRONE_SAVE_CURRENT", "保存当前无人机");
            ht.Add("DRONE_SAVE_CURRENT_DESC", "保存当前的无人机参数");
            ht.Add("DRONE_SPAWN", "应用无人机参数");
            ht.Add("DRONE_SPAWN_DESC", "应用此已保存的无人机参数");
            ht.Add("DRONE_RENAME", "重命名无人机");
            ht.Add("DRONE_RENAME_DESC", "重命名你已保存的无人机");
            ht.Add("DRONE_DELETE", "~r~删除无人机");
            ht.Add("DRONE_DELETE_DESC", "~r~这将会删除你已保存的无人机。警告：这是不可撤销的操作");
            ht.Add("DRONE_MODE", "模式");
            ht.Add("DRONE_MODE_DESC", "选择无人机的飞行模式\n" +
                                                                    "~r~Race 穿梭机~s~ - 普通，带有重力的无人机\n" +
                                                                    "~g~Zero-G 无重力~s~ - 没有重力的无人机，增加了减速\n" +
                                                                    "~b~Spectator 旁观者~s~ - 方便操作进行旁观\n" +
                                                                    "~y~Homing 返航机~s~ - 获取目标并将其作为中心点");
            return ht;
        }
    }
}
