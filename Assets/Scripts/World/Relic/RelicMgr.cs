using Addrs;
using AutoCodes;
using Foundations;
using Foundations.MVVM;
using Foundations.Tickers;
using System;
using System.Collections.Generic;
using UnityEngine;
using World.Relic.Relics;
using World.Relic.RelicStores;

namespace World.Relic
{
    public interface IRelicMgrView : IModelView<RelicMgr>
    {
        void notify_init();
        void notify_add_rellic(Relic relic);
        void notify_remove_rellic(Relic relic);
    }
    public class RelicMgr : Model<RelicMgr, IRelicMgrView>, IMgr
    {
        private Dictionary<string, Func<Relic>> relic_dic = new() {
            { "AddRepeatRelic",()=> new AddRepeatRelic()},
            { "ZoomMeleeRelic",()=> new ZoomMeleeRelic()},
            { "AccelerateRelic",()=> new AccelerateRelic()},
            { "TitanEnemyRelic",()=> new TitanEnemyRelic()},
            { "DifficultModeRelic",()=> new DifficultModeRelic()},
            { "LuckyDogRelic",()=> new LuckyDogRelic()},

            { "Relic_Global_Critical_Rate",() => new Relic_Global_Critical_Rate()},
            { "Relic_Ranged_Salvo",()=> new Relic_Ranged_Salvo()},
            { "Relic_Ranged_Ammo_Bigger",() => new Relic_Ranged_Ammo_Bigger() },
            { "Relic_Modify_Variable",() => new Relic_Modify_Variable()},
        };
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;
        int IMgr.priority => m_mgr_priority;
        readonly int m_mgr_priority;

        public List<Relic> relic_list = new();

        public RelicStore relic_store;

        public RelicMgr(string mgr_name, int mgr_priority, params object[] args)
        {
            m_mgr_name = mgr_name;
            m_mgr_priority = mgr_priority;

            (this as IMgr).init(args);
        }

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
        void tick()
        {

        }

        void tick1()
        {

        }

        private Relic create_relic(string id)
        {
            if (relic_dic.ContainsKey(id))
            {
                return relic_dic[id]();
            }

            UnityEngine.Debug.LogWarning($"错误遗物类型 {id}");
            return new Relic();
        }

        private List<Relic> GetRndRelics(List<string> relics_id, int num)
        {
            List<Relic> temp = new();
            for (int i = 0; i < num; i++)
            {
                if (relics_id.Count == 0)
                    break;
                var index = UnityEngine.Random.Range(0, relics_id.Count);
                relics.records.TryGetValue(relics_id[index], out var relic_value);
                var r = create_relic(relic_value.behaviour_script);
                r.desc = relic_value;
                temp.Add(r);
                relics_id.RemoveAt(index);
            }
            return temp;
        }

        public void Init()
        {
            foreach(var view in views)
            {
                view.notify_init();
            }
        }

        public void InstantiateRelicStore(List<string> relics_id)
        {
            List<Relic> temp = GetRndRelics(relics_id, 3);
            relic_store = new RelicStore();
            Addressable_Utility.try_load_asset<RelicStoreView>("rsv", out var rsv_view);
            var view = GameObject.Instantiate(rsv_view, WorldSceneRoot.instance.uiRoot.transform, false);
            relic_store.add_view(view);
            relic_store.Init(temp);
        }

        public void ClearRelicStore()
        {
            relic_store.remove_all_views();
            relic_store = null;
        }

        public void AddRelic(Relic relic)
        {
            relic_list.Add(relic);
            relic.Get();
            foreach (var view in views)
            {
                view.notify_add_rellic(relic);
            }
        }

        public void AddRelic(string id)
        {
            relics.records.TryGetValue(id, out var relic_value);
            if (relic_value != null)
            {
                var relic = create_relic(relic_value.behaviour_script);
                relic.desc = relic_value;

                AddRelic(relic);
            }
        }
        public void RemoveRelic(Relic relic)
        {
            relic_list.Remove(relic);
            relic.Drop();
            foreach (var view in views)
            {
                view.notify_remove_rellic(relic);
            }
        }
    }
}
