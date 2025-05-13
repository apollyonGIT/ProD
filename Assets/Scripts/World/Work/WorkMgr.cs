using Foundations;
using Foundations.MVVM;
using Foundations.Tickers;
using System.Collections.Generic;

namespace World.Work
{
    public interface IWorkMgrView : IModelView<WorkMgr>
    {
        void add_work(Work work);
        void remove_work(Work work);

        void sort_work();
    }

    public class WorkMgr : Model<WorkMgr, IWorkMgrView>, IMgr
    {
        public WorkPD pd;
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;
        int IMgr.priority => m_mgr_priority;
        readonly int m_mgr_priority;

        void IMgr.fini()
        {
            Mission.instance.detach_mgr(m_mgr_name);

            var ticker = Ticker.instance;
            ticker.remove_tick(m_mgr_name);
            ticker.remove_tick1(m_mgr_name);
        }
        void IMgr.init(params object[] args)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);

            var ticker = Ticker.instance;
            ticker.add_tick(m_mgr_priority, m_mgr_name, tick);
            ticker.add_tick1(m_mgr_priority, m_mgr_name, tick1);
        }

        // 记录将要删除或者增加的work
        public List<(Work, bool)> work_willing = new();
        public List<Work> works = new List<Work>();
        public WorkPanelMgr workPanelMgr = new WorkPanelMgr();

        public WorkMgr(string name, int priority, params object[] args)
        {
            m_mgr_name = name;
            m_mgr_priority = priority;

            (this as IMgr).init(args);
        }
        public Work AddWork(List<uint> jobs_id)
        {
            var work = new Work();
            foreach (var job_id in jobs_id)
            {
                var job = WorkUtility.Id2Job(job_id);
                work.jobs.Add(job);
                job.owner = work;
            }
            work_willing.Add((work, true));

            return work;
        }
        public void RemoveWork(Work w)
        {
            foreach (var work in works)
            {
                if (w == work)
                {
                    work_willing.Add((w, false));
                    w.WorkRemove();
                }
            }
            //TBD
        }
        public void Init()
        {
            advance_willing();

            foreach(var view in views)
            {
                view.sort_work();
            }
        }
        void tick()
        {
            advance_willing();

            foreach (var work in works)
            {
                work.tick();
            }

            workPanelMgr.tick();
        }
        void tick1()
        {

        }


        private void advance_willing()
        {
            foreach (var (work, active) in work_willing)
            {
                if (active)
                {
                    works.Add(work);
                    foreach (var view in views)
                    {
                        view.add_work(work);
                    }
                }
                else
                {
                    for (int i = works.Count - 1; i >= 0; i--)
                    {
                        if (work == works[i])
                        {
                            works.RemoveAt(i);

                            foreach (var view in views)
                            {
                                view.remove_work(work);
                            }

                            break;
                        }
                    }
                }
            }

            work_willing.Clear();
        }
    }
}
