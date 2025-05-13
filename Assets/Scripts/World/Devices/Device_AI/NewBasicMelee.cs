using AutoCodes;
using Commons;
using UnityEngine;
using World.Audio;
using World.Enemys;

namespace World.Devices.Device_AI
{
    public class NewBasicMelee : Device, IAttack, ISharp
    {
        #region CONST
        private const float MOVE_SPEED_SLOW_COEF = 0.5F;

        private const string BONE_FOR_ROTATION = "roll_control";
        private const float ATK_ERROR_DEGREE = 5F;

        private const string ANIM_IDLE = "idle";
        private const string ANIM_ATTACK_1 = "attack_1";
        private const string ANIM_ATTACK_2 = "attack_2";
        private const string ANIM_BROKEN = "idle";
        private const string COLLIDER_1 = "collider_1";
        private const string KEY_POINT_1 = "collider_1";



        #endregion

        private enum Device_FSM_Melee
        {
            idle,
            sharping,
            attacking,
            broken,
        }
        private Device_FSM_Melee fsm;

        private int melee_atk_dmg;
        private float basic_knockback;

        private float hit_box_offset;
        private float distance_can_attack;

        private bool can_move = true;
        public float move_speed;
        private float move_speed_fast;
        private float move_speed_slow;

        private float rotation_speed_fast;
        private float rotation_speed_slow;

        public float attack_factor = 1f;

        public Vector2 sub_position = Vector2.zero;

        private bool can_blaze;  //按住鼠标可以连续攻击
        protected bool can_attack_check_post_cast = true;  //判定攻击后摇是否结束；
        protected bool can_attack_check_cd => Current_Interval <= 0;

        public bool Sharping => fsm == Device_FSM_Melee.sharping;

        public float Damage_Increase { get; set; }
        public float Knockback_Increase { get; set; }
        public int Attack_Interval { get; set; }
        public int Current_Interval { get; set; }
        public float Sharpness_Current { get; set; }
        public float Sharpness_Min { get; set; }
        public float Sharpness_Loss { get; set; }
        public float Sharpness_Recover { get; set; }

