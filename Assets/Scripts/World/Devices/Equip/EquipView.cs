using Addrs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace World.Devices.Equip
{
    public class EquipView : MonoBehaviour,IPointerClickHandler
    {
        public GameObject select;
        public Device data;

        [HideInInspector]
        public EquipmentMgrView owner;
        public Image icon;

        public void Init(Device device,EquipmentMgrView owner)
        {
            data = device;
            this.owner = owner;
            if(data.desc.icon != "" && data.desc.icon != null)
            {
                Addressable_Utility.try_load_asset<Sprite>(data.desc.icon, out var sprite);
                icon.sprite = sprite;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            owner.owner.SelectDevice(data);
        }

        public void Select(bool b)
        {
            select.SetActive(b);
        }
    }
}
