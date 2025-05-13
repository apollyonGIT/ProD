using Foundations.MVVM;
using UnityEngine;

namespace World.Relic.RelicStores
{
    public class RelicStoreView : MonoBehaviour, IRelicStoreView
    {
        RelicStore owner;
        public RelicStoreSingleView prefab;
        public Transform content;

        void IModelView<RelicStore>.attach(RelicStore owner)
        {
            this.owner = owner;
        }

        void IModelView<RelicStore>.detach(RelicStore owner)
        {
            if (this.owner != null)
                this.owner = null;
            Destroy(gameObject);
        }

        void IRelicStoreView.init()
        {
            foreach(var relic in owner.relic_list)
            {
                var view = Instantiate(prefab, content);
                view.Init(relic);

                view.gameObject.SetActive(true);
            }
        }
    }
}
