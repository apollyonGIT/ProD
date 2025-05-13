using Commons;
using World.Helpers;

namespace World.Work.jobs
{
    public class FixJob :Job
    {
        protected override bool CompleteJob()
        {
            if(!able)
                return false;

            return true;
        }

        protected override void NotWorking()
        {
            base.NotWorking();

            UpdateProgress(-Config.current.normal_work_ability * Config.PHYSICS_TICK_DELTA_TIME);
        }
    }
}