        public override void InitData(device_all rc)
        {
            module_list.Clear();    //之后可能考虑统一到卸载里面

            DeviceModule deviceModule = new DeviceModule();
            deviceModule.db_list.Add(new FireBehaviour()
            {
                module = deviceModule,
            });
            deviceModule.db_list.Add(new SharpBehaviour()
            {
                module = deviceModule,
            });

            deviceModule.device = this;
            module_list.Add(deviceModule);

            device_type = DeviceType.melee;

            rotation_speed_fast = rc.rotate_speed.Item1;
            rotation_speed_slow = rc.rotate_speed.Item2;

            melee_logics.TryGetValue(rc.melee_logic.ToString(), out var logic);
            melee_atk_dmg = logic.damage;
            can_blaze = logic.can_blaze;
            basic_knockback = logic.knockback_ft;
            hit_box_offset = logic.hit_box_offset;
            distance_can_attack = hit_box_offset + rc.basic_range.Item2;
            move_speed_fast = logic.atk_part_speed;
            move_speed_slow = logic.atk_part_speed * MOVE_SPEED_SLOW_COEF;
            Current_Interval = logic.cd;
            Attack_Interval = logic.cd;

            Sharpness_Current = 100;
            Sharpness_Min = logic.sharpness_min;
            Sharpness_Loss = logic.sharpness_loss;
            Sharpness_Recover = logic.sharpness_recover;

            bones_direction.Clear();
            bones_direction.Add(BONE_FOR_ROTATION, Vector2.up);

            base.InitData(rc);

            var anim_1_col_turn_on_time = logic.hit_period.Item1;
            var anim_1_col_turn_off_time = logic.hit_period.Item2;
            var anim_2_col_turn_on_time = logic.hit_period_2 == null ? anim_1_col_turn_on_time : logic.hit_period_2.Value.Item1;
            var anim_2_col_turn_off_time = logic.hit_period_2 == null ? anim_1_col_turn_off_time : logic.hit_period_2.Value.Item2;

            #region Anim_Event_Attack_1
            anim_events.Add(new AnimEvent()
            {
                anim_name = ANIM_ATTACK_1,
                percent = anim_1_col_turn_on_time,
                anim_event = (Device d) => open_collider(d)
            });
            anim_events.Add(new AnimEvent()
            {
                anim_name = ANIM_ATTACK_1,
                percent = anim_1_col_turn_off_time,
                anim_event = (Device d) =>
                {
                    d.CloseCollider(COLLIDER_1);
                    can_move = true;
                    can_attack_check_post_cast = true;
                }
            });
            anim_events.Add(new AnimEvent()
            {
                anim_name = ANIM_ATTACK_1,
                percent = 1,
                anim_event = (Device d) => FSM_change_to(Device_FSM_Melee.idle)
            });
            #endregion

            #region Anim_Event_Attack_2
            anim_events.Add(new AnimEvent()
            {
                anim_name = ANIM_ATTACK_2,
                percent = anim_2_col_turn_on_time,
                anim_event = (Device d) => open_collider(d)
            });
            anim_events.Add(new AnimEvent()
            {
                anim_name = ANIM_ATTACK_2,
                percent = anim_2_col_turn_off_time,
                anim_event = (Device d) =>
                {
                    d.CloseCollider(COLLIDER_1);
                    can_move = true;
                    can_attack_check_post_cast = true;
                }
            });
            anim_events.Add(new AnimEvent()
            {
                anim_name = ANIM_ATTACK_2,
                percent = 1,
                anim_event = (Device d) => FSM_change_to(Device_FSM_Melee.idle)
            });

            #endregion

            #region Attack_Subfunc
            void open_collider(Device d)
            {
                AudioSystem.instance.PlayOneShot(logic.SE_attack_begin);

                d.OpenCollider(COLLIDER_1, (ITarget t) =>
                {
                    int final_dmg = (int)(melee_atk_dmg * (1 + Damage_Increase) * Sharpness_Current * 0.01f);
                    float final_ft = basic_knockback * (1 + Knockback_Increase);
                    Attack_Data attack_data = new()
                    {
                        atk = final_dmg,
                        critical_chance = logic.critical_chance + BattleContext.instance.critical_chance_delta,
                        critical_rate = logic.critical_dmg_rate + BattleContext.instance.global_critical_rate,
                    };

                    t.hurt(this, attack_data, out var dmg_data);
                    BattleContext.instance.ChangeDmg(this, dmg_data.dmg);

                    if (t.hp <= 0)
                    {
                        kill_enemy_action?.Invoke(t);
                    }

                    t.impact(WorldEnum.impact_source_type.melee, (Vector2)key_points[KEY_POINT_1].position, BattleUtility.get_target_colllider_pos(t), final_ft);

                    Sharpness_Current = Mathf.Max(Sharpness_Current - Sharpness_Loss, Sharpness_Min);
                });
                can_move = false;
            }
            #endregion
        }

        public override void Start()
        {
            base.Start();
            FSM_change_to(Device_FSM_Melee.idle);
        }

        // ========================================================================================

        protected virtual bool can_auto_attack()
        {
            return can_attack_check_cd && can_attack_check_post_cast && check_error_angle() && target_list.Count > 0;
        }

        protected virtual bool can_attack_check_distance()
        {
            return BattleUtility.get_v2_to_target_collider_pos(target_list[0], sub_position).magnitude <= distance_can_attack;
        }

