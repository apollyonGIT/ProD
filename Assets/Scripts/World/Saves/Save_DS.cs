using Commons;
using Foundations;
using System.Collections.Generic;
using System.Linq;

namespace World.Saves
{
    public class Save_DS : Singleton<Save_DS>
    {
        public bool need_load_device = false;

        public List<object> caravan_datas = new();
        public Dictionary<string, string> device_datas = new();
        public List<object> backpack_datas = new();
        public List<uint> relic_datas = new();
        public List<List<object>> character_datas = new();

        //==================================================================================================

        public void save()
        {
            clear();

            var ctx = WorldContext.instance;

            //车体
            if (Mission.instance.try_get_mgr("CaravanMgr", out Caravans.CaravanMgr caravan_mgr))
            {
                caravan_datas.Add(caravan_mgr.cell._desc.id);
                caravan_datas.Add(ctx.caravan_hp);
                caravan_datas.Add(ctx.caravan_hp_max);
            }

            //设备
            if (Mission.instance.try_get_mgr(Config.DeviceMgr_Name, out Devices.DeviceMgr device_mgr))
            {
                foreach (var (slot_name, device) in device_mgr.slots_device.Where(t => t.Value != null))
                {
                    device_datas.Add(slot_name, $"{device.desc.id}");
                }
            }

            //背包
            if (Mission.instance.try_get_mgr("BackPack", out BackPack.BackPackMgr backpack_mgr))
            {
                backpack_datas.Add(backpack_mgr.slot_count);
                backpack_datas.Add(backpack_mgr.get_all_loots());
            }

            //遗物
            if (Mission.instance.try_get_mgr(Config.RelicMgr_Name, out Relic.RelicMgr relic_mgr))
            {
                foreach (var relic in relic_mgr.relic_list)
                {
                    relic_datas.Add(relic.desc.id);
                }
            }

            //角色
            if (Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out Characters.CharacterMgr character_mgr))
            {
                foreach (var character in character_mgr.characters)
                {
                    List<object> data = new();
                    data.Add(character.desc.id);
                    data.Add(character.current_mood); //心情值
                    data.Add((int)character.es); //进食状态

                    character_datas.Add(data);
                }
            }

            need_load_device = true;
        }


        public void clear()
        {
            need_load_device = false;

            device_datas.Clear();
            caravan_datas.Clear();
            backpack_datas.Clear();
            relic_datas.Clear();
            character_datas.Clear();
        }
    }
}

