using Foundations.MVVM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace World.Relic
{
    public class RelicMgrView : MonoBehaviour, IRelicMgrView
    {
        RelicMgr owner;

        public List<RelicView> relic_views = new();
        public RelicView prefab;

        public Transform content;
        void IModelView<RelicMgr>.attach(RelicMgr owner)
        {
            this.owner = owner;
        }
        void IModelView<RelicMgr>.detach(RelicMgr owner)
        {
            if(this.owner !=null)
            {
                this.owner = null;
            }

            Destroy(gameObject);
        }
        void IRelicMgrView.notify_add_rellic(Relic relic)
        {
            var r_obj = Instantiate(prefab, content, false);
            r_obj.Init(relic);

            r_obj.gameObject.SetActive(true);
            relic_views.Add(r_obj);
        }
        void IRelicMgrView.notify_remove_rellic(Relic relic)
        {
            for(int i = 0; i < relic_views.Count; i++)
            {
                if (relic_views[i].data == relic)
                {
                    Destroy(relic_views[i].gameObject);
                    relic_views.RemoveAt(i);
                    break;
                }
            }
        }

        void IRelicMgrView.notify_init()
        {
            foreach(var rv in relic_views)
            {
                Destroy(rv.gameObject);
            }

            relic_views.Clear();

            foreach (var relic in owner.relic_list)
            {
                var r_obj = Instantiate(prefab, content, false);
                r_obj.Init(relic);
                r_obj.gameObject.SetActive(true);
                relic_views.Add(r_obj);
            }
        }

        public void Init()
        {
            owner.Init();
        }
    }
}
