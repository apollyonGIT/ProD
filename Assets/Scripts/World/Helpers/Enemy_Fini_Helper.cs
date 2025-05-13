using Commons;
using UnityEngine;
using World.Enemys;

namespace World.Helpers
{
    public class Enemy_Fini_Helper
    {
        public static void @do(Enemy cell)
        {
            //规则：甩脱时，不会播放音效和掉落物
            if (cell.is_fling_off) return;

            //死亡音效
            Audio.AudioSystem.instance.PlayOneShot(Config.current.SE_monster_die);

            //掉落物
            if (cell._desc.loot_list == null) return;

            foreach (var drop_loot in cell._desc.loot_list)
            {
                var prob = (drop_loot.Value + BattleContext.instance.drop_loot_delta) * 100;

                var r = Random.Range(0, 100);

                if (r <= prob)
                {
                    Drop_Loot_Helper.drop_loot(drop_loot.Key, cell.pos, Vector2.zero);
                }
            }
        }
    }
}

