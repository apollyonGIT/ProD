using Commons;
using Foundations;
using System.Collections.Generic;

namespace World.BackPack
{
    public class BackPackPD : Producer
    {
        public BackPackMgrView bmv;
        public override IMgr imgr => mgr;
        BackPackMgr mgr;

        public override void call()
        {
            
        }

        public override void init(int priority)
        {
            if (Saves.Save_DS.instance.need_load_device)
                use_current_data(priority);
            else
                use_default_data(priority);
        }


        public void use_current_data(int priority)
        {
            var datas = Saves.Save_DS.instance.backpack_datas;

            mgr = new BackPackMgr("BackPack", priority);
            mgr.add_view(bmv);

            mgr.Init((int)datas[0]);

            var all_loot_infos = (List<uint>)datas[1];
            foreach (var loot_id in all_loot_infos)
            {
                mgr.AddLoot(loot_id);
            }
        }


        public void use_default_data(int priority)
        {
            mgr = new BackPackMgr("BackPack", priority);
            mgr.add_view(bmv);

            mgr.Init(Config.current.basic_backpack_slot_num);
        }
    }
}
