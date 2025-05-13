namespace World.Relic.Relics
{
    public class Relic_Ranged_Salvo : Relic
    {
        public override void Get()
        {
            BattleContext.instance.projectile_salvo_amount += desc.parm_int[0];
        }

        public override void Drop()
        {
            BattleContext.instance.projectile_salvo_amount -= desc.parm_int[0];
        }
    }
}
