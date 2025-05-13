using AutoCodes;

namespace World.Characters.CharacterProperties
{
    public class CP_Big_Eater : CharacterProperty
    {
        public CP_Big_Eater(Character c, properties p) : base(c, p)
        {
        }

        public override void Init()
        {
            owner.per_eat = desc.int_parms[0];
            owner.after_eat += AfterEat;
        }

        private void AfterEat(uint f)
        {
            owner.UpdateMood(desc.float_parms[0]);
        }
    }
}
