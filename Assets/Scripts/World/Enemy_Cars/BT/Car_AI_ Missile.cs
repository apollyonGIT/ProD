using Commons;
using System;
using UnityEngine;
using World.Caravans;
using World.Enemys;
using World.Helpers;

namespace World.Enemy_Cars.BT
{
    public class Car_AI_Missile : IEnemy_BT
    {
        EN_caravan_move_type m_state = EN_caravan_move_type.Run;

        string IEnemy_BT.state => $"{m_state}";

        Enemy_Car car_cell;

        float target_x;
        int lock_cd = 0;
        Action select_target_ac = null;

        //==================================================================================================

        void IEnemy_BT.init(Enemy _cell, params object[] prms)
        {
            car_cell = _cell as Enemy_Car;

            target_x = WorldContext.instance.caravan_pos.x + 99f;
            //car_cell.ctx.caravan_velocity.x = 4f;
        }


        void IEnemy_BT.tick(Enemy _cell)
        {
            var world_ctx = WorldContext.instance;
            if (world_ctx.is_need_reset) return;

            var caravan_driving_lever = world_ctx.driving_lever;
            var caravan_x = world_ctx.caravan_pos.x - 5f;

            var ctx = car_cell.ctx;
            ref var driving_lever = ref ctx.driving_lever;
            var self_x = car_cell.pos.x;

            var relative_x = caravan_x - self_x;
            var is_forward = relative_x >= 0;
            var dis = is_forward ? relative_x : -relative_x;

            lock_cd--;

            if (is_forward && lock_cd <= 0)
            {
                select_target_ac = () => { target_x = caravan_x + 2f; };
                lock_cd = 120;
            }
            else if (!is_forward && lock_cd <= 0)
            {
                select_target_ac = () => { target_x = caravan_x - 2f; };
                lock_cd = 120;
            }

            select_target_ac?.Invoke();

            var relative_target_x = target_x - self_x;
            if (Mathf.Abs(relative_target_x) <= 0.5f)
                lock_cd = 0;

            if (relative_target_x >= 0)
                relative_target_x = Mathf.Clamp(relative_target_x, 4.5f, 8);
            else
                relative_target_x = Mathf.Clamp(relative_target_x, -8, -4.5f);

            driving_lever = relative_target_x / 10;

            if (is_forward && dis >= 5f)
            {
                driving_lever = 0.8f;
                target_x = caravan_x + 99f;
            }
        }


        void IEnemy_BT.notify_on_enter_die(Enemy cell)
        {

        }


        void IEnemy_BT.notify_on_dying(Enemy cell)
        {

        }


        void IEnemy_BT.notify_on_dead(Enemy cell)
        {

        }
    }
}

