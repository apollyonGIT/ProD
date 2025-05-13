using Addrs;
using Foundations;

namespace World.Devices.Equip
{
    public class EquipPD : Producer
    {
        public override IMgr imgr => mgr;
        EquipmentMgr mgr;

        public override void call()
        {
            
        }

        public override void init(int priority)
        {
            mgr = new("EquipMgr", priority);
            /*Addressable_Utility.try_load_asset<EquipmentMgrView>("equipview",out var view);
            var v = Instantiate(view, WorldSceneRoot.instance.uiRoot.transform, false);
            mgr.add_view(v);
            mgr.Init(new string [0]);*/
        }
    }
}
