using AutoCodes;
using Commons;
using Foundations;
using Foundations.Tickers;
using UnityEngine;
using World.Devices.Device_AI;
using World.Helpers;

namespace World.Widgets
{
    public class Widget_DrivingLever_Context : Singleton<Widget_DrivingLever_Context>
    {
        #region
        public const float MAX_LEVER = 1.25f;

        private const int HIGH_LEVER_STATE_TICK = 1000;
        #endregion

        private float state_ticks;
        private Devices.Device wheel_device;  //没有找到合适的位置进行初始化
        private bool wheel_is_ok() => wheel_device == null || wheel_device.is_validate;

        public bool will_switch_into_high_lever() => wheel_is_ok() && WorldContext.instance.caravan_velocity.magnitude >= get_standard_car_speed();
        public bool can_switch_into_high_lever() => wheel_is_ok() && state_ticks >= HIGH_LEVER_STATE_TICK;

        public float target_lever { get; private set; }

        public CaravanModule driving_module;

        //==================================================================================================

        public void attach()
        {
            Ticker.instance.do_when_tick_start += tick;

            driving_module = new CaravanModule();
        }


        public void detach()
        {
            Ticker.instance.do_when_tick_start -= tick;
        }


        private void tick()
        {
            ref var driving_lever = ref WorldContext.instance.driving_lever;

            if (will_switch_into_high_lever())
            {
                if (!can_switch_into_high_lever())
                    state_ticks++;
            }
            else
            {
                state_ticks = 0;
            }

            if (driving_lever > 1 && !can_switch_into_high_lever())
                SetLever(1f, false);

            driving_module.tick();
        }

        public void SetLever(float value, bool also_set_target_lever)
        {
            value = Mathf.Clamp(value, 0f, MAX_LEVER);

            if (also_set_target_lever)
                target_lever = value;

            WorldContext.instance.driving_lever = value;
        }

        public void Drag_Lever(bool can_drag, bool drag_to_up, bool also_set_target_lever)
        {
            var base_lever = WorldContext.instance.driving_lever;
            if (can_drag)
            {
                var dir_speed = drag_to_up ? Config.current.lever_up_speed : Config.current.lever_down_speed;
                base_lever += (dir_speed - base_lever) * Config.current.lever_move_delta;

                if (base_lever >= 1f && drag_to_up)
                {
                    if (can_switch_into_high_lever())
                        base_lever = MAX_LEVER;
                    else
                        base_lever = 1f;
                }

                if (base_lever > 1f && !drag_to_up)
                    base_lever = 1f;

                SetLever(base_lever, also_set_target_lever);

                if (WorldContext.instance.driving_lever != 0)
                    WorldContext.instance.caravan_status_acc = WorldEnum.EN_caravan_status_acc.driving;
            }
        }

        private float get_standard_car_speed()
        {
            // 一直在做查询可能会有额外的性能开销
            wheel_device = Device_Slot_Helper.GetDevice("device_slot_wheel");
            device_wheels.TryGetValue(wheel_device.desc.id.ToString() + ",0", out var wheel_rc);
            
            return wheel_rc.high_lever_switch_speed;
        }

    }
}

