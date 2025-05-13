using Commons;
using Foundations;
using System.Collections.Generic;
using World.Characters;
using World.Work;

namespace World.Helpers
{
    public class Cubicle_Info_Helper
    {
        public static Dictionary<Character, Cubicle> character_info = new();

        public static Character GetSelectCharacter()
        {
            Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
            return cmgr.select_character;
        }

        public static void SetSelectCharacter(Character c)
        {
            Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
            cmgr.SelectCharacter(c);
        }

        /// <summary>
        /// 让某人在某处工作,把某人原本的工作清空
        /// </summary>
        /// <param name="c"></param>
        /// <param name="cubicle"></param>
        public static void SetCubicle(Character c, Cubicle cubicle)
        {
            Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
            if (character_info.ContainsKey(c))
            {
                var past_cub = character_info[c];
                past_cub.owner.owner.ChangeCubicle(past_cub.owner, past_cub, null);
                past_cub.has_worker = false;           //原工位变为无人

                character_info[c] = cubicle;                    //指定新工作
                cubicle.has_worker = true;                      //新工作有人

                cmgr.SetCharacterWorking(c, true);
                cubicle.owner.owner.ChangeCubicle(cubicle.owner, cubicle, c.desc.portrait_small);

                cmgr.CancelSelectCharacter(c);
                return;
            }
            character_info.Add(c, cubicle);
            cubicle.has_worker = true;
            cubicle.owner.owner.ChangeCubicle(cubicle.owner, cubicle, c.desc.portrait_small);
            cmgr.SetCharacterWorking(c, true);
            cmgr.CancelSelectCharacter(c);
        }

        /// <summary>
        /// 取消工位上的人的工作
        /// </summary>
        /// <param name="cubicle"></param>
        public static void EmptyCubicle(Cubicle cubicle)
        {
            var remove_character = new Character();

            foreach (var (character, cub) in character_info)
            {
                if (cub == cubicle)
                {
                    cub.has_worker = false;
                    remove_character = character;
                    cubicle.owner.owner.ChangeCubicle(cubicle.owner, cubicle, null);
                }
            }
            character_info.Remove(remove_character);

            Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
            cmgr.SetCharacterWorking(remove_character, false);
        }

        public static Character GetCubicle(Cubicle cubicle)
        {
            foreach (var (character, cub) in character_info)
            {
                if (cub == cubicle)
                {
                    return character;
                }
            }
            return null;
        }
    }
}
