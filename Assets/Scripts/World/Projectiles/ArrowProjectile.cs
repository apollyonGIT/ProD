using Commons;
using UnityEngine;
using World.Devices;

namespace World.Projectiles
{
    public class ArrowProjectile : Projectile
    {
        private const int MAX_IN_OBJECT_TICKS = 10;

        private int in_object_ticks = 0;

        protected override void movement_in_object()
        {
            if (in_object_ticks < 0)
            {
                var current_dir = in_target.direction;
                var angel = Vector2.SignedAngle(direction_in_object, current_dir);
                Vector2 current_offset = Quaternion.AngleAxis(angel, Vector3.forward) * pos_offset_in_object;

                position = in_target.Position + current_offset;
            }
            else
            {
                var acc_y = Config.current.gravity;
                Vector2 ammo_acc = new Vector2(0, acc_y);
                if (desc.propulsion_force > 0)
                {
                    ammo_acc += direction.normalized * desc.propulsion_force / desc.mass;
                    rot_speed += rot_propulsion * Config.PHYSICS_TICK_DELTA_TIME;
                }

                velocity += ammo_acc * Config.PHYSICS_TICK_DELTA_TIME;
                velocity *= 0.95f;  //降低速度
                position += velocity * Config.PHYSICS_TICK_DELTA_TIME;  // Move

                if (--in_object_ticks >= 0)
                    return;

                // last，check if is still collided with the same target
                if (check_and_select_single_target(out var target, true))
                {
                    //记录插入时 物体的 position 与 direction
                    pos_offset_in_object = position - in_target.Position;
                    direction_in_object = in_target.direction;
                }
                else
                {
                    movement_status_change_to(MovementStatus.normal);
                }
            }
        }


        protected override void rotate()
        {
            if (desc.propulsion_force > 0)
                base.rotate();
            else
                direction = velocity;
        }


        public override void HitEnemy(ITarget target_hit)
        {
            //1.对目标造成伤害与击退
            var ad = modify_attack_data(target_hit);

/*            if (emitter is Device d)
                BattleContext.instance.ChangeDmg(d, target_hit.hurt(ad));
            else
                target_hit.hurt(ad);*/

            target_hit.hurt(this, ad, out var dmg_data);
            if(emitter is Device d)
            { 
                BattleContext.instance.ChangeDmg(d, dmg_data.dmg);
                if (target_hit.hp <= 0)
                {
                    d.kill_enemy_action?.Invoke(target_hit);
                }
            }
           

            target_hit.impact(WorldEnum.impact_source_type.projectile, velocity, mass, Config.current.arrow_penetration_loss);

            //2.根据剩余动能，判定飞射物自身的后续运动方式
            last_hit = target_hit;
            var ek_mul_2 = mass * Mathf.Pow(velocity.magnitude, 2);
            var delta_ek = ek_mul_2 - Config.current.arrow_penetration_loss * target_hit.Mass;

            if (delta_ek > 0)
            {
                velocity *= Mathf.Sqrt(delta_ek / mass) / velocity.magnitude;
                direction = velocity.normalized;
            }
            else
            {
                in_object_ticks = MAX_IN_OBJECT_TICKS;
                in_target = target_hit;
                movement_status_change_to(MovementStatus.in_object);

                //规则：附加自身在目标上的重量
                target_hit.attach_data(mass);
            }

            hit_target_event?.Invoke(target_hit);
        }


        public override void HitGround(float road_height)
        {
            movement_status_change_to(MovementStatus.in_ground);
        }


        /*public  void HitDevice()
        {
            if (target_select(out var target))
            {
                //1.对目标造成伤害与击退
                var ad = modify_attack_data(target);

                target.hurt(ad);

                target.impact(WorldEnum.impact_source_type.projectile, velocity, mass, Config.current.arrow_penetration_loss);

                //2.根据剩余动能，判定飞射物自身的后续运动方式
                last_hit = target;
                //车体  设备 必定插
                in_target = target;
                movement_status_change_to(MovementStatus.in_object);

                //规则：附加自身在目标上的重量
                target.attach_data(mass);
            }
        }*/
    }
}
