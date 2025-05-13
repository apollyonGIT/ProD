using Commons;
using Foundations;
using Foundations.Tickers;
using UnityEngine;

namespace World.Progresss
{
    public class Progress_Context : Singleton<Progress_Context>
    {
        WorldContext ctx;

        //==================================================================================================

        public void attach()
        {
            ctx = WorldContext.instance;

            Ticker.instance.do_when_tick_start += tick;
        }


        public void detach()
        {
            Ticker.instance.do_when_tick_start -= tick;
        }


        void tick()
        {
            if (ctx.scene_remain_progress <= 0)
            {
                detach();

                var ds = Share_DS.instance;
                ds.try_get_value(Game_Mgr.key_scene_index, out int scene_index);

                //没有后续场景
                if (ctx.r_level.scene.Count == scene_index + 1)
                {
                    leave_without_next_scene();
                    return;
                }
                
                //存在后续场景
                Helpers.Safe_Area_Helper.enter();
            }
        }


        //规则：如果没有后续场景，返回主菜单，世界进度 + 1
        public void leave_without_next_scene()
        {
            Game_Mgr.on_exit_world(ctx.world_progress + 1);
            Share_DS.instance.add(Game_Mgr.key_scene_index, 0);

            WorldSceneRoot.instance.back_initScene();
        }


        //规则：存在后续场景，场景序列号 + 1，并再次进入关卡
        public void leave_with_next_scene()
        {
            Saves.Save_DS.instance.save();

            Game_Mgr.on_exit_world(ctx.world_progress);

            var ds = Share_DS.instance;
            ds.try_get_value(Game_Mgr.key_scene_index, out int scene_index);
            Share_DS.instance.add(Game_Mgr.key_scene_index, ++scene_index);

            Game_Mgr.on_enter_world(ctx.world_id);
            WorldSceneRoot.instance.goto_testScene();
        }
    }
}

