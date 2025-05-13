using Commons;
using Foundations;
using Foundations.Tickers;
using System.Collections.Generic;
using UnityEngine;
using World.Devices;

namespace World
{
    public class BattleContext : Singleton<BattleContext>
    {
        public float melee_scale_factor = 1f;           //近战设备大小
        public int projectile_scale_factor = 1000;      //设备子弹大小，千分比
        public int projectile_repeate_amount = 0;       //设备重复射击增量
        public int projectile_salvo_amount = 0;         //设备散射增量
        public float enemy_scale_factor = 1f;           //怪物体型系数
        public float enemy_attack_factor = 1f;          //怪物攻击力系数
        public float enemy_def_factor = 1f;             //怪物防御力系数
        public float enemy_vel_factor = 1f;             //怪物速度系数
        public float enemy_mass_factor = 1f;            //怪物重量系数
        public float enemy_hp_factor = 1f;              //怪物hp系数
        public float drop_loot_delta = 0f;              //掉落物品增量    

        public int critical_chance_delta = 0;        //全局暴击概率增量
        public int global_atk_pts = 0;               //全局攻击力百分百增量（千分之百）
        public int global_atk_add = 0;               //全局攻击力增量
        public int global_critical_rate = 0;         //全局暴击率增量
        public int global_critical_dmg = 0;          //全局暴伤百分百增量
        public int global_knockback = 0;              //全局击退增量
        public int global_armor_piercing = 0;        //全局破甲百分百
        public int global_armor_add = 0;             //全局护甲增量
        public int global_hp_pts = 0;                //全局血量百分百增量（千分之百）
        public int global_hp_add = 0;                //全局血量增量


        #region Melee

        public int melee_atk_pts = 0;             //近战攻击力百分百增量（千分之百）
        public int melee_atk_add = 0;             //近战攻击力增量
        public int melee_critical_rate = 0;          //近战暴击倍率增量
        public int melee_critical_dmg = 0;           //近战暴伤百分百增量
        public int melee_knockback = 0;               //近战击退增量
        public int melee_armor_piercing = 0;         //近战破甲百分百增量
        public int melee_sharpness_durability = 0;   //近战锋利度耐用修正
        public int melee_sharpness_add = 0;          //近战锋利度上限增量
        public int melee_sharpness_recover = 0;      //近战锋利度恢复增量


        #endregion

        #region Ranged
        public int ranged_atk_pts = 0;             //远程攻击力百分百增量（千分之百）
        public int ranged_atk_add = 0;             //远程攻击力增量
        public int ranged_critical_rate = 0;       //远程暴击倍率增量
        public int ranged_critical_dmg = 0;        //远程暴伤百分百增量
        public int ranged_knockback = 0;            //远程击退增量
        public int ranged_armor_piercing = 0;      //远程破甲百分百增量

        public int ranged_reloading_speedup = 0;   //远程装填速度增量
        public int ranged_capacity;                //远程弹药容量增量
        public int ranged_salvo;                   //远程散射增量

        public int ranged_critical_reload = 0;     //暴击时子弹恢复
        public int ranged_ammo_bigger = 0;         //子弹体积增大

        #endregion

        #region Wheel
        public int wheel_charge_capacity = 0;             //轮子充能上限增量
        public int wheel_charge_effect = 0;               //轮子冲刺效果提升
        public int wheel_charge_recover = 0;              //轮子充能恢复增量
        public int wheel_tractive_force = 0;              //轮子动力增强
        #endregion

        #region Ram

        public int ram_stability = 0;                  //撞角稳定性增量
        public int ram_maneuverability = 0;            //撞角灵活性增量
        public int ram_dmg_speed_bonus = 0;            //撞角伤害速度附加

        #endregion

        public void Init()
        {
            Ticker.instance.add_tick(0, "BattleContext", tick);
        }

        public void Fini()
        {
            Ticker.instance.remove_tick("BattleContext");
        }

        public Dictionary<Device,DmgInfos> device_dmg_dic = new();          //设备和造成伤害的映射关系

        public void tick()
        {
            foreach(var (_,infos) in device_dmg_dic)
            {
                for (int i = infos.effect_dmgs.Count - 1; i >= 0; i--)
                {
                    infos.effect_dmgs[i].remain_ticks -= 1;
                    if (infos.effect_dmgs[i].remain_ticks <= 0)
                    {
                        infos.effect_dmgs.RemoveAt(i);
                    }
                }
            }
        }

        public void ResetDmg()
        {
            device_dmg_dic.Clear();
            Mission.instance.try_get_mgr(Commons.Config.DeviceMgr_Name, out DeviceMgr dmgr);
            foreach(var (slot,device) in dmgr.slots_device)
            {
                if (device != null)
                {
                    device_dmg_dic.Add(device, new DmgInfos(device));
                }
            }
        }

        public void ChangeDmg(Device device, float delta)
        {
            if (device_dmg_dic.ContainsKey(device))
            {
                device_dmg_dic[device].AddDmg(delta);
            }
            else
            {
                device_dmg_dic.Add(device, new DmgInfos(device));
                device_dmg_dic[device].AddDmg(delta);
            }
        }
    }

    public class DmgInfos
    {
        public DmgInfos(Device d)
        {
            device = d;
        }

        public Device device;
        public float total_dmg;         //总伤
        public float highest_dmg;       //最高伤害
        public List<DmgInfo> effect_dmgs = new();

        public float GetAverDmg()
        {
            float sum = 0;
            foreach (var dmi in effect_dmgs)
            {
                sum += dmi.dmg;
            }

            return sum/((float)device.equip_time/Config.PHYSICS_TICK_DELTA_TIME);
        }
        public void AddDmg(float dmg,float remain_ticks = 3600)
        {
            effect_dmgs.Add(new DmgInfo()
            {
                dmg = dmg,
                remain_ticks = remain_ticks,
            });
            total_dmg += dmg;
        }

        public float GetTotalDmg()
        {
            return total_dmg;
        }
    }

    public class DmgInfo
    {
        public float dmg;

        public float remain_ticks;          //应该没用了 
    }
}
