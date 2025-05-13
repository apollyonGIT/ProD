using AutoCodes;
using Foundations;
using UnityEngine;
using World.BackPack;

namespace World.Characters.CharacterProperties
{
    public class CP_Food_Saver : CharacterProperty
    {
        public CP_Food_Saver(Character c, properties p) : base(c, p)
        {
        }

        public override void Init()
        {
            owner.after_eat += AfterEat;
        }

        private void AfterEat(uint food_id)
        {
            int i = Random.Range(0, 1000);
            if (i <= desc.int_parms[0])
            {
                Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);

                for (int t = 0; t < owner.per_eat; t++)
                    bmgr.AddLoot(food_id);
            }
        }
    }
}