        public override void tick()
        {
            if (WorldContext.instance.is_need_reset)
                sub_position -= new Vector2(WorldContext.instance.reset_dis, 0);

            if (!is_validate && fsm != Device_FSM_Melee.broken)       //坏了
            {
                FSM_change_to(Device_FSM_Melee.broken);
                can_move = true;
            }

            sub_position = position;

            Current_Interval--;

            switch (fsm)
            {
                case Device_FSM_Melee.idle:
                    if (target_list.Count == 0)
                        try_get_target();

                    if (target_list.Count == 0)
                        melee_device_rotate_to(Vector2.up, rotation_speed_fast);
                    else
                        FindTarget_Attack();

                    break;
                case Device_FSM_Melee.sharping:
                    melee_device_rotate_to(new Vector2(-1f, -1f), rotation_speed_fast);
                    break;
                case Device_FSM_Melee.attacking:
                    break;
                case Device_FSM_Melee.broken:
                    target_list.Clear();

                    if (is_validate)
                        FSM_change_to(Device_FSM_Melee.idle);
                    break;

                default:
                    break;
            }
            base.tick();
        }

        private void FSM_change_to(Device_FSM_Melee target_fsm)
        {
            fsm = target_fsm;
            switch (target_fsm)
            {
                case Device_FSM_Melee.idle:
                    ChangeAnim(ANIM_IDLE, true);
                    move_speed = move_speed_fast;
                    can_attack_check_post_cast = true;
                    break;
                case Device_FSM_Melee.sharping:
                    ChangeAnim(ANIM_IDLE, true);
                    move_speed = move_speed_fast;
                    can_attack_check_post_cast = true;
                    break;
                case Device_FSM_Melee.attacking:
                    ChangeAnim(Random.Range(0, 2) == 0 ? ANIM_ATTACK_1 : ANIM_ATTACK_2, false);
                    move_speed = move_speed_slow;
                    can_attack_check_post_cast = false;
                    break;
                case Device_FSM_Melee.broken:
                    ChangeAnim(ANIM_BROKEN, true);
                    move_speed = move_speed_slow;
                    can_attack_check_post_cast = true;
                    break;
                default:
                    break;
            }
        }

        // ----------------------------------------------------------------------------------------

        public override void InitPos()
        {
            sub_position = position;

            base.InitPos();
        }

        public void ChangeFactor(float f)
        {
            attack_factor = f;
            Current_Interval = (int)(Current_Interval / attack_factor);

            foreach (var view in views)
            {
                view.notify_change_anim_speed(attack_factor);
            }
        }

        protected override bool target_in_radius(ITarget t)
        {
            var tp = BattleUtility.get_target_colllider_pos(t);
            if (WorldContext.instance.is_need_reset)
            {
                tp -= new Vector2(WorldContext.instance.reset_dis, 0);
            }
            var t_distance = (tp - position).magnitude;
            return t_distance <= desc.basic_range.Item2;
        }

        protected override bool try_get_target()
        {
            var target = try_select_nearest_target();

            if (target != null)
            {
                if (target is Enemy enemy)
                    enemy.Select(true);
                target_list.Add(target);
            }

            // 这里即使返回null也是可以接受的，无需排除此结果
            return target != null;

            ITarget try_select_nearest_target()
            {
                // 以槽位位置为中心寻找半径内的目标，返回其中最近的目标。最近 = 到自身当前位置的距离最近
                var ts = BattleUtility.select_all_target_in_circle(position, desc.basic_range.Item2, faction, (ITarget ts) =>
                {
                    return target_can_be_selected(ts) && !target_list.Contains(ts) && !outrange_targets.ContainsKey(ts);
                });

                if (ts == null || ts.Count == 0)
                    return null;

                ts.Sort((ITarget t1, ITarget t2) =>
                {
                    if (BattleUtility.get_v2_to_target_collider_pos(t1, sub_position).magnitude > BattleUtility.get_v2_to_target_collider_pos(t2, sub_position).magnitude)
                        return 1;
                    else if (BattleUtility.get_v2_to_target_collider_pos(t1, sub_position).magnitude == BattleUtility.get_v2_to_target_collider_pos(t2, sub_position).magnitude)
                        return 0;
                    return -1;
                });
                return ts[0];
            }
        }

