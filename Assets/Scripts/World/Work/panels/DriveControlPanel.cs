using Commons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Helpers;

namespace World.Work.panels
{
    public enum LeverState
    {
        push = 0,
        pull = 1,
        idle = 2,
    }

    public class DriveControlPanel : WorkControlPanel
    {
        public Slider drive_slider;
        public TextMeshProUGUI status_info;
        
        private WorldContext wctx = WorldContext.instance;
        public LeverState lever_state;

        private bool is_decaying;
        private int state_ticks;
        private int state_ticks_max;

        public float change_speed;
        public GameObject[] gs;

        public int change_tick = 360;

        private const float SLIDER_DECAY_PER_SEC = 0.05f;

        public override void open()
        {
            base.open();
            drive_slider.value = wctx.driving_lever;
            status_info.text = "Ready to Go";

            lever_state = LeverState.idle;
            SetLeverView((int)lever_state);
        }

        public override void tick()
        {
            base.tick();

            switch (lever_state)
            {
                case LeverState.push:
                    drive_slider.value += change_speed * Config.PHYSICS_TICK_DELTA_TIME;
                    if(drive_slider.value  == drive_slider.maxValue)
                    {
                        change_tick--;
                        if(change_tick < 0)
                        {
                            lever_state = LeverState.idle;
                            SetLeverView((int)lever_state);
                        }
                    }
                    break;
                case LeverState.pull:
                    drive_slider.value -= change_speed * Config.PHYSICS_TICK_DELTA_TIME;
                    if (drive_slider.value == drive_slider.minValue)
                    {
                        change_tick--;
                        if (change_tick < 0)
                        {
                            lever_state = LeverState.idle;
                            SetLeverView((int)lever_state);
                        }
                    }
                    break;
                default:
                    break;    
            }

            var max_value = 100;
           // var max_value = Mathf.Min(drive_slider.value, drive_slider.maxValue * (0.5f + 0.5f * ((float)Device_Slot_Helper.GetDeviceHp(work) / Device_Slot_Helper.GetDeviceMaxHp(work))));
            drive_slider.value = max_value;
            wctx.driving_lever = max_value;
            
            // 在滑落状态时，栏杆每帧下滑
            if (is_decaying) {
                drive_slider.value *= (1 - SLIDER_DECAY_PER_SEC * Config.PHYSICS_TICK_DELTA_TIME);
            } 
            
            // 当前状态持续时间过长时，切换状态。
            if (state_ticks > state_ticks_max) {
                is_decaying = !is_decaying;
                regenereate_state_ticks_max();
            }
            state_ticks++;        
        }

        public void ChangeLeverState()
        {
            int i = (int)lever_state;
            i = i + 1 > 2 ? 0 : i + 1;
            lever_state = (LeverState)i;
            SetLeverView(i);

            reset_ticks_when_drag_lever();
        }

        public void SetLeverView(int i)
        {
            change_tick = 360;
            for(int x = 0; x < gs.Length; x++)
            {
                gs[x].SetActive(x == i);
            }
        }


        public void reset_ticks_when_drag_lever() {
            regenereate_state_ticks_max();
            is_decaying = false;
            wctx.caravan_status_acc = WorldEnum.EN_caravan_status_acc.driving;
            status_info.text = "Free";
        }

        private void regenereate_state_ticks_max() {
            state_ticks = 0;
            state_ticks_max = Mathf.CeilToInt(Random.Range(0.3f, 2f) * Config.PHYSICS_TICKS_PER_SECOND * (is_decaying ? 1f : 8f));
        }

        public void braking_btn_pressed() {
            wctx.caravan_status_acc = WorldEnum.EN_caravan_status_acc.braking;
            status_info.text = "Hold";
            drive_slider.value = 0f;
        }

    }
}
