using AutoCodes;
using Commons;
using Foundations;
using UnityEngine;
using World.Audio;
using World.Enemys;
using World.Projectiles;
using static World.WorldEnum;

namespace World.Devices.Device_AI
{
    public class NewBasicShooter : Device, IAttack, ILoad
    {
        #region CONST
        private const string ANIM_IDLE = "idle";
        private const string ANIM_SHOOT = "shoot";
        private const string ANIM_BROKEN = "idle";
        protected const string BONE_FOR_ROTATE = "roll_control";
        private const float SHOOT_ERROR_DEGREE = 5F;
        #endregion

        protected enum NewDevice_FSM_Shooter
        {
            idle,
            shoot,
            reloading,
            broken,
        }
        protected NewDevice_FSM_Shooter fsm;

        private bool can_blaze; //能否连续射击
        protected bool can_shoot_check_post_cast = true;  //判定射击后摇是否结束；

        protected bool can_shoot_check_ammo => Current_Ammo >= 1;
        protected bool can_shoot_check_cd => Current_Interval <= 0;

        private bool another_ammo_reloaded = true;
        private float manual_reload_reward, manual_reload_punishment;
        private float manual_reload_process_threshold;

        // Virtual for Derived Mod Shooting Data
        protected virtual float shoot_deg_offset { get; set; } = 0f;  //子弹出射角度相对枪口骨骼的角度偏移
        protected virtual float ammo_velocity_mod { get; set; } = 1f;  //子弹出射速度修正

        // Public for UI Panel
        public bool can_manual_reload => another_ammo_reloaded && Reloading_Process >= manual_reload_process_threshold;
        public int reload_stage_max;
        public int reload_stage_current;
        public bool reload_by_stage => reload_stage_max > 0;
        public bool reloading => fsm == NewDevice_FSM_Shooter.reloading;
        public void UI_Controlled_Reloading() { if (Current_Ammo < Max_Ammo) FSM_change_to(NewDevice_FSM_Shooter.reloading); }
        public virtual void UI_Controlled_Shooting() { if (can_manual_shoot()) FSM_change_to(NewDevice_FSM_Shooter.shoot); }

        #region Load Interface
        public int Max_Ammo { get; set; }
        public int Current_Ammo { get; set; }
        public float Reloading_Process { get; set; }
        public float Reload_Speed { get; set; }
        #endregion

        #region Shoot Interface
        public float Damage_Increase { get; set; }
        public float Knockback_Increase { get; set; }
        public int Attack_Interval { get; set; }
        public int Current_Interval { get; set; }
        #endregion

        #region SFX
        //private string se_fire;
        private string se_reloaded;
        #endregion

        //============================================================================================================

        public override void InitData(device_all rc)
        {
            module_list.Clear();    //之后可能考虑统一到卸载里面

            DeviceModule deviceModule = new DeviceModule();
            var fb = new FireBehaviour() { module = deviceModule };
            var lb = new LoadBehaviour() { module = deviceModule };
            deviceModule.db_list.Add(fb);
            deviceModule.db_list.Add(lb);
            deviceModule.device = this;
            module_list.Add(deviceModule);

            fire_logics.TryGetValue(rc.fire_logic.ToString(), out var record);

            Max_Ammo = (int)record.capacity;
            Current_Ammo = Max_Ammo;
            Reload_Speed = record.reload_speed;
            can_blaze = record.can_blaze;

            reload_stage_max = record.reload_by_stage;

            Attack_Interval = record.cd;
            Current_Interval = 0;

            manual_reload_reward = record.reload_manual_reward.Item1;
            manual_reload_punishment = record.reload_manual_reward.Item2;
            manual_reload_process_threshold = record.reload_manual_process;

            se_reloaded = record.SE_reloaded;

            bones_direction.Clear();
            bones_direction.Add(BONE_FOR_ROTATE, Vector2.right);

            base.InitData(rc);

            #region Animation Event
            var shoot = new AnimEvent()
            {
                anim_name = ANIM_SHOOT,
                percent = record.tick_percent,
                anim_event = (Device d) =>
                {
                    single_shoot(record, ammo_velocity_mod);
                    Current_Ammo--;
                }
            };
            var end_post_cast = new AnimEvent()
            {
                anim_name = ANIM_SHOOT,
                percent = record.rapid_fire_tick_percent,
                anim_event = (Device d) =>
                {
                    can_shoot_check_post_cast = true;
                    Current_Interval = Attack_Interval;
                }
            };
            var back_to_idle = new AnimEvent()
            {
                anim_name = ANIM_SHOOT,
                percent = 1f,
                anim_event = (Device d) => FSM_change_to(NewDevice_FSM_Shooter.idle)
            };

            anim_events.Add(shoot);
            anim_events.Add(end_post_cast);
            anim_events.Add(back_to_idle);
            #endregion
        }

