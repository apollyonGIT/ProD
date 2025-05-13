using Commons.Levels;
using Foundations;
using Foundations.Tickers;
using UnityEngine;

namespace Commons
{
    public class Game_Mgr
    {
        public const string key_world_and_level_infos = "world_and_level_infos";
        public const string key_world_id = "world_id";
        public const string key_scene_index = "scene_index";

        public const string key_is_continue_game = "is_continue_game";

        //==================================================================================================

        public static void on_init_game()
        {
            Share_DS._init();

            Share_DS.instance.add(key_is_continue_game, false);
        }


        public static void on_start_new_game()
        {
            Level_Mgr.init();

            Share_DS.instance.add(key_is_continue_game, false);
        }


        public static void on_continue_game()
        {
        }


        public static void on_exit_game()
        {
            
        }


        public static void on_enter_world(params object[] args)
        {
            var world_id = (string)args[0];
            Share_DS.instance.add(key_world_id, world_id);
        }


        public static void on_exit_world(params object[] args)
        {
            Ticker.instance.is_end = true;

            Share_DS.instance.try_get_value(key_world_id, out string world_id);
            var world_progress = (int)args[0];

            Level_Mgr.upd_world_progress(world_id, world_progress);

            Share_DS.instance.add(key_is_continue_game, true);
        }
    }
}

