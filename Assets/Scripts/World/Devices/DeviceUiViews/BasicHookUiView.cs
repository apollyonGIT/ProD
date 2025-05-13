using UnityEngine.UI;
using World.Devices.Device_AI;

namespace World.Devices.DeviceUiViews
{
    public class BasicHookUiView : DeviceUiView
    {
        public UiTurnTable htt;
        public Slider spring_tightness_slider;

        new private NewBasicHook owner;

        public override void attach(Device owner)
        {
            base.attach(owner);
            this.owner = base.owner as NewBasicHook;
        }

        public override void init()
        {
            base.init();
            htt.init(base.owner);
        }


        public override void notify_on_tick()
        {
            base.notify_on_tick();
            htt.tick();
            spring_tightness_slider.value = owner.spring_tightness_01;
        }
    }
}
