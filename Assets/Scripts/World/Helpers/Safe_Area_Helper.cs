using Addrs;
using AutoCodes;
using Commons;
using Foundations;
using UnityEngine;
using World.BackPack;
using World.Business;
using World.Characters;
using World.Devices.Equip;
using World.Progresss;

namespace World.Helpers
{
    public class Safe_Area_Helper
    {
        public static void enter()
        {
            WorldSceneRoot.instance.BlackScreen();
            Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
            foreach (var character in cmgr.characters)
            {
                if (character != null)
                {
                    character.EnterSafeArea();
                }
            }
        }

        public static void leave()
        {
            Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
            foreach (var character in cmgr.characters)
            {
                if (character != null)
                {
                    character.LeaveSafeArea();
                }
            }
            Progress_Context.instance.leave_with_next_scene();
        }

        public static int GetLootCount(int loot_id)
        {
            Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);
            return bmgr.GetLootAmount(loot_id);
        }

        public static bool SpendLootCount(int loot_id, int count)
        {
            Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);

            return bmgr.RemoveLoot(loot_id, count);
        }

        public static void TryToSellDevice(Business.Business b)
        {
            Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);
            Mission.instance.try_get_mgr("EquipMgr", out EquipmentMgr emgr);
            if (emgr.select_device != null)
            {
                emgr.RemoveDevice(emgr.select_device);
                
                for (int i = 0; i < 2; i++)
                    bmgr.AddLoot(Config.current.coin_id);
                b.AddGoods(emgr.select_device,GoodsType.device,emgr.select_device.desc.id,6000,2);

                emgr.SelectDevice(null);
            }
        }

        public static void TryToSellCharacter(Business.Business b)
        {
            Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);
            Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
            if (cmgr.select_character != null)
            {
                cmgr.RemoveCharacter(cmgr.select_character);

                for (int i = 0; i < 2; i++)
                    bmgr.AddLoot(Config.current.coin_id);
                b.AddGoods(cmgr.select_character,GoodsType.role,cmgr.select_character.desc.id,6000, 2);
                cmgr.SelectCharacter(null);
            }
        }

        public static Character CreateCharacter(role role)
        {
            var c = new Character();
            c.Init(role);
            c.Start();

            return c;
        }

        public static DeviceBusinessPanel InsDeviceBusinessPanel(Transform t)
        {
            Addressable_Utility.try_load_asset<DeviceBusinessPanel>("BusinessPanel",out var asset);
            var dbp = GameObject.Instantiate(asset,t,false);
            return dbp;
        }
    }
}

