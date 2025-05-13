using Commons;
using Foundations;
using World.Enemys;
using World.Helpers;
using World.Progresss;
using static Commons.Attributes;

namespace World
{
    public class World_Console_Code
    {
        //[Detail("【无敌】whosyourdaddy")]
        //public static void whosyourdaddy()
        //{
        //    var ctx = WorldContext.instance;

        //    if (ctx.is_player_seckill)
        //        Debug.Log("whosyourdaddy，已激活");
        //    else
        //        Debug.Log("whosyourdaddy，已取消");
        //}


        public static void help()
        {
            Console_Code_Helper.help(typeof(World_Console_Code));
        }


        //public static void cloud()
        //{
        //    Console_Code_Helper.cloud();
        //}


        [Detail("【一键清屏】clear")]
        public static void clear()
        {
            Mission.instance.try_get_mgr("EnemyMgr", out EnemyMgr emgr);
            foreach (var cell in emgr.cells)
            {
                (cell as ITarget).hurt(null, new Attack_Data()
                {
                    atk = 9999,
                },
                out _);
            }
        }


        [Detail("【存档】save")]
        public static void save()
        {
            SL_Helper.save_game();
        }


        [Detail("【读档】load")]
        public static void load()
        {
            SL_Helper.load_game();
        }


        public static void dice()
        {
            ProgressEvent progressEvent = new();
            progressEvent.record = new();
            progressEvent.record.dialogue_graph = "dice_000";

            Encounters.Dialogs.Encounter_Dialog.instance.init(progressEvent, "Dice_Game_Window");
        }


        [Detail("【测试敌方小车】car")]
        public static void car()
        {
            if (!Mission.instance.try_get_mgr("EnemyMgr", out EnemyMgr mgr)) return;

            mgr.pd.add_enemy_directly_req(0, 211101u, new(5, 0), "None");
        }
    }
}

