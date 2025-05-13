using System.Collections.Generic;
using UnityEngine;

namespace Commons
{
    [CreateAssetMenu(menuName = "GameConfig", fileName = "GameConfig")]
    public class Config : ScriptableObject
    {
        #region codes
        public static Config current
        {
            get
            {
                if (s_current == null)
                {
                    s_current = CreateInstance<Config>();
                }
                return s_current;
            }
        }
        private static Config s_current;
        private void OnEnable()
        {
            s_current = this;
        }
        private void OnDisable()
        {
            if (ReferenceEquals(s_current, this))
            {
                s_current = null;
            }
        }
        #endregion

        [Range(1, 256)]
        public int pixelPerUnit = 100;

        public float scaled_pixel_per_unit { get; set; } = 100;

        public Vector2Int desiredResolution = new Vector2Int(1920, 1080);

        public float desiredPerspectiveFOV = 60;

        [Foldout("主界面")] public string main_menu_website_weibo = "https://weibo.com/u/1002143827";
        [Foldout("主界面")] public string main_menu_website_bilibili = "https://space.bilibili.com/423895";
        [Foldout("主界面")] public string main_menu_website_bug_report;

        [Foldout("YH 存档")]
        public string save_00_path = @"D:\save_00.dat";

        [Foldout("车体运动 - 车身旋转")] public float caravan_rotate_angle_limit = 60f;  // FT沿用
        [Foldout("车体运动 - 车身旋转")] public float caravan_rotation_damp_1 = 60f; // FT沿用
        [Foldout("车体运动 - 车身旋转")] public float caravan_rotation_damp_2 = 6f; // FT沿用

        [Foldout("YH 车体运动")] public float gravity = -10f;
        [Foldout("YH 车体运动")] public float acc_braking = -30f;
        [Foldout("YH 车体运动")] public float caravan_bounce_coefficient = 0.6f;
        [Foldout("YH 车体运动")] public float caravan_bounce_loss = 500f;

        [Foldout("主菜单界面 - 测试关卡入口")] public Vector2Int test_level_body_id;
        [Foldout("主菜单界面 - 测试关卡入口")] public uint test_level_world_id;
        [Foldout("主菜单界面 - 测试关卡入口")] public uint test_level_level_id;
        [Foldout("主菜单界面 - 测试关卡入口")] public bool show_test_level;

        [Foldout("场景切换/场景名UI")]
        public float scene_title_fade_out_time;
        [Foldout("场景切换/场景名UI")]
        public float scene_title_lasting_time;

        [Foldout("YH Game Init 初始化")] public string first_load_scene = "InitScene"; //首次加载scene
        [Foldout("YH Game Init 初始化")] public uint caravan_id = 100000000u;
        [Foldout("YH Game Init 初始化")] public uint init_wheel = 1210101u;
        [Foldout("YH Game Init 初始化")] public uint init_device_in_slot_top;
        [Foldout("YH Game Init 初始化")] public uint init_device_in_slot_front_top;
        [Foldout("YH Game Init 初始化")] public uint init_device_in_slot_back_top;
        [Foldout("YH Game Init 初始化")] public uint init_device_in_slot_front;
        [Foldout("YH Game Init 初始化")] public uint init_device_in_slot_back;
        [Foldout("YH Game Init 初始化")] public List<int> init_roles;

        [Foldout("YH Backpack 背包")] public float per_overweight_slot_weight = 300;
        [Foldout("YH Backpack 背包")] public int basic_backpack_slot_num = 5;

        [Foldout("YH Camera 镜头运动")] public float travel_scene_camera_offset_x;  // FT沿用
        [Foldout("YH Camera 镜头运动")] public float cam_pos_offset_y;
        [Foldout("YH Camera 镜头运动")] public float cam_pos_offset_z;
        [Foldout("YH Camera 镜头运动")] public bool free_camera;

        [Foldout("YH Encounter 奇遇事件")] public float trigger_length = 25;
        [Foldout("YH Encounter 奇遇事件")] public float notice_length_1 = 50f;
        [Foldout("YH Encounter 奇遇事件")] public float notice_length_2 = 10f;
        [Foldout("YH Encounter 奇遇事件")] public float event_btn_car_vx_limit = 0.1f;
        [Foldout("YH Encounter 奇遇事件")] public int event_btn_car_parking_ticks = 60;

        [Foldout("YH Hookrope 钩索")] public float rope_max_length;
        [Foldout("YH Hookrope 钩索")] public float rope_min_length;
        [Foldout("YH Hookrope 钩索")] public float rope_elasticity;

        [Foldout("YH Loot 掉落物")] public float loot_pickup_distance = 0.1f;
        [Foldout("YH Loot 掉落物")] public float loot_attraction_power = 20f;
        [Foldout("YH Loot 掉落物")] public float coin_surface_bounce_coef = 1f;
        [Foldout("YH Loot 掉落物")] public float caravan_vel2_screen_bounce = 0.5f;

        [Foldout("YH Monster 怪物")] public float monster_grip = 35f;
        [Foldout("YH Monster 怪物")] public float fling_off_dis = 40f;

        [Foldout("YH Monster_Tide 兽潮")] public int time_pressure_min = 15;
        [Foldout("YH Monster_Tide 兽潮")] public int time_pressure_max = 60;
        [Foldout("YH Monster_Tide 兽潮")] public int pressure_kill_score = 40;

