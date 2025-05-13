using AutoCodes;
using Commons;
using Foundations;
using System.Collections.Generic;
using World.Devices;
using World.Helpers;

namespace World.Work
{
    public class WorkPD : Producer
    {
        public WorkPanelView wpv;
        public WorkMgrView wmv;
        public override IMgr imgr => mgr;
        WorkMgr mgr;

        public override void call()
        {

        }

        public override void init(int priority)
        {
            
            
        }

    }
}
