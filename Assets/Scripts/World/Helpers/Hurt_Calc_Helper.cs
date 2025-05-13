using UnityEngine;

namespace World.Helpers
{
    public class Hurt_Calc_Helper
    {
        static Dmg_Data calc(Attack_Data attack_data, Defense_Data defense_data)
        {
            var battle_ctx = BattleContext.instance;

            //1. 获取攻击者的【攻击】，记为atk，类型为int
            //   获取受击者的【减伤】，记为def，类型为int
            var atk = Mathf.FloorToInt(attack_data.atk * battle_ctx.enemy_attack_factor);
            var def = Mathf.FloorToInt(defense_data.def * battle_ctx.enemy_def_factor);

            //2. 获取攻击者的【破甲】，记为ap
            var ap = attack_data.armor_piercing / 1000f;

            //3. 计算受击者的被修正后的护甲值，记为D
            var D = Mathf.CeilToInt(def * (1 - ap));
            D = Mathf.Max(D, 0);

            //4. 获取攻击者的【暴击几率】，记为cc
            //   获取攻击者的【暴击倍率】，记为cr
            var cc = attack_data.critical_chance / 1000f;
            var cr = attack_data.critical_rate / 1000f;

            //5. 计算本次攻击是否暴击
            bool is_critical = Random.Range(0f, 1f) < cc;

            //6. 获取攻击者的【攻击力】，记为atk
            if (is_critical)
                atk = Mathf.CeilToInt(atk * cr);

            var f = Random.Range(0.9f, 1.1f);
            //7. 计算造成的伤害dmg
            Dmg_Data dmg_data = new()
            {
                dmg = atk - D > 1 ? (int)((atk - D) * f) : 1,
                is_critical = is_critical
            };

            return dmg_data;
        }


        public static Dmg_Data dmg_to_enemy(Attack_Data attack_data, params object[] args)
        {
            var key = (string)args[0];

            AutoCodes.monsters.TryGetValue(key, out var r);
            Defense_Data defense_data = new()
            {
                def = r.def
            };

            return calc(attack_data, defense_data);
        }


        public static Dmg_Data dmg_to_caravan(Attack_Data attack_data, params object[] args)
        {
            var key = (string)args[0];

            AutoCodes.caravan_bodys.TryGetValue(key, out var r);
            Defense_Data defense_data = new()
            {
                def = r.def
            };

            return calc(attack_data, defense_data);
        }


        public static Dmg_Data dmg_to_device(Attack_Data attack_data, params object[] args)
        {
            var key = (string)args[0];

            AutoCodes.device_alls.TryGetValue(key, out var r);
            Defense_Data defense_data = new()
            {
                def = r.def
            };

            return calc(attack_data, defense_data);
        }
    }
}

