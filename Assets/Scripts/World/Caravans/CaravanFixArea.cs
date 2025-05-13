using Commons;
using UnityEngine;

namespace World.Caravans
{
    /// <summary>
    /// 车体ui 可以被修复的外观挂载这个脚本
    /// </summary>
    public class CaravanFixArea : MonoBehaviour, IUiFix
    {
        void IUiFix.Fix()
        {
            var wctx = WorldContext.instance;

            wctx.caravan_hp += (int)(wctx.caravan_hp_max * Config.current.fix_caravan_job_effect * 3);
            wctx.caravan_hp = Mathf.Min(wctx.caravan_hp, wctx.caravan_hp_max);
        }
    }
}
