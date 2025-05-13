using AutoCodes;
using Commons;
using Foundations.Tickers;
using UnityEngine;
using World.Devices.Device_AI;

namespace World.Progresss
{
    public class ProgressEvent
    {
        public float trigger_progress;
        public bool notice_triggered = false;

        public bool need_remove = false;

        public event_site record;

        public ProgressModule module;
        public int current_value = 0 ;
        public int max_value = 100;

        public Vector2 pos;

        public string cd_TX => launch_loot_cd == 0 ? "" : $"{Mathf.CeilToInt((float)(launch_loot_cd) / Config.PHYSICS_TICKS_PER_SECOND)}秒后获得{launch_loot_name}";
        public int launch_loot_cd = 0;
        public string launch_loot_name = "";
        public Request lanuch_loot_req;


        public ProgressEvent()
        { 
        }


        public ProgressEvent(float v,event_site record,Vector2 pos)
        {
            trigger_progress = v;
            this.record = record;
            this.pos = pos;
        }

        public void Enter()
        {
            notice_triggered = true;
        }

        public void tick()
        {
            if (current_value >= max_value)
            {
                CompleteJob();
                current_value = 0;
            }

            module.tick();
        }

        private void CompleteJob()
        {
            Encounters.Dialogs.Encounter_Dialog.instance.init(this, "Dialog_Window");
        }

        public void Exit()
        {
            need_remove = true;
            Encounters.Dialogs.Encounter_Dialog.instance.fini(null);
        }
    }
}