        [Foldout("YH Power_Lever 动力拉杆")] public float lever_up_speed = 1.2f;
        [Foldout("YH Power_Lever 动力拉杆")] public float lever_down_speed = -0.8f;
        [Foldout("YH Power_Lever 动力拉杆")] public float lever_move_delta = 0.01f;

        [Foldout("YH Projectile 飞射物")] public float bullet_bounce_threshold_ekmin = 100f;
        [Foldout("YH Projectile 飞射物")] public float bullet_surface_bounce_coef = 0.5f;
        [Foldout("YH Projectile 飞射物")] public float bullet_bounce_threshold_vmax = 20f;
        [Foldout("YH Projectile 飞射物")] public float surface_friction = 0.3f;
        [Foldout("YH Projectile 飞射物")] public float arrow_penetration_loss = 10f;
        [Foldout("YH Projectile 飞射物")] public float bullet_penetration_loss = 6f;
        [Foldout("YH Projectile 飞射物")] public float bullet_enemy_bounce_coef = 0.2f;

        [Foldout("YH Repair 维修")] public int repairing_counts = 3;
        [Foldout("YH Repair 维修")] public int repairing_cd_ticks = 720;

        [Foldout("YH SFX 音效")] public string SE_target_locked;
        [Foldout("YH SFX 音效")] public string SE_fix;
        [Foldout("YH SFX 音效")] public string SE_monster_die;

        [Foldout("YH VFX_Device_Damaged 设备损毁特效")] public string devicedestroy_vfx;
        [Foldout("YH VFX_Device_Damaged 设备损毁特效")] public string devicesmoke_vfx;

        [Foldout("YH Work 工作")] public int normal_work_ability;
        [Foldout("YH Work 工作")] public float fix_device_job_effect = 0.05f;
        [Foldout("YH Work 工作")] public float fix_caravan_job_effect = 0.02f;

        [Foldout("YH测试")] public bool is_load_devices = true;
        [Foldout("YH测试")] public bool is_load_enemys = true;
        [Foldout("YH测试")] public uint test_level_id = 4001;

        [Foldout("YH安全区")] public float security_zone_distance;
        [Foldout("YH安全区")] public uint food_id_1;
        [Foldout("YH安全区")] public uint food_id_2;
        [Foldout("YH安全区")] public uint food_id_3;
        [Foldout("YH安全区")] public uint food_id_4;
        [Foldout("YH安全区")] public uint coin_id;

        [Foldout("YH大本营")] public Vector2 camp_camera_size_range = new(3f, 25f);
        [Foldout("YH大本营")] public float camp_camera_zoom_speed = 0.2f;

        #region const
        //帧率
        public const int PHYSICS_TICKS_PER_SECOND = 120;
        public const float PHYSICS_TICK_DELTA_TIME = 1f / PHYSICS_TICKS_PER_SECOND;
        #endregion

        #region internal_setting
        //Mgr tick优先级
        public const int CaravanMgr_Priority = 1;
        public const int DeviceMgr_Priority = 2;
        public const int EnemyMgr_Priority = 3;
        public const int New_EnemyMgr_Priority = 3;
        public const int EnvironmentMgr_Priority = 4;
        public const int MapMgr_Priority = 5;
        public const int CardMgr_Priority = 6;
        public const int ProjectileMgr_Priority = 7;
        public const int ProjectileMgr_Enemy_Priority = 8;
        public const int SisterMgr_Priority = 9;
        public const int BrotherMgr_Priority = 10;
        public const int Base_SlotMgr_Priority = 11;
        public const int Vfx_Priority = 99;

        public const int BuffMgr_Priority = 0;

        public const int CoreMgr_Priority = 20;

        //Mgr注册name(tick)
        public const string PlayerMgr_Name = "Player";
        public const string AdventureMgr_Name = "Adventure";
        public const string ProjectileMgr_Name = "ProjectileMgr";
        public const string GarageMgr_Name = "GarageMgr";
        public const string CaravanMgr_Name = "CaravanMgr";
        public const string DeviceMgr_Name = "DeviceMgr";
        public const string CoreMgr_Name = "CoreMgr";
        public const string EnemyMgr_Name = "EnemyMgr";
        public const string New_EnemyMgr_Name = "New_EnemyMgr";
        public const string BuffMgr_Name = "BuffMgr";
        public const string WorkMgr_Name = "WorkMgr";
        public const string CharacterMgr_Name = "CharacterMgr";
        public const string RelicMgr_Name = "RelicMgr";

        public const string EnvironmentMgr_Name = "enviroment";
        public const string CardMgr_Name = "CardMgr";
        public const string WareHouseMgr_Name = "WareHouse";
        public const string MapMgr_Name = "MapMgr";
        public const string ProjectileMgr_Enemy_Name = "ProjectileMgr_Enemy";
        public const string SisterMgr_Name = "SisterMgr";
        public const string BrotherMgr_Name = "BrotherMgr";
        public const string BgmMgr_Name = "BgmMgr";
        public const string HoldingDeviceMgr_Name = "HoldingDeviceMgr";
        public const string Vfx_Name = "Vfx";

        //Mgr注册name(普通)
        public const string RewardHouse_Name = "RewardHouse";
        public const string Elite_Battle_TimerMgr_Name = "Elite_Battle_TimerMgr";
        public const string SelectLevelMgr_Name = "SelectLevel";

        #endregion
    }

}
