using TMPro;
using UnityEngine.UI;
using World.Devices.Device_AI;

namespace World.Devices.DeviceUiViews
{
    public class BasicWheelUIView : DeviceUiView
    {
        public Button Push_Car_btn;
        public Slider Push_Car_Energy_Indicator;
        public TextMeshProUGUI Push_Car_Stored_Times;

        new private BasicWheel owner;

        public override void init()
        {
            base.init();
        }

        public override void attach(Device owner)
        {
            base.attach(owner);
            this.owner = base.owner as BasicWheel;
        }

        public override void notify_on_tick()
        {
            base.notify_on_tick();
            Push_Car_btn.interactable = owner.sprint_energy_recharge_cd <= 0;
            Push_Car_Energy_Indicator.value = owner.sprint_energy_01;
            Push_Car_Stored_Times.text = owner.sprint_stored_times > 0 ? $"{owner.sprint_stored_times}" : "";
        }


        public void UI_Triggered_Sprint()
        {
            owner.UI_Controlled_Sprint();
        }
    }
}
