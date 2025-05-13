using AutoCodes;
using System;
using System.Collections.Generic;

namespace World.Devices.DeviceUpgrades
{
    public interface IDeviceUpgrade
    {
        public bool Applied { get; set; }
        public void ApplyUpgrade(Device device);
        public void RemoveUpgrade(Device device);
    }

    public class DeviceUpgrade
    {
        public DeviceUpgrade(device_upgrade desc)
        {
            this.desc = desc;
            InitIUpgrade();
        }
        public device_upgrade desc;
        private IDeviceUpgrade upgrade;

        private void InitIUpgrade()
        {
            switch (desc.script)
            {
                case "ModuleUpgrade":
                    upgrade = new ModuleUpgrade(desc.int_parms[0]);
                    break;
                case "AttackUpgrade":
                    upgrade = new AttackUpgrade(desc.int_parms[0] / 100f, desc.int_parms[1]/100f, desc.int_parms[2]/100f, desc.int_parms[3]/100f);
                    break;
                case "ExtendedAmmoUpgrade":
                    upgrade = new ExtendedAmmoUpgrade(desc.int_parms[0]);
                    break;
                default:
                    break;
            }
        }

        public bool Upgradeable(Device device)
        {
            List<uint> temp = new List<uint>();
            foreach (var upgrade in device.upgrades)
            {
                if(upgrade.desc.id == desc.id && upgrade.Applied())          //同一组
                {
                    temp.Add(upgrade.desc.sub_id);          //记录同组所有已经启动了的升级的子id
                }
            }
            if (desc.incompatible != null)
            {
                foreach (var incompatible in desc.incompatible)
                {
                    if (temp.Contains(incompatible))
                        return false;
                }
            }
            if (desc.precondition != null)
            {
                foreach (var precondition in desc.precondition)
                {
                    if (!temp.Contains(precondition))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 是否存在冲突
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>

        public bool Incompatible(Device device)
        {
            List<uint> temp = new List<uint>();
            foreach (var upgrade in device.upgrades)
            {
                if (upgrade.desc.id == desc.id && upgrade.Applied())          //同一组
                {
                    temp.Add(upgrade.desc.sub_id);          //记录同组所有已经启动了的升级的子id
                }
            }
            if (desc.incompatible != null)
            {
                foreach (var incompatible in desc.incompatible)
                {
                    if (temp.Contains(incompatible))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否满足前置
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool Precondition(Device device)
        {
            List<uint> temp = new List<uint>();
            foreach (var upgrade in device.upgrades)
            {
                if (upgrade.desc.id == desc.id && upgrade.Applied())          //同一组
                {
                    temp.Add(upgrade.desc.sub_id);          //记录同组所有已经启动了的升级的子id
                }
            }

            if (desc.precondition != null)
            {
                foreach (var precondition in desc.precondition)
                {
                    if (!temp.Contains(precondition))
                        return false;
                }
            }

            return true;
        }

        public bool Applied()
        {
            return upgrade.Applied;
        }

        public void ApplyUpgrade(Device device)
        {
            upgrade.ApplyUpgrade(device);
        }

        public void RemoveUpgrade(Device device)
        {
            upgrade.RemoveUpgrade(device);
        }
    }
}
