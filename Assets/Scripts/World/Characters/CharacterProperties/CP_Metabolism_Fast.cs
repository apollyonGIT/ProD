using AutoCodes;
using World.Devices;

namespace World.Characters.CharacterProperties
{
    public class CP_Metabolism_Fast : CharacterProperty
    {
        public CP_Metabolism_Fast(Character c, properties p) : base(c, p)
        {
        }

        public override void Init()
        {
            owner.enter_safe_area += RapidHungery;
            owner.device_properties_func.Add(AddAbilityWithoutHunger);
        }

        private float AddAbilityWithoutHunger(Device device, float arg2)
        {
            if (owner.es != EatingState.Hungry && owner.es != EatingState.ExtremelyHungry)
            {
                return arg2 + desc.int_parms[0];
            }

            return arg2;
        }

        private void RapidHungery()
        {
            owner.SetEatingState(EatingState.ExtremelyHungry);
        }
    }
}
