using Foundations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace World.Relic.RelicStores
{
    public class RelicStoreSingleView : RelicView, IPointerClickHandler
    {
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Mission.instance.try_get_mgr(Commons.Config.RelicMgr_Name, out RelicMgr rmgr);
            rmgr.AddRelic(data);
            rmgr.ClearRelicStore();
        }
    }
}
