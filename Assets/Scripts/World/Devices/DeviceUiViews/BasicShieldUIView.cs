using UnityEngine.UI;
using World.Devices.Device_AI;

namespace World.Devices.DeviceUiViews
{
    public class BasicShieldUIView : DeviceUiView
    {
        public Slider ShieldEnergyShlider;

        new private BasicShield owner;

        public override void attach(Device owner)
        {
            base.attach(owner);
            this.owner = base.owner as BasicShield;
        }


        public override void notify_on_tick()
        {
            base.notify_on_tick();
            ShieldEnergyShlider.value = owner.ShieldEnergy_Current;
            ShieldEnergyShlider.maxValue = owner.ShieldEnergy_Max;
        }

        public void UI_Controlled_Reset_Shield()
        {
            owner.Def_End_By_UI_Control();
        }

    }
}
