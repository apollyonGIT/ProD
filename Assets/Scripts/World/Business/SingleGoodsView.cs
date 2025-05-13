using Addrs;
using AutoCodes;
using Commons;
using Foundations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using World.Devices;
using World.Devices.DeviceUpgrades;
using World.Devices.Equip;
using World.Helpers;

namespace World.Business
{
    public class SingleGoodsView : MonoBehaviour,IPointerClickHandler
    {
        public TextMeshProUGUI goods_name;
        public TextMeshProUGUI goods_price;

        public GoodsData goods_record;

        public DeviceBusinessView dbv;

        public void Init(GoodsData rec)
        {
            goods_record = rec;
            var device = (goods_record.obj as Device);

            goods_name.text = Localization_Utility.get_localization(device.desc.name);
            goods_price.text = goods_record.price_count.ToString();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Mission.instance.try_get_mgr("EquipMgr", out EquipmentMgr emgr);
            Mission.instance.try_get_mgr(Config.DeviceMgr_Name, out DeviceMgr dmgr);

            if (goods_record.price_count <= Safe_Area_Helper.GetLootCount(goods_record.price_id)){

                Safe_Area_Helper.SpendLootCount(goods_record.price_id, goods_record.price_count);

                dbv.main_panel.owner.RemoveGoods(goods_record.goods_id);

                var device = (goods_record.obj as Device);
                emgr.AddDevice(device);
            }
        }

        public void ShowUpgrades()
        {
            var device = (goods_record.obj as Device);
            Addressable_Utility.try_load_asset<DeviceUpgradesView>("DeviceUpgradesView", out var upgrade_panel);
            var u_view = Instantiate(upgrade_panel, WorldSceneRoot.instance.uiRoot.transform, false);
            u_view.Init(device);
            u_view.IsReadOnly = true;
        }
    }
}
