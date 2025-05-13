using Foundations;
using Foundations.MVVM;
using UnityEngine;
using World.Devices.Equip;

namespace World.Business { 
    public class DeviceBusinessPanel :MonoBehaviour,IBusinessView 
    {
        public EquipmentMgrView ev;
        public DeviceBusinessView dv;

        public bool inited = false;
        public Business owner;

        public void Init(int business_id)
        {
            if (inited)
                return;
            Mission.instance.try_get_mgr("EquipMgr", out EquipmentMgr emgr);
            emgr.add_view(ev);
            emgr.Init(new string[] { });


            Mission.instance.try_get_mgr("BusinessMgr", out BusinessMgr bmgr);
            owner = new Business();
            bmgr.AddBusiness(owner);
            owner.add_view(this);
            owner.Init((uint)business_id);
            inited = true;
        }

        void IModelView<Business>.attach(Business owner)
        {
            this.owner = owner;            
        }

        void IModelView<Business>.detach(Business owner)
        {
            if(this.owner != null)
            {
                this.owner = null;
            }

            Destroy(gameObject);
        }

        void IBusinessView.init()
        {
            dv.Init();
        }

        void IBusinessView.update_goods()
        {
            dv.UpdateGoods();
        }
    }
}
    