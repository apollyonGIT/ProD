using Foundations.SceneLoads;
using UnityEngine;

namespace Commons
{
    public class Main : MonoBehaviour
    {
        public Config config = null;

        //==================================================================================================

        private void Awake()
        {
            var go = new GameObject("[Game]");
            DontDestroyOnLoad(go);

            Game_Mgr.on_init_game();

            SceneLoad_Utility.load_scene_async(config.first_load_scene);
        }
    }
}

