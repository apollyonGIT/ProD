using AutoCodes;
using World.Devices;

namespace World.Characters.CharacterProperties
{
    public class CP_Metabolism_None : CharacterProperty
    {
        public CP_Metabolism_None(Character c, properties p) : base(c, p)
        {
        }

        public override void Init()
        {
            owner.enter_safe_area += SlowHungery;
            owner.device_properties_func.Add(ReduceAbility);
        }

        private float ReduceAbility(Device device, float arg2)
        {
            return arg2 + desc.int_parms[0];
        }

        private void SlowHungery()
        {
            owner.SetEatingState(EatingState.Full);
        }
    }
}
