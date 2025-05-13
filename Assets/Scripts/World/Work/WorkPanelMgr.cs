using Foundations.MVVM;
using System.Collections.Generic;

namespace World.Work
{
    public interface IWorkPanelView : IModelView<WorkPanelMgr>
    {
        void tick();
        void notify_update_panel(string pos_type,string path,bool b,Work work);
        void notify_able_panel(string pos_type, bool b);
    }
    public class WorkPanelMgr : Model<WorkPanelMgr, IWorkPanelView>
    {
        public Dictionary<string,string> panels_dic = new Dictionary<string,string>();
        public void UpdatePanel(string pos_type, string path, bool b,Work work)
        {
            if(b == false)
            {
                panels_dic.Remove(pos_type);
            }
            else
            {
                if (panels_dic.ContainsKey(pos_type))
                {
                    if (panels_dic[pos_type] == path)
                        return;
                    panels_dic[pos_type] = path;
                }
                else
                {
                    panels_dic.Add(pos_type, path);
                }
            }
            foreach(var view in views)
            {
                view.notify_update_panel(pos_type, path, b,work);
            }
        }


        public void AblePanel(string pos_type,bool b)
        {
            foreach (var view in views)
            {
                view.notify_able_panel(pos_type,b);
            }
        }

        public void tick()
        {
            foreach (var view in views)
            {
                view.tick();
            }
        }

    }
}
