using Addrs;
using Commons;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Characters;

namespace World.SafeArea
{
    public class RoleFoodPanel : MonoBehaviour
    {
        public FoodPanel main_panel;
        public Transform content;
        public SingleFoodView prefab;
        public List<SingleFoodView> foodViews = new List<SingleFoodView>();

        public Character data;

        public Image role_image;
        public TextMeshProUGUI role_mood;
        public TextMeshProUGUI role_state;

        public void Init(Character c,FoodPanel fp)
        {
            main_panel = fp;
            data = c;
            Addressable_Utility.try_load_asset(c.desc.portrait_big, out Sprite s);
            if(s != null)
                role_image.sprite = s;
            role_mood.text = c.current_mood.ToString();
            role_state.text = c.es.ToString();

            var f1 = Instantiate(prefab,content,false);
            f1.Init(Config.current.food_id_1,c,fp);
            f1.gameObject.SetActive(true);
            foodViews.Add(f1);

            var f2 = Instantiate(prefab, content, false);
            f2.Init(Config.current.food_id_2, c, fp);
            f2.gameObject.SetActive(true);
            foodViews.Add(f2);

            var f3 = Instantiate(prefab, content, false);
            f3.Init(Config.current.food_id_3, c, fp);
            f3.gameObject.SetActive(true);
            foodViews.Add(f3);

            var f4 = Instantiate(prefab, content, false);
            f4.Init(Config.current.food_id_4, c, fp);
            f4.gameObject.SetActive(true);
            foodViews.Add(f4);
        }


        public void UpdatePanel()
        {
            role_mood.text = data.current_mood.ToString();
            role_state.text = data.es.ToString();

            foreach (var fv in foodViews)
            {
                fv.UpdateView();
            }
        }
    }
}
