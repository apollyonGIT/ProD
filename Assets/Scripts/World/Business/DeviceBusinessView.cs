﻿using Commons;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using World.Helpers;

namespace World.Business
{
    public class DeviceBusinessView : MonoBehaviour,IPointerClickHandler
    {
        public Transform content;
        public SingleGoodsView prefab;
        public List<SingleGoodsView> goods_list = new();
        public TextMeshProUGUI coin_text;

        public DeviceBusinessPanel main_panel;

        public void Init()
        {
            for (int i = 0; i < goods_list.Count; i++)
            {
                Destroy(goods_list[i].gameObject);
            }
            goods_list.Clear();

            foreach(var goods in main_panel.owner.goods)
            {
                var single_goods = Instantiate(prefab, content, false);
                single_goods.gameObject.SetActive(true);
                single_goods.Init(goods);

                goods_list.Add(single_goods);
            }

            coin_text.text = Safe_Area_Helper.GetLootCount((int)Config.current.coin_id).ToString();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Safe_Area_Helper.TryToSellDevice(main_panel.owner);
        }

        public void UpdateGoods()
        {
            coin_text.text = Safe_Area_Helper.GetLootCount((int)Config.current.coin_id).ToString();

            for (int i = 0; i < goods_list.Count; i++)
            {
                Destroy(goods_list[i].gameObject);
            }
            goods_list.Clear();

            foreach (var goods in main_panel.owner.goods)
            {
                var single_goods = Instantiate(prefab, content, false);
                single_goods.gameObject.SetActive(true);
                single_goods.Init(goods);

                goods_list.Add(single_goods);
            }
        }
    }
}
