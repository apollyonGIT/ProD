using Foundations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Commons.Levels
{
    public class Level_Mgr
    {
        public struct Struct_world_and_level_info
        {
            public AutoCodes.game_world r_game_world;
            public AutoCodes.level[] r_level_array;
            public int world_progress;
        }

        public enum EN_level_type
        {
            环境侦察 = 1,
            资源收集,
            战斗歼灭,
            首领侦察,
            消灭首领,
            已通关
        }

        //==================================================================================================

        public static void init()
        {
            Dictionary<string, Struct_world_and_level_info> world_and_level_infos = new();

            var r_game_worlds = AutoCodes.game_worlds.records;
            var r_levels = AutoCodes.levels.records;

            foreach (var (world_id, r_game_world) in r_game_worlds)
            {
                List<string> level_type_list = new() { "" };

                foreach (var e in System.Enum.GetValues(typeof(EN_level_type)))
                {
                    level_type_list.Add($"{e}");
                }

                //规则：50%交换考察、战斗关卡
                var level_group_id_array = r_game_world.level;
                var is_level_group_seq_convert = Random.Range(0, 2) == 1 ? true : false;
                if (is_level_group_seq_convert)
                {
                    var t_1 = level_group_id_array[1];
                    var t_2 = level_group_id_array[2];

                    level_group_id_array[1] = t_2;
                    level_group_id_array[2] = t_1;

                    level_type_list[2] = $"{(EN_level_type)3}";
                    level_type_list[3] = $"{(EN_level_type)2}";
                }

                r_game_world.diy_obj = level_type_list;

                //获取具体关卡信息
                List<AutoCodes.level> level_list = new();
                foreach (var level_group_id in level_group_id_array)
                {
                    var ie_levels = r_levels.Where(t => t.Key.Contains($"{level_group_id}"));
                    var level_sub_id = Random.Range(1, ie_levels.Count() + 1);

                    r_levels.TryGetValue($"{level_group_id},{level_sub_id}", out var r_level);
                    level_list.Add(r_level);
                }

                //录入
                Struct_world_and_level_info _struct = new()
                {
                    r_game_world = r_game_world,
                    r_level_array = level_list.ToArray(),
                    world_progress = 1
                };
                world_and_level_infos.Add(world_id, _struct);
            }

            Share_DS.instance.add(Game_Mgr.key_world_and_level_infos, world_and_level_infos);

            //场景序列号
            Share_DS.instance.add(Game_Mgr.key_scene_index, 0);
        }


        public static void upd_world_progress(string world_id, int world_progress)
        {
            Share_DS.instance.try_get_value(Game_Mgr.key_world_and_level_infos, out Dictionary<string, Struct_world_and_level_info> world_and_level_infos);
            var _struct = world_and_level_infos[world_id];
            _struct.world_progress = world_progress;
            world_and_level_infos[world_id] = _struct;

            Share_DS.instance.add(Game_Mgr.key_world_and_level_infos, world_and_level_infos);
        }
    }
}

