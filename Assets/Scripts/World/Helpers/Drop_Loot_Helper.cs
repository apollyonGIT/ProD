using AutoCodes;
using Commons;
using Foundations;
using UnityEngine;
using World.Loots;
using World.Relic;

namespace World.Helpers
{
    public class Drop_Loot_Helper
    {
        public static void drop_loot(uint loot_id, Vector2 pos, Vector2 init_velocity)
        {
            loots.TryGetValue(loot_id.ToString(), out var loot);

            Mission.instance.try_get_mgr("Loot", out LootMgr lmgr);
            lmgr.InstantiateLoot(new Loot()
            {
                pos = pos,
                velocity = init_velocity,
                desc = loot,
            });
        }
    }


    public class Drop_Relic_Helper
    {
        public static void drop_relic(uint relic_id)
        {
            Mission.instance.try_get_mgr(Config.RelicMgr_Name, out RelicMgr mgr);
            mgr.AddRelic($"{relic_id}");
        }
    }
}
