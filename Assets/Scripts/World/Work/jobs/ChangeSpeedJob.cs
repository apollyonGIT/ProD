using Foundations.Tickers;
using World.Helpers;

namespace World.Work.jobs
{
    public class ChangeSpeedJob : Job
    {
        public Request request = null;
        protected override bool CompleteJob()
        {
            /*Device_Slot_Helper.ChangeSpeed(owner, ref request);*/

            return base.CompleteJob();
        }
    }
}
