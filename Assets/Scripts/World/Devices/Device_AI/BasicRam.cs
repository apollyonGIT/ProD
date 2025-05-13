using AutoCodes;
using UnityEngine;
using World.Audio;
using World.Enemys;
using World.Enemys.BT;

namespace World.Devices.Device_AI
{
    public class BasicRam : Device
    {
        #region CONST
        private const string BONE_FOR_ROTATION = "roll_control";

        private const string ANIM_IDLE = "idle";
        private const string ANIM_BROKEN = "idle";
        private const string COLLIDER_1 = "collider_1";
        private const string KEY_POINT_1 = "collider_1";
        #endregion

        private enum Device_FSM_Ram
        {
            idle,
            broken,
        }
        private Device_FSM_Ram fsm;

        public Vector2 ram_dir = Vector2.right;
        private string se_ram_hit_enemy;
        private string se_ram_rotate;

        #region Table Data
        private int basic_damage;
        private float damage_speed_mod;
        private float basic_knockback;
        private float knockback_speed_mod;
        private float collision_rotate_coef;
        private float ui_rotate_coef;
        #endregion

        public float Damage_Increase { get; set; }
        public float Knockback_Increase { get; set; }

        public override void InitData(device_all rc)
        {
            module_list.Clear();    //之后可能考虑统一到卸载里面

            device_type = DeviceType.melee;

            bones_direction.Clear();
            bones_direction.Add(BONE_FOR_ROTATION, ram_dir);

            ram_logics.TryGetValue(rc.ram_logic.ToString(), out var logic);
            basic_damage = logic.damage;
            damage_speed_mod = logic.damage_v_coef;
            basic_knockback = logic.ft;
            knockback_speed_mod = logic.ft_v_coef;
            collision_rotate_coef = logic.collision_rotate_coef;
            ui_rotate_coef = logic.ui_rotate_coef;

            se_ram_hit_enemy = logic.SE_hit_enemy;
            se_ram_rotate = logic.SE_rotate;

            base.InitData(rc);
        }

        public override void Start()
        {
            base.Start();
            FSM_change_to(Device_FSM_Ram.idle);
        }


        // ========================================================================================

        public override void tick()
        {
            if (!is_validate && fsm != Device_FSM_Ram.broken)       //坏了
                FSM_change_to(Device_FSM_Ram.broken);

            switch (fsm)
            {
                case Device_FSM_Ram.idle:
                    rotate_bone_to_dir(BONE_FOR_ROTATION, ram_dir);
                    break;
                case Device_FSM_Ram.broken:
                    if (is_validate)
                        FSM_change_to(Device_FSM_Ram.idle);
                    break;

                default:
                    break;
            }
            base.tick();
        }

        private void FSM_change_to(Device_FSM_Ram target_fsm)
        {
            fsm = target_fsm;
            switch (target_fsm)
            {
                case Device_FSM_Ram.idle:
                    ChangeAnim(ANIM_IDLE, true);
                    open_collider_this(this);
                    break;
                case Device_FSM_Ram.broken:
                    ChangeAnim(ANIM_BROKEN, true);
                    break;
                default:
                    break;
            }
        }

        public override void Disable()
        {
            CloseCollider(COLLIDER_1);
            base.Disable();
        }

        // ----------------------------------------------------------------------------------------


        private void open_collider_this(Device d)
        {
            d.OpenCollider(COLLIDER_1, (ITarget t) =>
            {
                var v_relative_parallel = Vector2.Dot((this as ITarget).velocity - t.velocity, bones_direction[BONE_FOR_ROTATION].normalized);
                if (v_relative_parallel <= 0)
                    return;

                int final_dmg = (int)(basic_damage * (1 + Damage_Increase) * v_relative_parallel * damage_speed_mod);
                float final_ft = basic_knockback * (1 + Knockback_Increase * v_relative_parallel * knockback_speed_mod);

                Attack_Data attack_data = new()
                {
                    atk = final_dmg,
                    critical_chance = desc.critical_chance + BattleContext.instance.critical_chance_delta,
                    critical_rate = desc.critical_rate + BattleContext.instance.global_critical_rate,
                };

                t.hurt(this, attack_data, out var dmg_data);
                BattleContext.instance.ChangeDmg(this, dmg_data.dmg);

                if (t.hp <= 0)
                {
                    kill_enemy_action?.Invoke(t);
                }

                AudioSystem.instance.PlayOneShot(se_ram_hit_enemy);
                var sign = Mathf.Sign(BattleUtility.get_target_colllider_pos(t).x - key_points[KEY_POINT_1].position.x);
                t.impact(WorldEnum.impact_source_type.melee, Vector2.zero, new Vector2(sign, 0.5f), final_ft);
                if (t is Enemy e)
                    if (e.bt is IEnemy_Can_Jump j)
                        j.Get_Rammed(final_ft);

                var collision_angle = (Vector2.SignedAngle(bones_direction[BONE_FOR_ROTATION], Vector2.up) * v_relative_parallel);
                ram_dir = Quaternion.AngleAxis(collision_angle * collision_rotate_coef, Vector3.forward) * ram_dir;
            });
        }

        public void Rotate_Ram_Dir_By_UI(float angle)
        {
            ram_dir = Quaternion.AngleAxis(angle * ui_rotate_coef, Vector3.forward) * ram_dir;
        }

        /// <summary>
        /// 根据转轮是否转动，播放或停止音效
        /// </summary>
        public void Play_Or_End_SE_By_UI(bool play)
        {
            if (play)
                AudioSystem.instance.PlayClip(se_ram_rotate, true);
            else
                AudioSystem.instance.StopClip(se_ram_rotate);
        }
    }
}
