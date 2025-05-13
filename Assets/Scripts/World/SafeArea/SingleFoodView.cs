using Addrs;
using AutoCodes;
using Commons;
using Foundations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using World.BackPack;
using World.Characters;

namespace World.SafeArea
{
    public class SingleFoodView : MonoBehaviour, IPointerClickHandler
    {
        public FoodPanel main_panel;
        public Image food_image;
        public TextMeshProUGUI food_amount;
        public TextMeshProUGUI food_name;
        public Character character;

        public uint food_id;
        public void Init(uint food_id, Character c, FoodPanel fp)
        {
            main_panel = fp;
            this.food_id = food_id;
            character = c;
            loots.TryGetValue(food_id.ToString(), out var rec);
            Addressable_Utility.try_load_asset<Sprite>(rec.view, out Sprite s);
            if (s != null)
            {
                food_image.sprite = s;
            }

            Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);
            food_amount.text = $"{bmgr.GetLootAmount((int)food_id)}/{character.per_eat}";
            food_name.text = Localization_Utility.get_localization(rec.name);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);
            if (character.eat_times > 0 && bmgr.GetLootAmount((int)food_id) >= character.per_eat)
            {
                bmgr.RemoveLoot((int)food_id, character.per_eat);
                character.Eat(food_id);
                main_panel.UpdatePanel();
            }
        }

        public void UpdateView()
        {
            Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);
            food_amount.text = $"{bmgr.GetLootAmount((int)food_id)}/{character.per_eat}";
        }
    }
}