        private void melee_device_rotate_to(Vector2 expected_dir, float rotation_speed)
        {
            bones_direction[BONE_FOR_ROTATION] = BattleUtility.rotate_v2(bones_direction[BONE_FOR_ROTATION], expected_dir, rotation_speed);
        }

        private void melee_device_move_to(Vector2 expected_pos)
        {
            if (can_move)
            {
                var distance = expected_pos - sub_position;
                var move_distance_per_tick = move_speed * Config.PHYSICS_TICK_DELTA_TIME;
                if (distance.magnitude > move_distance_per_tick)
                    expected_pos = sub_position + distance.normalized * move_distance_per_tick;

                var t_distance = expected_pos - position;
                if (t_distance.magnitude > 3f)                   //此处3为近战设备的移动范围（随便填的)
                    expected_pos = (expected_pos - position).normalized * 3f + position;

                sub_position = expected_pos;
            }
        }

        protected bool check_error_angle()
        {
            if (target_list.Count == 0)
                return false;

            Vector2 target_v2 = BattleUtility.get_v2_to_target_collider_pos(target_list[0], position);
            var current_v2 = bones_direction[BONE_FOR_ROTATION];
            var delta_deg = Mathf.Abs(Vector2.SignedAngle(current_v2, target_v2));
            return delta_deg <= ATK_ERROR_DEGREE;
        }

        void IAttack.TryToAutoShoot()
        {
            if (player_oper || fsm == Device_FSM_Melee.broken)
                return;
            if (fsm == Device_FSM_Melee.idle || fsm == Device_FSM_Melee.attacking)
                FindTarget_Attack();
        }

        private void FindTarget_Attack()
        {
            if (target_list.Count == 0)
                try_get_target();

            Vector2 expected_pos_attacking;
            if (target_list.Count == 0)
            {
                expected_pos_attacking = sub_position;
                melee_device_rotate_to(Vector2.up, Current_Interval <= 0 ? rotation_speed_fast : rotation_speed_slow);
            }
            else
            {
                var delta_pos = BattleUtility.get_v2_to_target_collider_pos(target_list[0], position);
                expected_pos_attacking = position
                            + delta_pos.normalized
                            * Mathf.Min(desc.basic_range.Item2, delta_pos.magnitude - hit_box_offset);
                melee_device_rotate_to(delta_pos.normalized, Current_Interval <= 0 ? rotation_speed_fast : rotation_speed_slow);

                if (Current_Interval <= 0 && (BattleUtility.get_v2_to_target_collider_pos(target_list[0], sub_position)).magnitude <= distance_can_attack && check_error_angle())
                {
                    Current_Interval = Attack_Interval;
                    FSM_change_to(Device_FSM_Melee.attacking);
                }
            }
        }

        public override void Disable()
        {
            CloseCollider(COLLIDER_1);
            base.Disable();
        }

        void ISharp.TryToAutoSharp()
        {
            if (player_oper || fsm == Device_FSM_Melee.broken)
                return;

            if (fsm != Device_FSM_Melee.sharping && Sharpness_Current < 60)
            {
                StartSharp();
            }
            else if (fsm == Device_FSM_Melee.sharping && Sharpness_Current > 80)
            {
                EndSharp();
            }
        }

        public void StartSharp()
        {
            if (fsm == Device_FSM_Melee.broken)
                return;
            CloseCollider(COLLIDER_1);
            FSM_change_to(Device_FSM_Melee.sharping);
        }

        public void EndSharp()
        {
            if (fsm == Device_FSM_Melee.broken)
                return;
            FSM_change_to(Device_FSM_Melee.idle);
        }

        public void Sharp(float x_delta)
        {
            Sharpness_Current = Mathf.Min(Sharpness_Current + x_delta * Sharpness_Recover, 100);
        }
    }
}
