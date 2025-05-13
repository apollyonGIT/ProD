using Addrs;
using Commons;
using Foundations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using World.Devices.DeviceUpgrades;
using World.Helpers;

namespace World.Devices.Equip
{
    public class EquipSlot :MonoBehaviour,IPointerClickHandler
    {
        public EquipmentMgrView owner;
        public TextMeshProUGUI slot_device;

        [SerializeField]
        private string slot_name;

        public EquipSlot(string str)
        {
            slot_name = str;
        }

        public void update_slot()
        {
            Mission.instance.try_get_mgr(Config.DeviceMgr_Name, out DeviceMgr dmgr);
            dmgr.slots_device.TryGetValue(slot_name, out var device);

            if (device == null)
                slot_device.text = "";
            else
            {
                slot_device.text = Localization_Utility.get_localization(device.desc.name);
            }
        }

        public void init()
        {
            Mission.instance.try_get_mgr(Config.DeviceMgr_Name, out DeviceMgr dmgr);
            dmgr.slots_device.TryGetValue(slot_name, out var device);

            if(device!=null)
                slot_device.text = Localization_Utility.get_localization(device.desc.name);
            else
            {
                slot_device.text = "";
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var emgr = owner.owner;
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                var d = Device_Slot_Helper.RemoveDevice(slot_name);
                emgr.AddDevice(d);
            }
            else
            {
                if (owner.owner.select_device != null)
                {
                    emgr.RemoveDevice(emgr.select_device);
                    var remove_device = Device_Slot_Helper.InstallDevice(emgr.select_device, slot_name);
                    emgr.select_device = null;
                    emgr.AddDevice(remove_device);
                }
            }
            owner.owner.UpdateSlot();
        }

        public void OpenUpgradePanel()
        {
            var device = Device_Slot_Helper.GetDevice(slot_name);
            Addressable_Utility.try_load_asset<DeviceUpgradesView>("DeviceUpgradesView", out var upgrade_panel);
            var u_view = Instantiate(upgrade_panel, WorldSceneRoot.instance.uiRoot.transform, false);
            u_view.Init(device);
        }
    }
}
