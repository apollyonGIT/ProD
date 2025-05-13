using Foundations.MVVM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace World.Work
{
    public class WorkMgrView : MonoBehaviour, IWorkMgrView
    {
        public WorkMgr owner;

        public List<WorkView> workview_list = new();
        public WorkView prefab;
        public Transform content;

        void IWorkMgrView.add_work(Work work)
        {
            var wv = Instantiate(prefab, content, false);
            wv.gameObject.SetActive(true);
            work.add_view(wv);
            wv.init(work);

            workview_list.Add(wv);
        }

        void IModelView<WorkMgr>.attach(WorkMgr owner)
        {
            this.owner = owner;
        }

        void IModelView<WorkMgr>.detach(WorkMgr owner)
        {
            this.owner = null;
            Destroy(gameObject);
        }

        void IWorkMgrView.remove_work(Work work)
        {
            for(int i = workview_list.Count - 1; i >= 0; i--)
            {
                if (workview_list[i].owner == work)
                {
                    Destroy(workview_list[i].gameObject);
                    workview_list.RemoveAt(i);
                    break;
                }
            }
        }

        void IWorkMgrView.sort_work()
        {
            var start_pos = new Vector2(0,0);
            for (int i = 0; i < workview_list.Count - 1; i++)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(workview_list[i].GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(workview_list[i+1].GetComponent<RectTransform>());

                var rect = workview_list[i].GetComponent<RectTransform>();

                //更新Y 根据当前rect的高度
                start_pos = new Vector2(start_pos.x,0 + rect.rect.height / 2);
                rect.anchoredPosition = start_pos;

                //更新X 此处y更新什么都无所谓
                start_pos =  new Vector2(start_pos.x +rect.rect.width / 2 + workview_list[i+1].GetComponent<RectTransform>().rect.width/2,start_pos.y);
            }
            if(workview_list.Count >= 1)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(workview_list[workview_list.Count - 1].GetComponent<RectTransform>());
                workview_list[workview_list.Count - 1].GetComponent<RectTransform>().anchoredPosition = start_pos;
            }
        }
    }
}
