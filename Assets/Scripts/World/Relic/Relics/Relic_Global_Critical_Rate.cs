namespace World.Relic.Relics
{
    public class Relic_Global_Critical_Rate : Relic
    {
        public override void Get()
        {
            BattleContext.instance.critical_chance_delta += desc.parm_int[0] ;
        }

        public override void Drop()
        {
            BattleContext.instance.critical_chance_delta -= desc.parm_int[0] ;
        }
    }
}