        public override void Start()
        {
            base.Start();
            FSM_change_to(NewDevice_FSM_Shooter.idle);
        }
        private void single_shoot(fire_logic record, float ammo_velocity_mod)
        {
            Mission.instance.try_get_mgr(Config.ProjectileMgr_Name, out ProjectileMgr pmgr);

            var salvo = record.salvo + BattleContext.instance.projectile_salvo_amount;

            AudioSystem.instance.PlayOneShot(record.SE_fire);

            for (int i = 0; i < salvo; i++)
            {
                var angle = 2 * record.angle;
                var ave_a = angle / salvo;
                var angle_1 = -record.angle + (salvo - i - 1) * ave_a;
                var angle_2 = record.angle - i * ave_a;

                float speed;
                float init_speed;
                if (record.speed.Item2 == 0)
                {
                    speed = record.speed.Item1 * ammo_velocity_mod;
                    init_speed = record.speed.Item1;
                }
                else
                {
                    speed = Random.Range(record.speed.Item1, record.speed.Item2) * ammo_velocity_mod;
                    init_speed = (record.speed.Item1 + record.speed.Item2) * 0.5f;
                }

                var plt = record.projectile_life_ticks.Item2 == 0 ? record.projectile_life_ticks.Item1 : Random.Range(record.projectile_life_ticks.Item1, record.projectile_life_ticks.Item2);

                projectiles.TryGetValue(record.projectile_id.ToString(), out var projectile_record);
                var p = new Projectile();
                float rot_speed = projectile_record.inertia_moment > 0 ? projectile_record.mass * init_speed / projectile_record.inertia_moment : 0;
                rot_speed *= Random.Range(-1f, 1f);

                switch (projectile_record.ammo_type)
                {
                    case "Bullet":
                        p = new BulletProjectile();
                        break;
                    case "Arrow":
                        p = new ArrowProjectile();
                        break;
                    default:
                        break;
                }

                var shoot_bone_dir = bones_direction[BONE_FOR_ROTATE];
                var shoot_dir = Quaternion.AngleAxis((shoot_bone_dir.x >= 0 ? 1 : -1) * shoot_deg_offset, Vector3.forward) * shoot_bone_dir;

                //此处的attack_data先只考虑设备层面的增幅 全局的先不考虑（因为没做），实际触发伤害结算的时候再考虑 全局附加暴击
                Attack_Data attack_data = new()
                {
                    atk = (int)(record.damage * (1 + Damage_Increase)),
                    critical_chance = record.critical_chance,
                    critical_rate = record.critical_dmg_rate,
                    ignite = record.ignite,
                };


                p.Init(projectile_record, shoot_dir, key_points[record.bone_name].position, (this as ITarget).velocity, angle_1, angle_2, speed, init_speed, Faction.player, plt, this, attack_data, rot_speed);
                pmgr.AddProjectile(p);
            }
        }



