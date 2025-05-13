using Commons;
using Foundations;
using Foundations.MVVM;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using World.Helpers;
using World.VFXs.Damage_PopUps;
using World.VFXs.Enemy_Death_VFXs;
using static World.WorldEnum;

namespace World.Enemys
{
    public interface IEnemy_BT
    {
        void init(Enemy cell, params object[] prms);

        void tick(Enemy cell);

        void notify_on_enter_die(Enemy cell);

        void notify_on_dying(Enemy cell);

        void notify_on_dead(Enemy cell);

        string state { get; }
    }


    public interface IEnemyView : IModelView<Enemy>
    {
        void notify_on_tick1();

        void notify_on_hurt();

        void notify_on_pre_aim(bool ret);

        void notify_on_aim(bool ret);

        void notify_on_outrange_aim(bool ret);
    }


    public class Enemy : Model<Enemy, IEnemyView>, ITarget
    {
        public string GUID;

        public Vector2 pos;
        public Vector2 dir;

        public Vector2 view_pos => calc_view_pos();
        public Quaternion view_rotation => calc_view_rotation();
        public float view_scaleX => calc_view_scaleX();
        public float view_scaleY => calc_view_scaleY();

        public Vector2 velocity;
        public Vector2 acc_attacher;

        public EnemyMgr mgr;
        public EnemyMover mover;
        public IEnemy_BT bt;

        public AutoCodes.monster _desc;
        public object _sub_desc;

        public string view_resource_name;

        public float speed_expt;
        public Vector2 position_expt;

        public float mass_self;
        public float mass_attachment;
        public float mass_total => (mass_self + mass_attachment) * battle_ctx.enemy_mass_factor;

        public int hp_self;
        public int hp_self_max;
        public int hp => Mathf.FloorToInt(hp_self * battle_ctx.enemy_hp_factor);
        public int hp_max => Mathf.FloorToInt(hp_self_max * battle_ctx.enemy_hp_factor);

        public bool is_alive = true;

        public bool is_fling_off = false;

        public Vector3 collider_pos;

        public ITarget target;

        string m_death_vfx => _desc.death_vfx[death_vfx_index];
        public int death_vfx_index;

        public Dictionary<string, Spine.Bone> bones = new();

        public Dictionary<string, object> anim_info => calc_anim_info();

        Action<ITarget> m_tick_handle;

        public AutoCodes.monster_group _group_desc;

        Vector2 ITarget.Position => collider_pos;

        Faction ITarget.Faction => Faction.opposite;

        float ITarget.Mass => mass_total;

        bool ITarget.is_interactive => is_alive;

        int ITarget.hp => hp;

        Vector2 ITarget.acc_attacher { get => acc_attacher; set => acc_attacher = value; }

        Vector2 ITarget.direction => dir;

        Vector2 ITarget.velocity => velocity;

        public bool is_init;

        public BattleContext battle_ctx;

        public Action<object[]> outter_tick;
        public Action<object[]> outter_fini;

        //==================================================================================================

        public Enemy(uint id)
        {
            GUID = Guid.NewGuid().ToString();

            battle_ctx = BattleContext.instance;

            Mission.instance.try_get_mgr("EnemyMgr", out mgr);
            AutoCodes.monsters.TryGetValue($"{id}", out _desc);

            mover = new(this);

            var monster_behaviour_tree = _desc.monster_behaviour_tree;
            if (!string.IsNullOrEmpty(monster_behaviour_tree))
            {
                bt = (IEnemy_BT)Activator.CreateInstance(Assembly.Load("World").GetType($"World.Enemys.BT.{monster_behaviour_tree}"));
            }

            mass_self = _desc.mass;
            hp_self = _desc.hp;
            hp_self_max = _desc.hp;

            view_resource_name = _desc.monster_view;

            #region 加载子表
            var sub_table_name = _desc.sub_table_name;
            if (!string.IsNullOrEmpty(sub_table_name))
            {
                var sub_table_key = calc_sub_table_key();
                var sub_table_type = Assembly.Load("AutoCodes").GetType($"AutoCodes.{sub_table_name}s");
                var sub_table_agrs = new object[] { sub_table_key, _sub_desc };

                sub_table_type.GetMethod("TryGetValue").Invoke(null, sub_table_agrs);
                _sub_desc = sub_table_agrs[1];
            }
            #endregion
        }


        protected virtual string calc_sub_table_key()
        {
            return _desc.sub_table_key;
        }


        public virtual void do_after_init(params object[] args)
        {
        }


