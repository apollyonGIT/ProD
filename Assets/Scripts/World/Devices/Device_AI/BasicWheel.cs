using AutoCodes;
using Commons;
using UnityEngine;
using World.Caravans;
using World.Helpers;
using World.Widgets;

namespace World.Devices.Device_AI
{
    public class BasicWheel : Device
    {
        #region Const
        private const string BONE_FOR_ROTATE = "roll_control";

        private const float ANGLE_LIMIT = 1919810F;
        private const float ANGLE_RESET = 114514F * Mathf.PI;

        private const int BROKEN_MALFUNC_TICKS_RAND_MIN = 300;
        private const int BROKEN_MALFUNC_TICKS_RAND_MAX = 2000;
        #endregion

        private enum FSM_Wheel
        {
            Running,
            Braking,
            Jumping,
            Broken,
        }
        private FSM_Wheel fsm;

        private float angle_rotated;
        private float wheel_radius_reciprocal;
        private float wheel_jumping_visual_speed;

        private int broken_malfunc_ticks;
        private int broken_malfunc_ticks_current;

        private bool can_rotate = true;

        private float sprint_energy_recharge_speed;
        private int sprint_energy_recharge_cd_max;
        private int sprint_stored_times_max;
        private float sprint_speed_limit;
        // Public For UI
        public int sprint_energy_recharge_cd;
        public float sprint_energy_01;
        public int sprint_stored_times;

        //=================================================================================================================

        public override void InitData(device_all rc)
        {
            module_list.Clear();    //之后可能考虑统一到卸载里面
            bones_direction.Clear();

            device_wheels.TryGetValue(rc.id.ToString() + ",0", out var wheel_rc);
            wheel_radius_reciprocal = 1f / wheel_rc.wheel_radius_visual;

            sprint_energy_recharge_speed = wheel_rc.sprint_charge_speed;
            sprint_energy_recharge_cd_max = wheel_rc.sprint_charge_cd;
            sprint_stored_times_max = Mathf.Max(1, wheel_rc.sprint_charge_point);
            sprint_speed_limit = wheel_rc.sprint_max_speed;

            bones_direction.Add(BONE_FOR_ROTATE, Vector2.right);

            base.InitData(rc);

            generate_broken_malfunc_ticks();

        }

        public override void Start()
        {
            base.Start();
            FSM_change_to(FSM_Wheel.Braking);
        }

        public override void tick()
        {
            var vel = (this as ITarget).velocity;
            var wctx = WorldContext.instance;

            switch (fsm)
            {
                case FSM_Wheel.Running:
                    if (faction == WorldEnum.Faction.player)
                    {
                        car_pushing_status_tick();
                        if (wctx.caravan_status_acc == WorldEnum.EN_caravan_status_acc.braking)
                            FSM_change_to(FSM_Wheel.Braking);
                        else if (wctx.caravan_status_liftoff == WorldEnum.EN_caravan_status_liftoff.sky)
                            FSM_change_to(FSM_Wheel.Jumping);
                    }

                    wheel_rotate(vel.magnitude * Mathf.Sign(vel.x));

                    if (!is_validate)
                        FSM_change_to(FSM_Wheel.Broken);

                    break;

                case FSM_Wheel.Braking:
                    // 只有玩家可以进入这一状态，所以不需要检查faction
                    car_pushing_status_tick();
                    if (wctx.caravan_status_liftoff == WorldEnum.EN_caravan_status_liftoff.sky)
                        FSM_change_to(FSM_Wheel.Jumping);
                    else if (wctx.caravan_status_acc == WorldEnum.EN_caravan_status_acc.driving)
                        FSM_change_to(FSM_Wheel.Running);

                    if (!is_validate)
                        FSM_change_to(FSM_Wheel.Broken);
                    break;

                case FSM_Wheel.Jumping:
                    // 只有玩家可以进入这一状态，所以不需要检查faction
                    car_pushing_status_tick();
                    if (wctx.caravan_status_liftoff == WorldEnum.EN_caravan_status_liftoff.ground)
                        FSM_change_to(wctx.caravan_status_acc == WorldEnum.EN_caravan_status_acc.driving ? FSM_Wheel.Running : FSM_Wheel.Braking);

                    wheel_rotate(wheel_jumping_visual_speed);

                    if (!is_validate)
                        FSM_change_to(FSM_Wheel.Broken);
                    break;

                case FSM_Wheel.Broken:
                    if (faction == WorldEnum.Faction.player)
                    {
                        if (wctx.caravan_status_liftoff == WorldEnum.EN_caravan_status_liftoff.sky)
                        {
                            wheel_rotate(wheel_jumping_visual_speed);
                            break;
                        }

                        if (vel.magnitude > 1f)
                        {
                            if (broken_malfunc_ticks_current >= broken_malfunc_ticks)
                            {
                                broken_malfunc_ticks_current = 0;
                                generate_broken_malfunc_ticks();

                                //跳起
                                CaravanMover.do_jump_input_vy(vel.magnitude * 0.5f);

                                //刹车，且不重置target_lever高度
                                Widget_DrivingLever_Context.instance.SetLever(0, false);
                                WorldContext.instance.caravan_status_acc = WorldEnum.EN_caravan_status_acc.braking;
                            }
                            else
                                broken_malfunc_ticks_current++;
                        }
                    }

                    wheel_rotate(vel.magnitude);

                    if (is_validate)
                        FSM_change_to(FSM_Wheel.Running);

                    break;

                default:
                    break;
            }

            base.tick();
        }

