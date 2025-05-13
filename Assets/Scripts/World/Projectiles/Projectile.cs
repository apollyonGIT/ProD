using AutoCodes;
using Commons;
using Foundations;
using System;
using System.Collections.Generic;
using UnityEngine;
using World.Devices;
using World.Enemys;
using World.Helpers;
using World.VFXs;
using static World.WorldEnum;

namespace World.Projectiles
{
    public enum MovementStatus
    {
        normal,
        in_object,
        in_ground,
    }


    public class Projectile : ITarget
    {
        public projectile desc;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 direction;

        public float mass;
        public int life_ticks;
        protected int life_ticks_init;
        public float radius;
        public Attack_Data attack_data;
        public MovementStatus movement_status = MovementStatus.normal;
        public Faction faction;

        public bool validate = true;

        public ITarget emitter = null; // 发射者
        public ITarget in_target = null;
        protected Vector2 pos_offset_in_object = Vector2.zero;
        protected Vector2 direction_in_object = Vector2.zero;

        public ITarget last_hit = null;

        protected float init_speed;  // For Caculating Actual Damage When Hit Target
        protected float rot_speed;
        protected float rot_propulsion;
        protected Vector2 last_position;

        public Action<ITarget> hit_target_event;

        Vector2 ITarget.Position => position;

        Faction ITarget.Faction => faction;

        float ITarget.Mass => mass;

        bool ITarget.is_interactive => true;

        int ITarget.hp => validate ? 1 : 0;

