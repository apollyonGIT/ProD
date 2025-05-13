using Commons;
using UnityEngine;

namespace World.Work.jobs
{
    public class FixCaravanJob :Job
    {
        protected override bool CompleteJob()
        {
            if (!able)
                return false;
            var wctx = WorldContext.instance;

            wctx.caravan_hp += (int)(wctx.caravan_hp_max * Config.current.fix_caravan_job_effect);
            wctx.caravan_hp = Mathf.Min(wctx.caravan_hp, wctx.caravan_hp_max);
            return true;
        }

        protected override void NotWorking()
        {
            base.NotWorking();

            UpdateProgress(-Config.current.normal_work_ability * Config.PHYSICS_TICK_DELTA_TIME);
        }
    }
}
