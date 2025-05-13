using Commons;
using UnityEngine;
using World.Helpers;

namespace World.Work.jobs
{
    public interface IEmergency
    {
        public int e_current_value { get; set; }
        public int e_max_value { get; }
    }
    public class FireJob : Job, IEmergency
    {
        private int max_level = 100;
        private int level = 0;

        public int ticker = 3 * Config.PHYSICS_TICKS_PER_SECOND;

        int IEmergency.e_current_value
        {
            get => level;
            set
            {
                level = Mathf.Clamp(value, 0, max_level);
            }
        }

        int IEmergency.e_max_value => max_level;

        public override void InitData(uint id)
        {
            base.InitData(id);
            level = 50;
        }

        public override void tick()
        {
            base.tick();

            ticker--;
            if (ticker <= 0)
            {
                fire();
                ticker = 3 * Config.PHYSICS_TICKS_PER_SECOND;
            }

            if (level <= 0)
            {
                need_remove = true;
            }

#if UNITY_EDITOR
            Debug.Log($"火势:{level}");
#endif

        }

        protected override bool CompleteJob()
        {
            level -= 20;
            return true;
        }

        private void fire()
        {
            /*var hp_remain = Device_Slot_Helper.FireHurtDevice(owner, level);
            level = Mathf.CeilToInt(level * 1.2f);

            if (hp_remain == 0)
            {
                level = 0;
            }

            level = Mathf.Clamp(level, 0, 100);*/
        }
    }
}
