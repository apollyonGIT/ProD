using Foundations.MVVM;
using System.Collections.Generic;
using World.Helpers;

namespace World.Work
{
    public interface IWorkView : IModelView<Work>
    {
        public void init(Work work);
        public void tick();
        public void notify_add_job(Job job);
        public void notify_remove_job(Job job);
        public void notify_character_work(Job job, Cubicle cubicle,string sprite_path);
        public void notify_device_destroy();
        public void notify_device_repair();
    }
    public class Work : Model<Work, IWorkView>
    {
        public List<Job> jobs = new List<Job>();

        public void init()
        {
            foreach (Job job in jobs)
            {
                job.Init();
            }
            foreach (var view in views)
            {
                view.init(this);
            }
        }
        public void tick()
        {
            foreach (var job in jobs)
            {
                job.tick();
            }

            for (int i = jobs.Count - 1; i >= 0; i--)
            {
                if (jobs[i].need_remove)
                {
                    var job = jobs[i];
                    jobs.RemoveAt(i);

                    foreach (var cub in job.cubicles)
                    {
                        Cubicle_Info_Helper.EmptyCubicle(cub);
                    }

                    foreach (var view in views)
                    {
                        view.notify_remove_job(job);
                    }
                }
            }

            foreach (var view in views)
            {
                view.tick();
            }
        }
        public void AddJob(Job job)
        {
            jobs.Add(job);
            job.owner = this;

            foreach (var view in views)
            {
                view.notify_add_job(job);
            }
        }
        public void RemoveJob(Job job)
        {
            job.need_remove = true;
        }
        public void ChangeCubicle(Job job,Cubicle cubicle,string s)
        {
            foreach(var view in views)
                view.notify_character_work(job, cubicle, s);
        }
        public void DeviceDestroy()
        {
            foreach(var view in views)
            {
                view.notify_device_destroy();
            }
        }
        public void DeviceRepair()
        {
            foreach (var view in views)
            {
                view.notify_device_repair();
            }
        }
        public void WorkRemove()
        {
            foreach(var job in jobs)
            {
                foreach(var cub in job.cubicles)
                {
                    Cubicle_Info_Helper.EmptyCubicle(cub);
                }
            }
        }
    }
}
