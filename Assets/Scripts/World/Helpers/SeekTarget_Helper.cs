using Foundations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World.Helpers
{
    public class SeekTarget_Helper
    {

        //==================================================================================================

        public static IEnumerable<ITarget> all_targets()
        {
            Mission.instance.try_get_mgr("DeviceMgr", out Devices.DeviceMgr deviceMgr);
            foreach (var target in deviceMgr.slots_device.Where(t => t.Value != null).Select(t => t.Value))
            {
                if (target.current_hp <= 0) continue;
                yield return target;
            }

            Mission.instance.try_get_mgr("CaravanMgr", out Caravans.CaravanMgr caravanMgr);
            yield return caravanMgr.cell;

            Mission.instance.try_get_mgr("EnemyMgr", out Enemys.EnemyMgr enemyMgr);
            foreach (var target in enemyMgr.cells)
            {
                if (target.hp <= 0) continue;
                yield return target;
            }
        }


        public static IEnumerable<ITarget> player_parts()
        {
            Mission.instance.try_get_mgr("DeviceMgr", out Devices.DeviceMgr deviceMgr);
            foreach (var target in deviceMgr.slots_device.Where(t => t.Value != null).Select(t => t.Value))
            {
                if (target.current_hp <= 0) continue;
                yield return target;
            }

            Mission.instance.try_get_mgr("CaravanMgr", out Caravans.CaravanMgr caravanMgr);
            yield return caravanMgr.cell;
        }


        public static void nearest_player_part(ITarget self, out ITarget target)
        {
            target = default;
            float dis = 99999f;

            foreach (var temp_target in player_parts())
            {
                var temp_dis = temp_target.distance(self);
                if (temp_dis >= dis) continue;

                dis = temp_dis;
                target = temp_target;
            }
        }


        public static void random_player_part(out ITarget target)
        {
            var e = player_parts();
            var index = Random.Range(0, e.Count());

            target = e.ElementAt(index);
        }
    }
}

