using Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World.Encounters.Dialogs
{
    public class Encounter_Dialog_Window : MonoBehaviour
    {
        public Encounter_Dialog_Window_Title title;
        public Encounter_Dialog_Window_Dialog dialog;

        public Encounter_Dialog_Window_Btn_Option btn_option_model;

        List<Encounter_Dialog_Window_Btn_Option> m_btn_options = new();
        public List<Encounter_Dialog_Window_Btn_Option> btn_options => m_btn_options;

        public GameObject[] outter_go_array;

        //==================================================================================================

        public void init(params object[] args)
        {
            var uname = (string)args[0];

            title.content.text = Localization_Utility.get_localization_dialog((string)args[1]);
            dialog.content.text = Localization_Utility.get_localization_dialog((string)args[2]);
            var btn_option_ac_list = (List<(string, Func<object>)>)args[3];

            for (int i = 0; i < btn_option_ac_list.Count; i++)
            {
                var btn_option = Instantiate(btn_option_model, btn_option_model.transform.parent);
                btn_option.gameObject.SetActive(true);
                m_btn_options.Add(btn_option);
            }

            int option_index = 0;
            foreach (var (option_name, option_ac) in btn_option_ac_list)
            {
                var btn_info = m_btn_options[option_index];
                btn_info.btn_option_name.text = Localization_Utility.get_localization_dialog(option_name);
                btn_info.btn_option.onClick.AddListener(
                    () =>
                    {
                        foreach (var btn_click_ac in btn_info.btn_click_ac_list)
                        {
                            btn_click_ac?.Invoke();
                        }

                        fini();
                        option_ac?.Invoke();
                    });

                option_index++;
            }

            Encounter_Dialog.instance.opening_window = this;

            var console = (string)args[4];
            Encounter_Dialog_Console.decode_console(this, console, uname);

            //加载外部模块
            if (Encounter_Dialog.instance.cache_dic.TryGetValue("outter_go_infos", out var _outter_go_infos))
            {
                var outter_go_infos = (Dictionary<string, bool>)_outter_go_infos;

                foreach (var (go_name, go_active) in outter_go_infos)
                {
                    outter_go_array.Where(t => t.name == go_name).First().SetActive(go_active);
                }
            }
            else
            {
                Encounter_Dialog.instance.cache_dic.Add("outter_go_infos", new Dictionary<string, bool>());
            }
        }


        public void fini()
        {
            DestroyImmediate(gameObject);

            var e = Encounter_Dialog.instance;
            e.opening_window = null;
        }
    }
}

