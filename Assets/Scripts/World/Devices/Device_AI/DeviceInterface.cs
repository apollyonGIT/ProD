using UnityEngine;
using World.Projectiles;

namespace World.Devices.Device_AI
{
    public interface IAttack
    {
        float Damage_Increase { get; set; }
        float Knockback_Increase { get; set; }
        int Attack_Interval { get; set; }
        int Current_Interval { get; set; }
        public void TryToAutoShoot();
    }

    public interface ILoad
    {
        int Max_Ammo { get; set; }
        int Current_Ammo { get; set; }
        float Reloading_Process { get; set; }
        float Reload_Speed { get; set; }
        public void TryToAutoLoad();
    }

    public interface IRecycle
    {
        int Recycle_Interval { get; set; }
        int Current_Recycle_Interval { get; set; }
        public void TryToAutoRecycle();
    }

    public interface IShield
    {
        float ShieldEnergy_Current { get; set; }            
        float ShieldEnergy_Max { get; set; }
        float ShieldEnergy_Recover_1 { get; set; }            //盾牌能量恢复速度
        float ShieldEnergy_Recover_2 { get; set; }
        int ShieldEnergy_FreezeTick_Current { get; set; }
        int ShieldEnergy_FreezeTick_Max { get; set; }
        float ShieldEnergy_Deduct_By_Blocking { get; set; }
        int Shield_Blocking_Interval_Current {  get; set; }
        int Shield_Blocking_Interval_Max { get; set; }

        Vector2 Def_Dir { get; set; }
        float Def_Range { get; set; }
        Vector2 Shield_Dir { get; }
        bool Try_Rebound_Projectile(Projectile proj, Vector2 proj_v)
        {
            return false;
        }
    }

    public interface ISharp
    {
        float Sharpness_Current { get; set; }
        float Sharpness_Min { get; set; }
        float Sharpness_Loss { get; set; }
        float Sharpness_Recover { get; set; }

        public void TryToAutoSharp();
    }
}