        public override void tick()
        {
            if (!is_validate && fsm != NewDevice_FSM_Shooter.broken)
                FSM_change_to(NewDevice_FSM_Shooter.broken);

            switch (fsm)
            {
                case NewDevice_FSM_Shooter.idle:
                    if (Current_Interval > 0)
                        Current_Interval--;

                    //if (Current_Ammo <= 0)
                    //    FSM_change_to(NewDevice_FSM_Shooter.reloading);

                    if (target_list.Count != 0)
                    {
                        rotate_bone_to_target(BONE_FOR_ROTATE);
                        if (can_auto_shoot())
                            FSM_change_to(NewDevice_FSM_Shooter.shoot);         //有目标就切换到射击模式
                    }
                    break;
                case NewDevice_FSM_Shooter.shoot:
                    if (Current_Interval > 0)
                        Current_Interval--;
                    rotate_bone_to_target(BONE_FOR_ROTATE);

                    if (can_auto_shoot())
                        FSM_change_to(NewDevice_FSM_Shooter.shoot);
                    break;
                case NewDevice_FSM_Shooter.reloading:
                    ammo_reloading();
                    if (Current_Ammo < Max_Ammo)
                        break;
                    AudioSystem.instance.PlayOneShot(se_reloaded);
                    FSM_change_to(NewDevice_FSM_Shooter.idle);
                    break;
                case NewDevice_FSM_Shooter.broken:
                    if (is_validate)
                        FSM_change_to(NewDevice_FSM_Shooter.idle);
                    break;
                default:
                    break;
            }

            base.tick();
        }

        protected void FSM_change_to(NewDevice_FSM_Shooter target_fsm)
        {
            fsm = target_fsm;
            switch (fsm)
            {
                case NewDevice_FSM_Shooter.idle:
                    ChangeAnim(ANIM_IDLE, true);
                    rotate_speed = desc.rotate_speed.Item1;
                    break;
                case NewDevice_FSM_Shooter.shoot:
                    can_shoot_check_post_cast = false;
                    ChangeAnim(ANIM_SHOOT, false);
                    rotate_speed = desc.rotate_speed.Item2;
                    break;
                case NewDevice_FSM_Shooter.reloading:
                    ChangeAnim(ANIM_IDLE, true);
                    rotate_speed = desc.rotate_speed.Item1;
                    reload_stage_current = 0;
                    Reloading_Process = 0;
                    another_ammo_reloaded = true;
                    can_shoot_check_post_cast = true; //reset post_cast check when reloading
                    break;
                case NewDevice_FSM_Shooter.broken:
                    ChangeAnim(ANIM_BROKEN, true);
                    rotate_speed = 0;
                    Reloading_Process = 0;
                    another_ammo_reloaded = true;
                    can_shoot_check_post_cast = true;  //reset post_cast check when broken
                    break;
                default:
                    break;
            }
        }
        //------------------------------------------------------------------------------------------------------------
        void IAttack.TryToAutoShoot()
        {
            if (player_oper) //有玩家操控时 以玩家操控优先
                return;

            switch (fsm)
            {
                case NewDevice_FSM_Shooter.idle:
                    if (target_list.Count == 0)
                    {
                        try_get_target();
                    }
                    break;
                case NewDevice_FSM_Shooter.shoot:
                    //Current_Interval--;

                    if (target_list.Count == 0)
                    {
                        try_get_target();
                    }

                    /*rotate_bone_to_target(BONE_FOR_ROTATE);

                    if (can_auto_shoot())
                        FSM_change_to(NewDevice_FSM_Shooter.shoot);*/
                    break;

                default:
                    break;
            }
        }

        private int auto_load_delay_tick = Random.Range(10, 50);
        private int auto_load_over_tick;
        void ILoad.TryToAutoLoad()
        {
            if (player_oper) //有玩家操控时 以玩家操控优先
                return;

            if (Current_Ammo <= 0 && fsm == NewDevice_FSM_Shooter.idle)     //没子弹 且射击结束
                FSM_change_to(NewDevice_FSM_Shooter.reloading);

            /*    if (fsm == NewDevice_FSM_Shooter.reloading)
            {
                if (can_manual_reload)
                    auto_load_over_tick++;

                if (auto_load_over_tick > auto_load_delay_tick)
                {
                    manual_reload_success();
                    auto_load_delay_tick = Random.Range(10, 50);
                    auto_load_over_tick = 0;
                }
            }*/
        }
        //------------------------------------------------------------------------------------------------------------
        protected virtual bool can_auto_shoot()
        {
            return can_shoot_check_cd && can_shoot_check_error_angle() && can_shoot_check_ammo && can_shoot_check_post_cast;
        }
        protected virtual bool can_manual_shoot()
        {
            return can_shoot_check_cd && can_shoot_check_ammo && can_shoot_check_post_cast;
        }