        Vector2 ITarget.acc_attacher { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        Vector2 ITarget.direction => direction;

        Vector2 ITarget.velocity => velocity;

        private const float VELOCITY_IN_GROUND_REMAINING_PER_TICK = 0.2f;
        private const float VALIDATE_DISTANCE = 80F;                //当飞射物距离车距离超过一定值 判定为飞射物没有维护的价值


        public virtual void Init(projectile _desc,
            Vector2 dir, Vector2 position, Vector2 shooter_velocity,
            float rnd_angle_1, float rnd_angle_2, float speed, float init_speed,
            Faction f, int life_ticks, ITarget emitter, Attack_Data atk_data, float rot_speed, Action<ITarget> hit_event = null)
        {
            desc = _desc;
            mass = desc.mass;
            this.life_ticks = life_ticks;
            life_ticks_init = life_ticks;

            this.emitter = emitter;
            radius = desc.radius;
            faction = f;
            attack_data = atk_data;
            this.rot_speed = rot_speed;
            rot_propulsion = (UnityEngine.Random.value + UnityEngine.Random.value - 1f) * desc.propulsion_error;  // 取两次value相加是为了改变概率密度，不能改成乘2
            hit_target_event = hit_event;

            var rnd = UnityEngine.Random.Range(rnd_angle_1, rnd_angle_2);
            direction = (Quaternion.AngleAxis(rnd, Vector3.forward) * dir).normalized;
            velocity = direction * speed + shooter_velocity;

            this.init_speed = init_speed;

            this.position = position;
        }

        public virtual void tick()
        {
            switch (movement_status)
            {
                case MovementStatus.normal:
                    var acc_x = -velocity.x * desc.k_feedback / desc.mass;
                    var acc_y = Config.current.gravity;
                    Vector2 ammo_acc = new Vector2(acc_x, acc_y);
                    if (desc.propulsion_force > 0)
                    {
                        ammo_acc += direction.normalized * desc.propulsion_force / desc.mass;
                        rot_speed += rot_propulsion * Config.PHYSICS_TICK_DELTA_TIME;
                    }

                    velocity += ammo_acc * Config.PHYSICS_TICK_DELTA_TIME;
                    position += velocity * Config.PHYSICS_TICK_DELTA_TIME;  // Move
                    rotate(); // Rotate

                    // Check If Hit Ground
                    var road_height = Road_Info_Helper.try_get_altitude(position.x);
                    if (position.y <= road_height)
                        if (desc.exploded_by_lifetime[1])
                            projectile_explode();
                        else
                            HitGround(road_height);

                    switch (desc.detection_type)
                    {
                        case "Radius":
                            if (check_and_select_single_target(out var target_single))
                                if (desc.exploded_by_lifetime[2])
                                    projectile_explode();
                                else
                                    hit_enemy_check(target_single);
                            break;
                        case "Ray":
                            if (check_and_select_targets_collection(out var targets_collection))
                                if (desc.exploded_by_lifetime[2])
                                    projectile_explode();
                                else
                                    foreach (var t in targets_collection)
                                    {
                                        if (faction == t.Faction)  // 命中盾牌有可能会改变飞射物阵营
                                            continue;
                                        hit_enemy_check(t);
                                    }
                            break;
                        default:
                            break;
                    }

                    break;
                case MovementStatus.in_object:
                    movement_in_object();
                    if (in_target.hp <= 0)
                        RemoveSelf();
                    break;
                case MovementStatus.in_ground:
                    if (velocity.magnitude < 0.1f)
                        break;
                    velocity *= VELOCITY_IN_GROUND_REMAINING_PER_TICK;
                    position += velocity * Config.PHYSICS_TICK_DELTA_TIME;  // Move
                    rotate(); // Rotate

                    break;
            }

            if (--life_ticks <= 0)
            {
                if (desc.exploded_by_lifetime[0])
                    projectile_explode();
                RemoveSelf();
            }

            var dis = (WorldContext.instance.caravan_pos - position).magnitude;
            if (dis > VALIDATE_DISTANCE)
                RemoveSelf();
        }

        public virtual void tick1()
        {
            // Nothing for now
        }

        protected void movement_status_change_to(MovementStatus expected_movement_status)
        {
            movement_status = expected_movement_status;
            switch (expected_movement_status)
            {
                case MovementStatus.in_object:
                    // temp: Arrows should not stick in device or carbody
                    if (!(in_target is Enemys.Enemy))
                        RemoveSelf();

                    break;
            }
        }
        protected Attack_Data modify_attack_data(ITarget target)
        {
            Attack_Data result = new Attack_Data();
            var target_v = Vector2.zero;
            if (target is Enemy e)
                target_v = e.velocity;

            if (init_speed == 0)
                result.atk = attack_data.atk;
            else
            {
                var v_ration = (velocity - target_v).magnitude / init_speed;
                var coef = Mathf.Lerp(1, v_ration, desc.speed_dmg_mod);
                result.atk = (int)(attack_data.atk * Mathf.Pow(coef, 2));
            }
            result.critical_chance = attack_data.critical_chance + BattleContext.instance.critical_chance_delta;
            result.critical_rate = attack_data.critical_rate + BattleContext.instance.global_critical_rate;
            result.ignite = attack_data.ignite;

            return result;
        }

        private void projectile_explode()
        {
            //生成逻辑爆炸
            var targets = BattleUtility.select_all_target_in_circle(position, desc.explode_radius, faction);
            foreach (var t in targets)
            {
                if (t != null && t.is_interactive)
                {
                    Attack_Data attack_data = new()
                    {
                        atk = desc.explode_dmg,
                        ignite = desc.explode_ignite,
                    };
                    t.hurt(this, attack_data, out var dmg_data);
                    if (emitter is Device d)
                    {
                        BattleContext.instance.ChangeDmg(d, dmg_data.dmg);
                        if (t.hp <= 0)
                        {
                            d.kill_enemy_action?.Invoke(t);
                        }
                    }
                    t.impact(impact_source_type.melee, position, BattleUtility.get_target_colllider_pos(t), desc.explode_ft);
                }
            }
            //生成特效 仅外观
            Mission.instance.try_get_mgr("VFX", out VFXMgr vmgr);
            vmgr.AddVFX(desc.explode_vfx, Config.PHYSICS_TICKS_PER_SECOND, position);
            Audio.AudioSystem.instance.PlayOneShot(desc.explode_se);
            RemoveSelf();
        }

        protected virtual void rotate()
        {
            direction = Quaternion.AngleAxis(rot_speed * Config.PHYSICS_TICK_DELTA_TIME, Vector3.forward) * direction;
        }


        //--------------------------------------------------------------------------------
        protected virtual bool check_and_select_single_target(out ITarget target_selected, bool same_last_target = false)
        {
            target_selected = BattleUtility.select_target_in_circle(position, radius, faction,
                (ITarget t) => (t != last_hit) ^ same_last_target && t.is_interactive);
            return target_selected != null;
        }

        protected virtual bool check_and_select_targets_collection(out List<ITarget> targets_selected, bool same_last_target = false)
        {
            var size_factor = BattleContext.instance.projectile_scale_factor * 0.001F;
            var axis = (position - last_position).normalized;
            var bv = new Vector2(1 / axis.x, -1 / axis.y).normalized;
            if (axis.x == 0)
            {
                bv = new Vector2(1, 0);
            }
            if (axis.y == 0)
            {
                bv = new Vector2(0, 1);
            }

            var p1 = last_position + bv * radius * size_factor;
            var p2 = position - bv * radius * size_factor;

            targets_selected = BattleUtility.select_all_target_in_rect(p1, p2, faction, (ITarget t) =>
            {
                return t != last_hit;
            });
            return targets_selected.Count > 0;
        }
        //--------------------------------------------------------------------------------


        protected virtual void movement_in_object()
        {

        }


        private void hit_enemy_check(ITarget target_hit)
        {
            var ts = target_hit as Devices.Device_AI.IShield;

            // Countered by Shield
            if (ts != null)
                if (ts.Try_Rebound_Projectile(this, velocity))
                    return;

            HitEnemy(target_hit);
        }

        public virtual void HitEnemy(ITarget target_hit)
        {

        }

        public virtual void HitGround(float road_height)
        {

        }




        public virtual void RemoveSelf()
        {
            validate = false;
        }
        public virtual void ResetPos()
        {
            position -= new Vector2(WorldContext.instance.reset_dis, 0);
        }

        public virtual void ResetProjectile(Vector2 vel, Vector2 dir, Faction f, MovementStatus movement_status)
        {
            life_ticks = life_ticks_init;
            velocity = vel;
            direction = dir;
            faction = f;
            this.movement_status = movement_status;
        }

        #region ITargetFunc
        void ITarget.impact(params object[] prms)
        {

        }
        void ITarget.attach_data(params object[] prms)
        {

        }
        void ITarget.detach_data(params object[] prms)
        {

        }
        void ITarget.hurt(ITarget harm_source, Attack_Data attack_data, out Dmg_Data dmg_data)
        {
            dmg_data = default;
        }
        float ITarget.distance(ITarget target)
        {
            return Vector2.Distance(position, target.Position);
        }
        void ITarget.tick_handle(System.Action<ITarget> outter_request, params object[] prms)
        {

        }
        #endregion
    }
}
