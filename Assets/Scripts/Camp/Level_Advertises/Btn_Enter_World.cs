using Commons;
using Foundations.SceneLoads;
using TMPro;
using UnityEngine;

namespace Camp.Level_Advertises
{
    public class Btn_Enter_World : MonoBehaviour
    {
        public TextMeshProUGUI tx_world_info;
        public TextMeshProUGUI tx_level_info;

        string m_world_id;

        bool is_world_gameover;

        //==================================================================================================

        public void init(params object[] args)
        {
            m_world_id = (string)args[0];

            tx_world_info.text = (string)args[1];
            tx_level_info.text = (string)args[2];

            is_world_gameover = tx_level_info.text == "已通关";
        }


        public void btn_enter_world()
        {
            if (is_world_gameover)
            {
                Debug.Log("世界已通关！");
                return;
            }

            Game_Mgr.on_enter_world(m_world_id);
            Debug.Log(m_world_id);

            SceneLoad_Utility.load_scene_with_loading("TestScene");
        }
    }
}

