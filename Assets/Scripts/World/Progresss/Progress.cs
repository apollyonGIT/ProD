using Addrs;
using AutoCodes;
using Commons;
using Foundations;
using Foundations.MVVM;
using System.Collections.Generic;
using UnityEngine;
using World.Devices.Device_AI;
using World.Environments;
using World.Helpers;

namespace World.Progresss
{
    public interface IProgressView : IModelView<Progress>
    {

        void notify_init();
        void notify_on_tick();
        /// <summary>
        /// 提醒玩家 已经接近奇遇了
        /// </summary>
        /// <param name="p"></param>
        /// <param name="b"></param>
        void notify_notice_encounter(float p, bool b);

        void notify_add_progress_event(ProgressEvent pe);

        void notify_remove_progress_event(ProgressEvent pe);
    }

    public struct single_plot
    {
        public uint event_id;
        public float trigger_progress;      //触发的位置
        public bool ui_visible;
    }


    public class Progress : Model<Progress, IProgressView>
    {
        WorldContext ctx;

        //==================================================================================================

        public float total_progress => m_total_progress;
        public float m_total_progress;

        public float current_progress
        {
            get
            {
                return m_current_progress;
            }
            set
            {
                m_current_progress = Mathf.Clamp(value, 0f, m_total_progress);
            }
        }
        private float m_current_progress;


        public List<single_plot> single_plots = new();      //记录的是随机后生成的奇遇们的基础数据（未触发）


        public List<ProgressEvent> progress_events = new(); //记录的是所有生成的progress 但不一定触发了

        public void tick()
        {
            if (ctx.is_need_reset)
            {
                foreach (var pe in progress_events)
                {
                    pe.pos -= new Vector2(ctx.reset_dis, 0);
                }
            }

            foreach (var view in views)
            {
                view.notify_on_tick();
            }

            for (int i = progress_events.Count - 1; i >= 0; i--)
            {
                progress_events[i].tick();

                if (progress_events[i].need_remove == true)
                {
                    foreach (var view in views)
                    {
                        view.notify_remove_progress_event(progress_events[i]);
                    }

                    progress_events.RemoveAt(i);
                    trigger_next_event();
                }
            }

            //录入当前进度
            ctx.scene_remain_progress = m_total_progress - current_progress;
        }


        public void tick1()
        {

        }

        public void Init(uint map_id)
        {
            ctx = WorldContext.instance;

            var dis = 0f;

            List<plot> plots_list = new();

            foreach(var plot_record in plots.records)
            {
                if (plot_record.Value.group_id == map_id)
                {
                    plots_list.Add(plot_record.Value);  
                }
            }

            plots_list.Sort((a, b) => a.site_order.CompareTo(b.site_order));

            foreach(var p in plots_list)
            {
                for(int i = 0; i < p.event_count; i++)
                {
                    dis += Random.Range(p.event_interval.Item1, p.event_interval.Item2);
                    var event_id_index = Random.Range(0, p.event_list.Count);
                    single_plots.Add(new single_plot()
                    {
                        event_id = p.event_list[event_id_index],
                        trigger_progress = dis,
                        ui_visible = p.ui_visible
                    });
                }
            }

            dis += Config.current.security_zone_distance;

            m_total_progress = dis;

            current_progress = 0f;
            //剩余进度
            ctx.scene_remain_progress = m_total_progress;

            trigger_next_event();

            foreach(var view in views)
            {
                view.notify_init();
            }
        }

        public void Move(float dis)
        {
            m_current_progress += dis;
            foreach (var view in views)
            {
                view.notify_notice_encounter(0, false);
            }

            foreach (var pe in progress_events)
            {
                if (m_current_progress > pe.trigger_progress - Config.current.notice_length_1 && m_current_progress < pe.trigger_progress - Config.current.notice_length_2)
                {
                    foreach (var view in views)
                    {
                        view.notify_notice_encounter(pe.trigger_progress, true);
                    }
                    var x = pe.pos.x;
                    Road_Info_Helper.try_get_altitude(x, out var altitude);
                    pe.pos.y = altitude + 2;
                }

                if (m_current_progress > pe.trigger_progress - Config.current.trigger_length && m_current_progress < pe.trigger_progress + Config.current.trigger_length && pe.notice_triggered == false)
                {
                    pe.Enter();
                }
                else if (m_current_progress > pe.trigger_progress + Config.current.trigger_length)
                {
                    pe.Exit();
                }

            }
        }

        private void trigger_next_event()
        {
            if (single_plots.Count > 0)
            {
                var event_id = single_plots[0].event_id;

                event_sites.TryGetValue(event_id.ToString(), out var event_site_record);

                Mission.instance.try_get_mgr(Config.EnvironmentMgr_Name, out EnvironmentMgr emgr);
                var dis = single_plots[0].trigger_progress - current_progress;
                if (event_site_record.prefeb != null)
                {
                    Debug.Log(event_site_record.prefeb);
                    Addressable_Utility.try_load_asset(event_site_record.prefeb, out EncounterObjects objs);            //奇遇这个为纯外观 不带一丝逻辑
                    if (objs != null)
                    {
                        emgr.AddEncounterObj(objs, new Vector2(WorldContext.instance.caravan_pos.x + dis, 0));
                    }
                }

                var pos = new Vector2(WorldContext.instance.caravan_pos.x + dis, 3);
                var pe = new ProgressEvent(current_progress + dis, event_site_record, pos);
                progress_events.Add(pe);
                ProgressModule pm = new ProgressModule()
                {
                    pe = pe,
                };
                pe.module = pm;
                foreach (var view in views)
                {
                    view.notify_add_progress_event(pe);
                }

                single_plots.RemoveAt(0);
            }

            return;
        }

        /*private void trigger_event(uint event_id)
        {
            event_sites.TryGetValue(event_id.ToString(), out var event_site_record);

            Mission.instance.try_get_mgr(Config.EnvironmentMgr_Name, out EnvironmentMgr emgr);
            if (event_site_record.prefeb != null)
            {
                Debug.Log(event_site_record.prefeb);
                Addressable_Utility.try_load_asset(event_site_record.prefeb, out EncounterObjects objs);            //奇遇这个为纯外观 不带一丝逻辑
                if (objs != null)
                {
                    emgr.AddEncounterObj(objs, new Vector2(WorldContext.instance.caravan_pos.x + event_site_record.distance, 0));
                }
            }

            var pos = new Vector2(WorldContext.instance.caravan_pos.x + event_site_record.distance, 3);
            var pe = new ProgressEvent(current_progress + event_site_record.distance, event_site_record, pos);
            progress_events.Add(pe);
            ProgressModule pm = new ProgressModule()
            {
                pe = pe,
            };
            pe.module = pm;
            foreach (var view in views)
            {
                view.notify_add_progress_event(pe);
            }
        }*/
    }
}





