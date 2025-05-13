using Commons;
using World.Helpers;
using UnityEngine;

namespace World.Work.jobs
{
    public class PushCarJob :Job
    {
        protected override bool CompleteJob()
        {
            if (!able)
                return false;

            var car = WorldContext.instance;

            Road_Info_Helper.try_get_leap_rad(car.caravan_pos.x, out var ground_rad);
            var cos_ground_rad = Mathf.Cos(ground_rad);

            WorldContext.instance.caravan_vx_stored += Mathf.Clamp(cos_ground_rad + 0.1f - car.caravan_velocity.magnitude * 2f, 0f, 0.5f);

            return true;
        }

        protected override void NotWorking()
        {
            base.NotWorking();

            UpdateProgress(-Config.current.normal_work_ability * Config.PHYSICS_TICK_DELTA_TIME);
        }
    }
}
