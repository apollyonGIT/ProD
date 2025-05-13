using AutoCodes;
using World.Work.jobs;

namespace World.Work
{
    public static class WorkUtility
    {
        public static Job Id2Job(uint id)
        {
            Job result = null;
            works.TryGetValue(id.ToString(), out var rec);
            var str = rec.job_class;
            
            switch (str)
            {
                case "FixJob":
                    result = new FixJob();
                    break;
                case "AimJob":
                    result = new AimJob();
                    break;
                case "DriveJob":
                    result = new DriveJob();
                    break;
                case "FireJob":
                    result = new FireJob();
                    break;
                case "PushCarJob":
                    result = new PushCarJob();
                    break;
                case "EncounterJob":
                    result  = new EncounterJob();
                    break;
                case "ChangeSpeedJob":
                    result = new ChangeSpeedJob();
                    break;
                case "FixCaravanJob":
                    result = new FixCaravanJob();
                    break;
                default:
                    result = new Job();
                    break;
            }
            result.InitData(id);
            return result;
        }
    }
}
