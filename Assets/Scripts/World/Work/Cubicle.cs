using Commons;

namespace World.Work
{
    public class Cubicle
    {
        public Job owner;
        public bool has_worker;
        public void tick()
        {
            if(owner.max_value != 0)
            {
                owner.UpdateProgress(Config.current.normal_work_ability * Config.PHYSICS_TICK_DELTA_TIME);
            }
        }
    }
}
