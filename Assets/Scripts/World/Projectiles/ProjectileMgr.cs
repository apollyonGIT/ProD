using Foundations;
using Foundations.MVVM;
using Foundations.Tickers;
using System.Collections.Generic;


namespace World.Projectiles
{
    public interface IProjectileMgrView : IModelView<ProjectileMgr>
    {
        public void notify_reset_projectile();
        public void notify_add_projectile(Projectile p);
        public void notify_remove_projectile(Projectile p);
        public void tick();
        public void tick1();
        public void init();
    }

    public class ProjectileMgr : Model<ProjectileMgr, IProjectileMgrView>, IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        int IMgr.priority => m_mgr_priority;
        readonly int m_mgr_priority;

        //==================================================================================================

        public ProjectileMgr(string name, int priority, params object[] args)
        {
            m_mgr_name = name;
            m_mgr_priority = priority;

            (this as IMgr).init(args);
        }
        void IMgr.init(params object[] args)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);

            var ticker = Ticker.instance;
            ticker.add_tick(m_mgr_priority, m_mgr_name, tick);
            ticker.add_tick1(m_mgr_priority, m_mgr_name, tick1);
        }
        void IMgr.fini()
        {
            Mission.instance.detach_mgr(m_mgr_name);

            var ticker = Ticker.instance;
            ticker.remove_tick(m_mgr_name);
            ticker.remove_tick1(m_mgr_name);
        }
        void tick()
        {
            if (WorldContext.instance.is_need_reset)
            {
                foreach(var p in projectiles)
                {
                    p.ResetPos();
                }

                foreach(var view in views)
                {
                    view.notify_reset_projectile();
                }
            }
            foreach (var p in projectiles)
            {
                p.tick();
            }

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (projectiles[i].validate == false)
                {
                    RemoveProjectile(projectiles[i]);
                }
            }
            


            foreach (var view in views)
            {
                view.tick();
            }
        }
        void tick1()
        {
            foreach (var p in projectiles)
            {
                p.tick1();
            }
            foreach (var view in views)
            {
                view.tick1();
            }
        }

        public List<Projectile> projectiles = new List<Projectile>();

        public void AddProjectile(Projectile p)
        {
            projectiles.Add(p);

            foreach (var view in views)
            {
                view.notify_add_projectile(p);
            }
        }

        public void RemoveProjectile(Projectile p)
        {
            projectiles.Remove(p);

            foreach (var view in views)
            {
                view.notify_remove_projectile(p);
            }
        }


        public void Init()
        {
            foreach (var view in views)
            {
                view.init();
            }
        }
    }
}