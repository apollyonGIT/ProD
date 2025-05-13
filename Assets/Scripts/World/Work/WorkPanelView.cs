using Addrs;
using Foundations.MVVM;
using System.Collections.Generic;
using UnityEngine;
using World.Work.panels;

namespace World.Work
{
    public class WorkPanelView : MonoBehaviour, IWorkPanelView
    {
        WorkPanelMgr owner;
        public Dictionary<string, WorkControlPanel> panels = new();

        public Transform r1, r2, t1, l1, l2,w1;
        public Transform panel;
        void IModelView<WorkPanelMgr>.attach(WorkPanelMgr owner)
        {
            this.owner = owner;
        }

        void IModelView<WorkPanelMgr>.detach(WorkPanelMgr owner)
        {
            if (this.owner != null)
            {
                owner = null;
            }
            Destroy(gameObject);
        }

        void IWorkPanelView.notify_update_panel(string pos_type, string path, bool b,Work work)
        {
            if (!b)
            {
                if(panels.TryGetValue(pos_type,out var cp))
                {
                    cp.close();
                    Destroy(cp.gameObject);
                    panels.Remove(pos_type);
                }
            }
            else
            {
                Addressable_Utility.try_load_asset<WorkControlPanel>(path, out var g);
                if(g == null)
                {
                    Debug.LogError("Panel的路径有问题");
                    return;
                }
                var panel = Instantiate(g,panel_transform(pos_type),false);
                panel.transform.localPosition = Vector3.zero;
                panel.init(work);
                panel.open();
                panels.Add(pos_type,panel);
            }
            
        }

        private Transform panel_transform(string pos_type)
        {
            switch (pos_type)
            {
                case "device_slot_wheel":
                    return w1;
                case "slot_top":
                    return t1;
                case "slot_front":
                    return r2;
                case "slot_back":
                    return l2;
                case "slot_upper_front":
                    return r1;
                case "slot_upper_back":
                    return l1;
                default:
                    return t1;      //这里暂且这么设计
            }
        }

        void IWorkPanelView.tick()
        {
            foreach(var (_,panel) in panels)
            {
                panel.tick();
            }
        }

        void IWorkPanelView.notify_able_panel(string pos_type, bool b)
        {
            panels.TryGetValue(pos_type, out var panel);
            if (panel != null)
                panel.mask.SetActive(!b);
        }
    }
}