        public virtual void tick()
        {
            if (!is_init) return;

            //甩脱检测
            var dis = mgr.ctx.caravan_pos.x - pos.x;
            if (!mgr.ctx.is_need_reset && dis >= Config.current.fling_off_dis)
            {
                hp_self = -1;
                is_fling_off = true;
            }

            //空血检测
            if (hp <= 0)
            {
                if (is_alive)
                {
                    is_alive = false;
                    bt.notify_on_enter_die(this);

                    //规则：非甩脱，怪物死亡增加击杀压力
                    if (!is_fling_off && _group_desc != null)
                    {
                        mgr.ctx.pressure += _group_desc.kill_pressure;
                    }

                    //规则：怪物死亡增加击杀分数
                    mgr.ctx.kill_score++;
                }
                else 
                {
                    bt.notify_on_dying(this);
                }

                return;
            }

            bt.tick(this);

            m_tick_handle?.Invoke(this);
            m_tick_handle = null;
        }


        public virtual void tick1()
        {
            foreach (var view in views)
            {
                view.notify_on_tick1();
            }
        }


        public virtual void fini()
        {
            mgr.fini_cells.AddLast(this);

            outter_fini?.Invoke(new object[] { this });

            //创建死亡特效
            if (m_death_vfx != "null" && Mission.instance.try_get_mgr("Enemy_Death_VFXMgr", out Enemy_Death_VFXMgr enemy_Death_VFXMgr))
                enemy_Death_VFXMgr.pd.create_cell(m_death_vfx, pos, 240);

            Helpers.Enemy_Fini_Helper.@do(this);
        }


        protected virtual Vector3 calc_view_pos()
        {
            return pos;
        }


        protected virtual Quaternion calc_view_rotation()
        {
            return EX_Utility.look_rotation_from_left(dir);
        }


        protected virtual float calc_view_scaleX()
        {
            return 1;
        }

        protected virtual float calc_view_scaleY()
        {
            return dir.x >= 0 ? 1 : -1;
        }


        protected virtual Dictionary<string, object> calc_anim_info()
        {
            AutoCodes.spine_monsters.TryGetValue($"{_desc.id},{bt.state}", out var r_spine);

            Dictionary<string, object> ret = new()
            {
                { "anim_name", r_spine.anim_name },
                { "loop", r_spine.loop }
            };

            return ret;
        }


        void ITarget.impact(params object[] prms)
        {
            mover.impact(prms);
        }


        void ITarget.hurt(ITarget harm_source, Attack_Data attack_data, out Dmg_Data dmg_data)
        {
            dmg_data = default;
            if (hp <= 0) return;

            dmg_data = calc_dmg(attack_data);
            var dmg = dmg_data.dmg;

            hp_self -= Mathf.FloorToInt(dmg * hp_self / hp);

            Damage_PopUp.instance.create_cell(dmg_data, pos);

            foreach (var view in views)
            {
                view.notify_on_hurt();
            }
        }


        protected virtual Dmg_Data calc_dmg(Attack_Data attack_data)
        {
            return Hurt_Calc_Helper.dmg_to_enemy(attack_data, $"{_desc.id}");
        }


        void ITarget.attach_data(params object[] prms)
        {
            var addition_mass = (float)prms[0];
            mass_attachment += addition_mass;
        }


        void ITarget.detach_data(params object[] prms)
        {
            var addition_mass = (float)prms[0];
            mass_attachment -= addition_mass;
        }


        float ITarget.distance(ITarget target)
        {
            return Vector2.Distance(pos, target.Position);
        }


        void ITarget.tick_handle(Action<ITarget> outter_request, params object[] prms)
        {
            m_tick_handle += outter_request;
        }


        public bool try_get_bone_pos(string bone_name, out Vector2 _pos)
        {
            _pos = default;

            if (!bones.TryGetValue(bone_name, out var bone))
            {
                Debug.LogError("怪物射击类，怪物骨骼未找到");
                return false;
            }

            bones.TryGetValue("root", out var root_bone);
            var offset = new Vector2(bone.WorldX, bone.WorldY) - new Vector2(root_bone.WorldX, root_bone.WorldY);

            if (dir.x > 0)
                _pos = pos + offset;
            else
                _pos = pos - offset;

            return true;
        }

        public void PreSelect(bool ret)
        {
            foreach(var view in views)
            {
                view.notify_on_pre_aim(ret);
            }
        }

        public void Select(bool ret)
        {
            foreach (var view in views)
            {
                view.notify_on_aim(ret);
            }
        }

        public void OutRangeSelect(bool ret)
        {
            foreach (var view in views)
            {
                view.notify_on_outrange_aim(ret);
            }
        }
    }
}





