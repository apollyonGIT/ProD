using Commons;
using Foundations;
using Foundations.MVVM;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using World.Helpers;
using World.Widgets;

namespace World.Caravans
{
    public interface ICaravanView : IModelView<Caravan>
    {
        void notify_on_tick1();

        void notify_on_hurt();

        void notify_on_tick();

        Skeleton sk { get; }
    }


    public class Caravan : Model<Caravan, ICaravanView>, ITarget
    {
        public Vector3 view_pos => calc_view_pos();
        public Quaternion view_rotation => calc_view_rotation();

        Action<ITarget> m_tick_handle;

        Vector2 ITarget.Position => mgr.ctx.caravan_pos;

        WorldEnum.Faction ITarget.Faction => faction;
        public WorldEnum.Faction faction = WorldEnum.Faction.player;

        float ITarget.Mass => WorldContext.instance.total_weight;

        bool ITarget.is_interactive => true;

        int ITarget.hp => mgr.ctx.caravan_hp;

        Vector2 ITarget.acc_attacher { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        Vector2 ITarget.direction => WorldContext.instance.caravan_dir;

        public AutoCodes.spine_caravan anim_info => calc_anim_info();

        Vector2 ITarget.velocity => WorldContext.instance.caravan_velocity;

        public AutoCodes.caravan_body _desc;

        CaravanMgr mgr;

        //==================================================================================================

        public Caravan(string key)
        {
            Mission.instance.try_get_mgr("CaravanMgr", out mgr);
            var ctx = mgr.ctx;

            AutoCodes.caravan_bodys.TryGetValue($"{key},0", out _desc);
            ctx.total_weight = _desc.weight;
            ctx.caravan_hp = _desc.hp;
            ctx.caravan_hp_max = _desc.hp;
        }


        public void tick()
        {
            m_tick_handle?.Invoke(this);
            m_tick_handle = null;

            foreach(var view in views)
            {
                view.notify_on_tick();
            }
        }


        public void tick1()
        {
            foreach (var view in views)
            {
                view.notify_on_tick1();
            }
        }


        Vector3 calc_view_pos()
        {
            return mgr.ctx.caravan_pos;
        }


        Quaternion calc_view_rotation()
        {
            if (mgr.ctx == null) return Quaternion.identity;

            return EX_Utility.look_rotation_from_left(mgr.ctx.caravan_dir);
        }


        public void load_caravan_slots(CaravanView view)
        {    
            load_caravan_slots(view, out var slot_info);

            ref var main_caravan_slots = ref mgr.ctx.caravan_slots;
            if (main_caravan_slots == null)
                main_caravan_slots = new Dictionary<string, Vector3>(slot_info);
        }


        public static void load_caravan_slots(CaravanView view, out Dictionary<string, Vector3> dic)
        {
            dic = new();

            var slots = view.gameObject.GetComponent<CaravanSlots>();
            foreach (var slot in slots.slots)
            {
                dic.Add(slot.slot_name, slot.transform.localPosition);
            }
        }


        float ITarget.distance(ITarget target)
        {
            return Vector2.Distance(mgr.ctx.caravan_pos, target.Position);
        }


        void ITarget.hurt(ITarget harm_source, Attack_Data attack_data, out Dmg_Data dmg_data)
        {
            ref var hp = ref mgr.ctx.caravan_hp;
            dmg_data = Hurt_Calc_Helper.dmg_to_caravan(attack_data, $"{_desc.id},{_desc.rank}");
            var dmg = dmg_data.dmg;

            hp -= dmg;
            UnityEngine.Debug.Log($"收到伤害: {dmg} hp剩余: {hp} ");
            if (hp < 0)
                hp = 0;

            foreach (var view in views)
            {
                view.notify_on_hurt();
            }
        }


        void ITarget.impact(params object[] prms)
        {
            
        }


        void ITarget.attach_data(params object[] prms)
        {
            
        }


        void ITarget.detach_data(params object[] prms)
        {
            
        }


        void ITarget.tick_handle(Action<ITarget> outter_request, params object[] prms)
        {
            m_tick_handle += outter_request;
        }


        private AutoCodes.spine_caravan calc_anim_info()
        {
            AutoCodes.spine_caravans.TryGetValue($"{_desc.id},0,{CaravanMover.move_type}", out var r_spine);

            return r_spine;
        }

        public void Brake()
        {
            Widget_DrivingLever_Context.instance.SetLever(0, true);
            WorldContext.instance.caravan_status_acc = WorldEnum.EN_caravan_status_acc.braking;
        }

        public void Fix()
        {
            var wctx = WorldContext.instance;

            wctx.caravan_hp += (int)(wctx.caravan_hp_max * Config.current.fix_caravan_job_effect);
            wctx.caravan_hp = Mathf.Min(wctx.caravan_hp, wctx.caravan_hp_max);
        }

    }
}





