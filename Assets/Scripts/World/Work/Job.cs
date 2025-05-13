using AutoCodes;
using System.Collections.Generic;

namespace World.Work
{
    public class Job
    {
        public Work owner;
        public virtual string _name => m_name;
        private string m_name;
        public List<Cubicle> cubicles = new List<Cubicle>();
        public bool able = true;
        public bool need_remove = false;

        public float max_value;
        public float current_value { get; private set; }
        public virtual void UpdateProgress(float delta)
        {
            if (!able)
            {
                return;
            }

            current_value += delta;
            if (current_value > max_value)
            {
                if(CompleteJob())
                    current_value -= max_value;
            }
            if (current_value < 0)
            {
                current_value = 0;
            }
        }

        protected virtual bool CompleteJob()
        {
            return able;
        }

        public virtual void tick()
        {
            if (!able)
            {
                NotWorking();
                return;
            }
            bool has_people = false;
            foreach(var cubicle in cubicles)
            {
                if (cubicle.has_worker)
                {
                    has_people = true;
                    cubicle.tick();
                }
            }
            if (has_people) 
            {
                Working();
                has_people = false;
            }
            else
            {
                NotWorking();
            }
        }

        protected virtual void Working()
        {

        }

        protected virtual void NotWorking()
        {

        }

        public virtual void InitData(uint id)       //
        {
            works.TryGetValue(id.ToString(), out var record);

            max_value = record.job_amount;
            current_value = 0;
            m_name = record.name;

            for (var i = 0; i < record.cubicle_count; i++)
            {
                var c = new Cubicle();
                c.owner = this;
                cubicles.Add(c);
            }
        }

        public virtual void Init()
        {

        }

        public virtual bool IsWorking()
        {
            if(!able)
                return false;

            foreach (var cubicle in cubicles)
            {
                if (cubicle.has_worker)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
