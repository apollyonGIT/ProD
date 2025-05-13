namespace World.Relic.Relics
{
    public class AddRepeatRelic :Relic
    {
        public override void Get()
        {
            BattleContext.instance.projectile_repeate_amount += 1;
        }

        public override void Drop()
        {
            BattleContext.instance.projectile_repeate_amount -= 1;
        }
    }
}
