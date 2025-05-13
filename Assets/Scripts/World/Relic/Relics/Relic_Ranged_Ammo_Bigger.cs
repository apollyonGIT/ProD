namespace World.Relic.Relics
{
    public class Relic_Ranged_Ammo_Bigger : Relic
    {
        public override void Get()
        {
            BattleContext.instance.projectile_scale_factor += desc.parm_int[0];
        }

        public override void Drop()
        {
            BattleContext.instance.projectile_scale_factor -= desc.parm_int[0];
        }
    }
}
