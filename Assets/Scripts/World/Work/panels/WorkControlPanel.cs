using UnityEngine;
using World.Helpers;

namespace World.Work.panels
{
    public class WorkControlPanel : MonoBehaviour
    {
        public GameObject mask;
        public Work work;
        public void init(Work work)
        {
            this.work = work;
        }

        public virtual void tick()
        {

        }

        public virtual void open()
        {
            
        }

        public virtual void close()
        {

        }
    }
}
