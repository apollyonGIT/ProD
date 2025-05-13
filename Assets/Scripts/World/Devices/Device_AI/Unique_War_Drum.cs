using AutoCodes;
using Foundations.Tickers;
using System.Collections.Generic;
using UnityEngine;
using World.Audio;
using World.Devices.Device_AI;
using World.Helpers;
using static World.WorldEnum;

namespace World.Devices
{
    public class Unique_War_Drum : Device, IAttack
    {
        #region CONST
        private const string ANIM_IDLE = "idle";
        private const string ANIM_ATTACK_L = "attack_1";
        private const string ANIM_ATTACK_R = "attack_2";
        private const string ANIM_BROKEN = "idle";
        //private const string BONE_FOR_ROTATION = "control";

        private const float EXPLOSION_EFFECT_MIN = 0.5F;

        private const float ATK_EVENT_TIME_PERCENT = 13f / 20f;

        private const int AUTO_ATTACK_DELAY = 20;
        #endregion

        private enum Device_War_Drum_FSM
        {
            idle,
            attack,
            broken,
        }
        private Device_War_Drum_FSM fsm;

        private List<ITarget> targets;
        private Request request;

        public float attack_factor_expt = 1f;
        private float attack_range;

        private bool will_atk_right;

        public bool manual_left_ready => fsm == Device_War_Drum_FSM.idle && !will_atk_right;
        public bool manual_right_ready => fsm == Device_War_Drum_FSM.idle && will_atk_right;

        public float Damage_Increase { get; set; }
        public float Knockback_Increase { get; set; }
        public int Attack_Interval { get; set; }
        public int Current_Interval { get; set; }

        public override void InitData(device_all rc)
        {
            module_list.Clear();    //之后可能考虑统一到卸载里面
            bones_direction.Clear();

            DeviceModule deviceModule = new DeviceModule();

            var fb = new FireBehaviour() { module = deviceModule };
            deviceModule.db_list.Add(fb);

            deviceModule.device = this;
            module_list.Add(deviceModule);

            other_logics.TryGetValue(rc.other_logic.ToString(), out var logic);

            attack_range = rc.basic_range.Item2;
            
            base.InitData(rc);

            #region AnimInfo
            var atk_anim_l_hit = new AnimEvent()
            {
                anim_name = ANIM_ATTACK_L,
                percent = ATK_EVENT_TIME_PERCENT,
                anim_event = (Device d) =>
                {
                    BoardCast_Helper.to_all_target(explosion);
                    AudioSystem.instance.PlayOneShot(logic.se_1);
                }
            };

            var atk_anim_l_end = new AnimEvent()
            {
                anim_name = ANIM_ATTACK_L,
                percent = 1,
                anim_event = (Device d) => FSM_change_to(Device_War_Drum_FSM.idle)
            };

            var atk_anim_r_hit = new AnimEvent()
            {
                anim_name = ANIM_ATTACK_R,
                percent = ATK_EVENT_TIME_PERCENT,
                anim_event = (Device d) =>
                {
                    BoardCast_Helper.to_all_target(explosion);
                    AudioSystem.instance.PlayOneShot(logic.se_1);
                }
            };

            var atk_anim_r_end = new AnimEvent()
            {
                anim_name = ANIM_ATTACK_R,
                percent = 1,
                anim_event = (Device d) => FSM_change_to(Device_War_Drum_FSM.idle)
            };

            #region 子函数 explosion
            void explosion(ITarget target)
            {
                WorldSceneRoot.instance.sr.SetActive(true);

                if (request != null)
                    request.interrupt();

                request = Request_Helper.delay_do("close_screen_shock", 30, (req) =>
                {
                    WorldSceneRoot.instance.sr.SetActive(false);
                });

                var dis = Vector2.Distance(target.Position, position);
                if (dis >= attack_range)  //爆炸范围
                    return;

                if (target.Faction != faction)
                {

                    // Make Damage
                    var dis_coef = (EXPLOSION_EFFECT_MIN - 1) / attack_range * dis + 1;  // Min Damage == 50%

                    Attack_Data attack_data = new()
                    {
                        atk = Mathf.CeilToInt(logic.damage * dis_coef * (1 + Damage_Increase))
                    };

                    target.hurt(this, attack_data, out var dmg_data);
                    BattleContext.instance.ChangeDmg(this, dmg_data.dmg);

                    if (target.hp <= 0)
                    {
                        kill_enemy_action?.Invoke(target);
                    }

                    // Make KnockBack
                    target.impact(impact_source_type.melee, position, target.Position, logic.knockback_ft * dis_coef * (1 + Knockback_Increase));
                }
            }
            #endregion

            #endregion

            anim_events.Add(atk_anim_l_hit);
            anim_events.Add(atk_anim_l_end);
            anim_events.Add(atk_anim_r_hit);
            anim_events.Add(atk_anim_r_end);
            
            
        }

        public override void tick()
        {
            if (!is_validate)       //坏了
                FSM_change_to(Device_War_Drum_FSM.broken);

            switch (fsm)
            {
                case Device_War_Drum_FSM.idle:
                    attack_factor_expt -= (attack_factor_expt - 1) * 0.01f;
                    break;
                case Device_War_Drum_FSM.attack:
                    break;
                case Device_War_Drum_FSM.broken:
                    if (is_validate)
                        FSM_change_to(Device_War_Drum_FSM.idle);
                    break;

                default:
                    break;
            }
            base.tick();
        }

        private void FSM_change_to(Device_War_Drum_FSM target_fsm)
        {
            fsm = target_fsm;
            switch (target_fsm)
            {
                case Device_War_Drum_FSM.idle:
                    ChangeAnim(ANIM_IDLE, true);
                    break;
                case Device_War_Drum_FSM.attack:
                    ChangeAnim(will_atk_right ? ANIM_ATTACK_R : ANIM_ATTACK_L, false);
                    will_atk_right = !will_atk_right;
                    break;
                case Device_War_Drum_FSM.broken:
                    ChangeAnim(ANIM_BROKEN, true);
                    break;
                default:
                    break;
            }
        }

        private void change_factor_of_anim_speed(float f)
        {
            foreach (var view in views)
                view.notify_change_anim_speed(f);
        }


        private void Auto_tick()
        {
            if (player_oper)
                return;

            switch (fsm)
            {
                case Device_War_Drum_FSM.idle:
                    if (Current_Interval <= 0)
                    {
                        if (has_target())
                            beat_drum(will_atk_right);
                        Current_Interval = AUTO_ATTACK_DELAY;
                    }
                    else
                        Current_Interval--;
                    break;

                default:
                    break;
            }
        }

        private bool has_target()
        {
            targets = BattleUtility.select_all_target_in_circle(position, attack_range, faction);
            return targets.Count > 0;
        }


        void IAttack.TryToAutoShoot()
        {
            Auto_tick();
        }

        private void beat_drum(bool input_right)
        {
            if (fsm == Device_War_Drum_FSM.idle && input_right == will_atk_right)
            {
                FSM_change_to(Device_War_Drum_FSM.attack);
                attack_factor_expt += 0.5f;
            }
            else
            {
                attack_factor_expt = 1f;
            }
            change_factor_of_anim_speed(attack_factor_expt);
        }

        #region PlayerControl
        public override void StartControl()
        {
            base.StartControl();
        }

        public override void EndControl()
        {
            base.EndControl();
        }

        public void Attack_1()
        {
            beat_drum(false);
        }

        public void Attack_2()
        {
            beat_drum(true);
        }
        #endregion
    }
}
