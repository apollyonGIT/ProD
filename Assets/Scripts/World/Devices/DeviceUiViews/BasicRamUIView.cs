using UnityEngine.UI;
using UnityEngine;
using World.Devices.Device_AI;

namespace World.Devices.DeviceUiViews
{
    public class BasicRamUIView : DeviceUiView
    {
        private float GEAR_BG_ROTATE_COEF = 7F;



        public UiTurnTable htt;

        public Image Gear_BG;
        public Image Gear_indicator;

        new private BasicRam owner;

        public override void init()
        {
            base.init();
            
        }

        public override void attach(Device owner)
        {
            base.attach(owner);
            this.owner = base.owner as BasicRam;
            htt.init(base.owner);
        }


        public override void notify_on_tick()
        {
            base.notify_on_tick();
            htt.tick();

            var a = Vector2.SignedAngle(Vector2.right, owner.ram_dir);
            Gear_indicator.transform.eulerAngles = new Vector3(0, 0, a);
            Gear_BG.transform.eulerAngles = new Vector3(0, 0, -a * GEAR_BG_ROTATE_COEF);
        }
    }
}
