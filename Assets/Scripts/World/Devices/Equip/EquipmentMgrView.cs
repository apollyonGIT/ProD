using Foundations.MVVM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace World.Devices.Equip
{
    public class EquipmentMgrView : MonoBehaviour,IEquipmentMgrView
    {
        public EquipmentMgr owner;

        public RawImage caravan_image;
        public List<EquipSlot> slots = new();
        public Transform content;

        public EquipView prefab;
        public List<EquipView> edv = new List<EquipView>();
        void IEquipmentMgrView.add_device(Device device)
        {
            var d = Instantiate(prefab, content, false);
            d.Init(device,this);
            d.gameObject.SetActive(true);

            edv.Add(d);
        }

        void IModelView<EquipmentMgr>.attach(EquipmentMgr owner)
        {
            this.owner = owner;
        }

        void IModelView<EquipmentMgr>.detach(EquipmentMgr owner)
        {
            this.owner = null;
            Destroy(gameObject);
        }

        void IEquipmentMgrView.init()
        {
            foreach (var slot in slots)
            {
                slot.init();
            }

            foreach(var device in owner.devices)
            {
                var d = Instantiate(prefab,content,false);
                d.Init(device,this);
                d.gameObject.SetActive(true);

                edv.Add(d);
            }
        }

        void IEquipmentMgrView.remove_device(Device device)
        {
            for(int i = edv.Count - 1; i >= 0; i--)
            {
                if(edv[i].data == device)
                {
                    Destroy(edv[i].gameObject);
                    edv.RemoveAt(i);
                }
            }
        }

        void IEquipmentMgrView.select_device(Device device)
        {
            foreach(var d in edv)
            {
                d.Select(d.data == device);
            }
        }

        void IEquipmentMgrView.tick()
        {
            
        }

        void IEquipmentMgrView.update_slot()
        {
            foreach(var slot in slots)
            {
                slot.update_slot();
            }
        }
    }
}