        protected bool can_shoot_check_error_angle()
        {
            if (target_list.Count == 0)
                return false;

            Vector2 target_v2 = BattleUtility.get_v2_to_target_collider_pos(target_list[0], position);
            var current_v2 = bones_direction[BONE_FOR_ROTATE];
            var delta_deg = Mathf.Abs(Vector2.SignedAngle(current_v2, target_v2));
            return delta_deg <= SHOOT_ERROR_DEGREE;
        }

        protected override bool try_get_target()
        {
            var target = BattleUtility.select_target_in_circle_min_angle(position, bones_direction[BONE_FOR_ROTATE], desc.basic_range.Item2, faction, (ITarget t) =>
            {
                return target_can_be_selected(t) && !target_list.Contains(t) && !outrange_targets.ContainsKey(t);       //目标可选，且不在已选列表中也不在超出射程列表中
            });

            if (target != null)
            {
                if (target is Enemy enemy)
                    enemy.Select(true);
                target_list.Add(target);
            }

            return target != null;
        }

        private void ammo_reloading()
        {
            Reloading_Process += Reload_Speed;

            if (Reloading_Process < 1)
                return;

            Reloading_Process--;
            if (reload_by_stage)
            {
                if (++reload_stage_current >= reload_stage_max)
                    Current_Ammo = Max_Ammo;
            }
            else
            {
                Current_Ammo++;
            }
            another_ammo_reloaded = true;
        }
        private void manual_reload_success()
        {
            Reloading_Process += manual_reload_reward * Random.Range(1f, 2f);
            Reloading_Process = Mathf.Min(Reloading_Process, 1);
            another_ammo_reloaded = false;
        }
        private void manual_reload_failure()
        {
            Reloading_Process += manual_reload_punishment;
            Reloading_Process = Mathf.Max(Reloading_Process, 0);
        }

        #region PlayerControl
        public override void StartControl()
        {
            InputController.instance.r_event += Load;

            InputController.instance.left_hold_event += TryToFire;
            InputController.instance.left_release_event += ResetFire;

            base.StartControl();
        }

        public override void EndControl()
        {
            InputController.instance.r_event -= Load;

            InputController.instance.RemoveLeftHold(TryToFire);
            InputController.instance.left_release_event -= ResetFire;

            base.EndControl();
        }

        virtual protected void Aiming()
        {
            var dir = InputController.instance.GetWorlMousePosition() - new Vector3(position.x, position.y, 10);
            rotate_bone_to_dir(BONE_FOR_ROTATE, dir);
        }

        public override void OperateClick()
        {
            if (can_manual_shoot())
            {
                if (fsm == NewDevice_FSM_Shooter.idle || fsm == NewDevice_FSM_Shooter.shoot)
                    FSM_change_to(NewDevice_FSM_Shooter.shoot);
            }
        }

        public override void OperateDrag(Vector2 dir)
        {
            rotate_bone_to_dir(BONE_FOR_ROTATE, -dir);
        }

        private bool fired = false;

        private void ResetFire()
        {
            fired = false;
        }
        private void TryToFire()
        {
            if (!fired)
            {
                if (Fire())
                    fired = true;
            }
            else if (can_blaze)
            {
                Fire();
            }
        }
        private bool Fire()
        {
            if (can_manual_shoot())
                if (fsm == NewDevice_FSM_Shooter.idle || fsm == NewDevice_FSM_Shooter.shoot)
                {
                    FSM_change_to(NewDevice_FSM_Shooter.shoot);
                    return true;
                }
            return false;
        }

        private void Load()
        {
            if (fsm == NewDevice_FSM_Shooter.idle)
            {
                Current_Ammo = 0;
            }

            if (fsm == NewDevice_FSM_Shooter.reloading)
            {
                if (can_manual_reload)
                    manual_reload_success();
                else
                    manual_reload_failure();
            }
        }
        #endregion
    }
}
