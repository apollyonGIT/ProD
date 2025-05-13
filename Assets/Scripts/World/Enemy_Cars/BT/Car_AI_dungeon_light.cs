using Commons;
using UnityEngine;
using World.Enemys;
using World.Enemys.BT;
using World.Helpers;

namespace World.Enemy_Cars.BT
{
    public class Car_AI_dungeon_light : Enemy_Monster_Basic_BT, IEnemy_BT
    {
        #region CONST
        const float FLYAROUND_DEG_MIN = 10F;
        const float FLYAROUND_DEG_MAX = 170F;
        const float FLYAROUND_RADIUS_MIN = 4F;
        const float FLYAROUND_RADIUS_MAX = 9F;

        const int CHARGE_CD = 7 * Config.PHYSICS_TICKS_PER_SECOND;
        const int MAX_CHARGE_TICKS = 180;

        const float FLY_SPEED_IDLE = 9F;
        const float FLY_SPEED_BURST = 16F;
        const float FLY_SPEED_MAX = 25F;
        #endregion

        private enum EN_dungeon_bat_FSM
        {
            Default,
            Charge,
            Fall,
            Dead
        }
        EN_dungeon_bat_FSM m_state;
        string IEnemy_BT.state => $"{m_state}";

        private int charge_cd = Random.Range(0, CHARGE_CD);
        private bool charge_to_right;
        private int ticks_in_charge;
        private bool can_bite;


        //==================================================================================================

        void IEnemy_BT.init(Enemy cell, params object[] prms)
        {
            m_state = (EN_dungeon_bat_FSM)System.Enum.Parse(typeof(EN_dungeon_bat_FSM), (string)prms[0]);
            cell.mover.move_type = EN_enemy_move_type.Fly;

        }


        void IEnemy_BT.tick(Enemy cell)
        {
            var ctx = cell.mgr.ctx;
            var mover = cell.mover;

       

            //朝向
            cell.dir.x = cell.velocity.x;
            mover.move();
        }

        private void set_speed_expt(Enemy cell, float v)
        {
            cell.speed_expt = Mathf.Clamp(v, 0, FLY_SPEED_MAX);

            var t_hp = 0.8f * (cell.hp_max - cell.hp) / cell.hp_max;
            var t_v = (FLY_SPEED_MAX - t_hp * cell.speed_expt) / FLY_SPEED_MAX;
            cell.speed_expt *= t_v;
        }



        void IEnemy_BT.notify_on_enter_die(Enemy cell)
        {
            m_state = EN_dungeon_bat_FSM.Fall;

            var mover = cell.mover;
            mover.move_type = EN_enemy_move_type.Hover;

            mover.move();
        }


        void IEnemy_BT.notify_on_dying(Enemy cell)
        {
            if (m_state == EN_dungeon_bat_FSM.Dead) return;

            var mover = cell.mover;
            if (mover.move_type == EN_enemy_move_type.Slide)
            {
                cell.fini();
                m_state = EN_dungeon_bat_FSM.Dead;
                (this as IEnemy_BT).notify_on_dead(cell);
            }

            mover.move();
        }


        void IEnemy_BT.notify_on_dead(Enemy cell)
        {

        }
    }
}

