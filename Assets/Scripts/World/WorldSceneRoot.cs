using Commons;
using Foundations;
using Foundations.SceneLoads;
using Foundations.Tickers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using World.Helpers;
using World.SafeArea;
using static Commons.Levels.Level_Mgr;

namespace World
{
    public class WorldSceneRoot : SceneRoot<WorldSceneRoot>
    {
        public ScriptableRendererFeature sr;

        [Header("控制台UI")]
        public TMP_InputField console_IPF;

        public Image black_screen;
        public SafeAreaView safe_area;

        //==================================================================================================

        protected override void on_init()
        {
            var ticker = GetComponent<Ticker_Mono>();
            ticker.init(Config.PHYSICS_TICK_DELTA_TIME);

            WorldContext._init();
            WorldContext.instance.attach();
            load_data_to_ctx();

            BattleContext.instance.Init();

            Context_Init_Helper.init_diy_context();
            Character_Module_Helper.character_module.Clear();

            Road_Info_Helper.reset();
            Encounters.Dialogs.Encounter_Dialog._init();

            sr.SetActive(false);

            base.on_init();

            FadeBlackScreen();
        }


        protected override void on_fini()
        {
            WorldContext.instance.detach();

            base.on_fini();

            BattleContext.instance.Fini();
        }


        void load_data_to_ctx()
        {
            var ctx = WorldContext.instance;
            var ds = Share_DS.instance;

            //世界id
            ds.try_get_value(Game_Mgr.key_world_id, out string world_id);
            ctx.world_id = world_id;
            Debug.Log($"========  已加载世界：{ctx.world_id}  ========");

            //世界和关卡信息
            ds.try_get_value(Game_Mgr.key_world_and_level_infos, out Dictionary<string, Struct_world_and_level_info> world_and_level_infos);
            var world_and_level_info = world_and_level_infos[world_id];

            ctx.world_progress = world_and_level_info.world_progress;
            ctx.r_game_world = world_and_level_info.r_game_world;
            ctx.r_level = world_and_level_info.r_level_array[ctx.world_progress - 1];
            ctx.pressure_threshold = ctx.r_game_world.pressure_threshold[ctx.world_progress - 1];
            ctx.pressure_growth_coef = ctx.r_game_world.pressure_growth_coef[ctx.world_progress - 1];
            Debug.Log($"========  已加载关卡：{ctx.r_level.id}_{ctx.r_level.sub_id}，类型：{(EN_level_type)ctx.world_progress}  ========");

            //场景信息
            ds.try_get_value(Game_Mgr.key_scene_index, out int scene_index);
            AutoCodes.scenes.TryGetValue($"{ctx.r_level.scene[scene_index]}", out ctx.r_scene);
            Debug.Log($"========  已加载子场景：{ctx.r_scene.id}，序列：{scene_index}/{ctx.r_level.scene.Count - 1}  ========");

            //规则：初次进入关卡时，清除玩家数据
            if (scene_index == 0)
            {
                Saves.Save_DS.instance.clear();
            }
        }


        public void btn_back_initScene()
        {
            Game_Mgr.on_exit_world(WorldContext.instance.world_progress);
            back_initScene();
        }

        public void TimeFreeze(bool b)
        {
            if (b)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }


        public void back_initScene()
        {
            SceneLoad_Utility.load_scene_async("InitScene");
        }


        public void goto_testScene()
        {
            SceneLoad_Utility.load_scene_with_loading("TestScene", true);
        }

        public void BlackScreen()
        {
            Share_DS.instance.add("is_world_fade_black", true);

            black_screen.transform.SetAsLastSibling();
            StartCoroutine("IBlackScreenCoroutine");
        }

        public void FadeBlackScreen()
        {
            if (!Share_DS.instance.try_get_value("is_world_fade_black", out bool is_world_fade_black) || !is_world_fade_black)
                return;

            Share_DS.instance.remove("is_world_fade_black");

            black_screen.transform.SetAsLastSibling();
            StartCoroutine("IFadeBlackScreenCoroutine");
        }

        private IEnumerator IBlackScreenCoroutine()
        {
            var color = black_screen.color;
            black_screen.color = new Color(color.r, color.g, color.b,0);
            black_screen.gameObject.SetActive(true);
            float alpha = 0;
            while(alpha < 1)
            {
                alpha += 1f/180f;
                black_screen.color = new Color(color.r, color.g, color.b, alpha);
                yield return new WaitForSeconds(Config.PHYSICS_TICK_DELTA_TIME);
            }

            black_screen.gameObject.SetActive(false);
            safe_area.gameObject.SetActive(true);
            safe_area.Init();

            Time.timeScale = 0;
        }

        private IEnumerator IFadeBlackScreenCoroutine()
        {
            Time.timeScale = 1;

            var color = black_screen.color;
            black_screen.color = new Color(color.r, color.g, color.b, 1);
            black_screen.gameObject.SetActive(true);
            float alpha = 1;
            while (alpha >0)
            {
                alpha -= 1f / 180f;
                black_screen.color = new Color(color.r, color.g, color.b, alpha);
                yield return new WaitForSeconds(Config.PHYSICS_TICK_DELTA_TIME);
            }

            black_screen.gameObject.SetActive(false);
            //safe_area.SetActive(true);
        }

        public void ExitSafeArea()
        {
            Helpers.Safe_Area_Helper.leave();
        }
    }
}

