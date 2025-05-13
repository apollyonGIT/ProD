using Commons;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Helpers;

namespace World.Devices.DeviceUpgrades
{
    public class DeviceUpgradeView : MonoBehaviour
    {
        public DeviceUpgradesView owner;
        public DeviceUpgrade data;

        public GameObject buy_button;
        public TextMeshProUGUI name_text;

        public void Init(DeviceUpgrade upgrade)
        {
            data= upgrade;
            name_text.text = Localization_Utility.get_localization(upgrade.desc.name);
            UpdateView();
        }

        public void tick()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (data.Applied())
            {
                GetComponent<Image>().color = Color.green;
                buy_button.SetActive(false);
            }
            else if(data.Upgradeable(owner.own_device))
            {
                GetComponent<Image>().color = Color.white;
                buy_button.SetActive(true);
            }
            else
            {
                if (data.Incompatible(owner.own_device))        //存在冲突
                {
                    GetComponent<Image>().color = Color.black;
                    buy_button.SetActive(false);
                }
                else                                            //有待解锁的前置
                {
                    GetComponent<Image>().color = Color.gray;
                    buy_button.SetActive(false);
                }
            }
        }

        public void ShowInfo()
        {
            owner.infoView.Init(data);
        }

        public void Buy()
        {
            if (owner.IsReadOnly)
                return;

            if (Upgrade_Cost_Helper.CheckUpgradeCost(data)&&data.Upgradeable(owner.own_device))
            {
                Upgrade_Cost_Helper.UpgradeCost(data,owner.own_device);
            }
        }
    }
}