        private void FSM_change_to(FSM_Wheel expected_fsm)
        {
            switch (expected_fsm)
            {
                case FSM_Wheel.Running:
                    wheel_jumping_visual_speed = 10f;
                    can_rotate = true;
                    break;
                case FSM_Wheel.Braking:
                    wheel_jumping_visual_speed = 0f;
                    can_rotate = false;
                    break;
                case FSM_Wheel.Jumping:
                    can_rotate = true;
                    break;
                case FSM_Wheel.Broken:
                    wheel_jumping_visual_speed = 1f;
                    broken_malfunc_ticks_current = 0;
                    can_rotate = true;
                    break;
                default:
                    break;
            }
            fsm = expected_fsm;
        }

        // ----------------------------------------------------------------------------------------------

        private void wheel_rotate(float v)
        {
            if (!can_rotate)
                return;

            angle_rotated -= v * wheel_radius_reciprocal * Config.PHYSICS_TICK_DELTA_TIME;   //Caculate in Rad

            if (Mathf.Abs(angle_rotated) > ANGLE_LIMIT)
                angle_rotated -= Mathf.Sign(angle_rotated) * ANGLE_RESET;

            bones_direction[BONE_FOR_ROTATE] = new Vector2(Mathf.Cos(angle_rotated), Mathf.Sin(angle_rotated));
        }

        private void generate_broken_malfunc_ticks()
        {
            broken_malfunc_ticks = Random.Range(BROKEN_MALFUNC_TICKS_RAND_MIN, BROKEN_MALFUNC_TICKS_RAND_MAX);
        }


        private void car_pushing_status_tick()
        {
            if (sprint_energy_recharge_cd > 0)
                sprint_energy_recharge_cd--;

            if (sprint_energy_recharge_cd > 0)
                return;

            sprint_energy_01 += sprint_energy_recharge_speed;

            if (sprint_energy_01 <= 1f)
                return;

            if (sprint_stored_times < sprint_stored_times_max)
            {
                sprint_stored_times++;
                sprint_energy_01--;
            }
            else
            {
                sprint_energy_01 = 1f;
            }
        }

        public void UI_Controlled_Sprint()
        {
            var car = WorldContext.instance;

            Road_Info_Helper.try_get_leap_rad(car.caravan_pos.x, out var ground_rad);
            var cos_ground_rad = Mathf.Cos(ground_rad);

            if (sprint_stored_times > 0)
            {
                sprint(1f);
                sprint_stored_times--;
            }
            else
            {
                sprint(sprint_energy_01);
                sprint_energy_recharge_cd = sprint_energy_recharge_cd_max;
                sprint_energy_01 = 0f;
            }

            void sprint(float energy_input)
            {
                //car.caravan_vx_stored += Mathf.Clamp(cos_ground_rad + 0.1f - car.caravan_velocity.magnitude * 2f, 0f, 0.5f);
                car.caravan_vx_stored += Mathf.Max(cos_ground_rad * Mathf.Pow(energy_input, 2) * 3f, sprint_speed_limit);
            }
        }
    }
}
