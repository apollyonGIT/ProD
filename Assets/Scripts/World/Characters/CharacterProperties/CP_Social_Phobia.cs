using AutoCodes;
using Commons;
using Foundations;
using System.Linq;

namespace World.Characters.CharacterProperties
{
    public class CP_Social_Phobia : CharacterProperty
    {
        public CP_Social_Phobia(Character c, properties p) : base(c, p)
        {
        }

        public override void Start()
        {
            float mood_delta = desc.int_parms[0];

            Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
            foreach (var c in cmgr.characters)
            {
                if (c != owner)
                {
                    c.UpdateMood(desc.int_parms[3]);

                    if (c.character_properties.Any(prop => prop is CP_Social_Phobia))
                    {
                        mood_delta += desc.int_parms[1];
                    }

                    if (c.character_properties.Any(prop => prop is CP_Gregarious))
                    {
                        mood_delta += desc.int_parms[2];
                    }
                }
            }

            owner.UpdateMood(mood_delta);
        }
    }
}
