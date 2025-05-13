using Commons;
using UnityEngine;
using World.Devices;
using World.Helpers;

namespace World.Projectiles
{
    public class BulletProjectile : Projectile
    {
        public override void tick()
        {
            last_position = position;
            base.tick();
        }

        public override void HitEnemy(ITarget target_hit)
        {
            var ad = modify_attack_data(target_hit);

            target_hit.hurt(this, ad, out var dmg_data);
            if (emitter is Device d)
            {
                BattleContext.instance.ChangeDmg(d, dmg_data.dmg);
                if (target_hit.hp <= 0)
                {
                    d.kill_enemy_action?.Invoke(target_hit);
                }
            }
                
            target_hit.impact(WorldEnum.impact_source_type.projectile, velocity, mass, Config.current.bullet_penetration_loss);

            //2.根据剩余动能，判定飞射物自身的后续运动方式
            last_hit = target_hit;
            var ek_mul_2 = mass * Mathf.Pow(velocity.magnitude, 2);
            var delta_ek = ek_mul_2 - Config.current.bullet_penetration_loss * target_hit.Mass;
            if (delta_ek > 0)
            {
                velocity *= Mathf.Sqrt(delta_ek / mass) / velocity.magnitude;
                direction = velocity.normalized;
            }
            else
            {
                if (mass > target_hit.Mass * 0.5)
                {
                    velocity *= Config.current.bullet_enemy_bounce_coef;
                    direction = Random.rotation * velocity;
                }
                else
                {
                    validate = false;
                }
            }

            hit_target_event?.Invoke(target_hit);
        }


        public override void HitGround(float road_height)
        {
            Road_Info_Helper.try_get_slope(position.x, out var slope);
            var ground_slope = new Vector2(1f, slope);
            var ground_normal = new Vector2(-slope, 1f);

            var v_landing_parall = Vector2.Dot(velocity, ground_slope) * ground_slope;
            var v_landing_vertical = Vector2.Dot(velocity, ground_normal) * ground_normal;

            var ek = mass * Mathf.Pow(velocity.magnitude, 2);
            var vtm = v_landing_vertical.magnitude;
            var vpm = v_landing_parall.magnitude;

            if (ek > Config.current.bullet_bounce_threshold_ekmin && vtm < Config.current.bullet_bounce_threshold_vmax)
            {
                v_landing_vertical *= -Config.current.bullet_surface_bounce_coef;
                if (vpm != 0)
                    v_landing_parall *= (vpm - Mathf.Min(vpm, vtm * (1 + Config.current.bullet_surface_bounce_coef) * Config.current.surface_friction)) / vpm;
                rot_speed *= 0.6f;
                velocity = v_landing_vertical + v_landing_parall;
                position = new Vector2(position.x, road_height);
                if (velocity.magnitude > desc.vfx_v_threshold && Vector2.SqrMagnitude(position - WorldContext.instance.caravan_pos) < 400f)
                {
                    if (desc.vfx_on_hit_ground != null)
                        Vfx_Helper.InstantiateVfx(desc.vfx_on_hit_ground, 240, position);
                    if (desc.se_on_hit_ground != null)
                        Audio.AudioSystem.instance.PlayOneShot(desc.se_on_hit_ground);
                }
            }
            else
            {
                movement_status_change_to(MovementStatus.in_ground);
            }
        }
        public override void ResetPos()
        {
            base.ResetPos();
            last_position -= new Vector2(WorldContext.instance.reset_dis, 0);
        }
        /*public override void HitCaravan()
        {
            
        }
        public override void HitDevice()
        {
            var size_factor = BattleContext.instance.projectile_scale_factor;

            if (desc.detection_type == "Ray")
            {
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

                var targets = BattleUtility.select_all_target_in_rect(p1, p2, faction, (ITarget t) =>
                {
                    return t != last_hit;
                });

                if (targets.Count != 0)
                {
                    foreach (var target in targets)
                    {
                        if (target != null && target.is_interactive)
                        {
                            //1.对目标造成伤害与击退
                            var ad = modify_attack_data(target);

                            target.hurt(ad);
                            target.impact(WorldEnum.impact_source_type.projectile, velocity, mass, Config.current.bullet_penetration_loss);

                            //2.根据剩余动能，判定飞射物自身的后续运动方式
                            last_hit = target;
                            var ek_mul_2 = mass * Mathf.Pow(velocity.magnitude, 2);
                            var delta_ek = ek_mul_2 - Config.current.bullet_penetration_loss * target.Mass;
                            if (delta_ek > 0)
                            {
                                velocity *= Mathf.Sqrt(delta_ek / mass) / velocity.magnitude;
                                direction = velocity.normalized;
                            }
                            else
                            {
                                if (mass > target.Mass * 0.5)
                                {
                                    velocity *= Config.current.bullet_enemy_bounce_coef;
                                    direction = Random.rotation * velocity;
                                }
                                else
                                {
                                    validate = false;
                                }
                            }
                        }
                    }
                }
            }
            else if (desc.detection_type == "Radius")
            {
                var target = BattleUtility.select_target_in_circle(position, radius, faction, (ITarget t) =>
                {
                    return t != last_hit;           //此处其实这个函数没有意义,只是用于示范
                });
                if (target != null && target.is_interactive)
                {
                    var ad = modify_attack_data(target);

                    target.hurt(ad);
                    target.impact(WorldEnum.impact_source_type.projectile, velocity, mass, Config.current.bullet_penetration_loss);

                    //2.根据剩余动能，判定飞射物自身的后续运动方式
                    last_hit = target;
                    var ek_mul_2 = mass * Mathf.Pow(velocity.magnitude, 2);
                    var delta_ek = ek_mul_2 - Config.current.bullet_penetration_loss * target.Mass;
                    if (delta_ek > 0)
                    {
                        velocity *= Mathf.Sqrt(delta_ek / mass) / velocity.magnitude;
                        direction = velocity.normalized;
                    }
                    else
                    {
                        if (mass > target.Mass * 0.5)
                        {
                            velocity *= Config.current.bullet_enemy_bounce_coef;
                            direction = Random.rotation * velocity;
                        }
                        else
                        {
                            validate = false;
                        }
                    }
                }
            }
        }*/
    }
}
