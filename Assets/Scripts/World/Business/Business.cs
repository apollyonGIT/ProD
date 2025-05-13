using AutoCodes;
using Commons;
using Foundations;
using Foundations.Excels;
using Foundations.MVVM;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Characters;
using World.Devices;
using World.Helpers;

namespace World.Business
{
    public interface IBusinessView : IModelView<Business>
    {
        void update_goods();
        void init();
    }

    public class Business : Model<Business, IBusinessView>
    {
        public List<GoodsData> goods = new();
        public virtual void Init(uint business_id)
        {
            shops.TryGetValue(business_id.ToString(), out shop record);
            var total_weight = 0;
            var shop_count = Random.Range(record.goods_count_rnd.Item1, record.goods_count_rnd.Item2 + 1);

            List<(uint, int)> goods_record_list = new();


            if (record.shop_type.value == Shop_Type.EN_Shop_Type.角色)
            {
                Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
                foreach (var (goods_id, goods_weight) in record.goods_rnd_pool)
                {
                    if (cmgr.characters.Any(c => c.desc.id == goods_id))
                    {
                        //已经有的角色不要加入随机池了
                        continue;
                    }
                    total_weight += goods_weight;

                    goods_record_list.Add((goods_id, goods_weight));
                }
            }
            else
            {
                foreach (var (goods_id, goods_weight) in record.goods_rnd_pool)
                {
                    total_weight += goods_weight;

                    goods_record_list.Add((goods_id, goods_weight));
                }
            }

            int loop_time = 0;      //防止死循环

            while (shop_count > 0 && loop_time++ <= 50)
            {

                var rnd_weight = Random.Range(0, total_weight);
                for (int i = goods_record_list.Count - 1; i >= 0; i--)
                {
                    var goods_rec = goods_record_list[i];
                    if (goods_rec.Item2 > rnd_weight)
                    {
                        var price_weight_total = 0;

                        foreach (var (_, _, price) in record.possible_price)
                        {
                            price_weight_total += price;
                        }

                        var price_weight = Random.Range(0, price_weight_total);

                        foreach (var (price_id, price_count, price) in record.possible_price)
                        {
                            price_weight -= price;
                            if (price_weight <= 0)
                            {
                                var gd = new GoodsData
                                {
                                    goods_type = GetGoodsType(record.shop_type.value),
                                    goods_id = goods_rec.Item1,
                                    price_id = (int)price_id,
                                    price_count = price_count,
                                };
                                gd.InitObj();

                                goods.Add(gd);

                                break;
                            }
                        }
                        total_weight -= goods_record_list[i].Item2;
                        goods_record_list.RemoveAt(i);
                        shop_count--;
                        break;
                    }
                    else
                    {
                        rnd_weight -= goods_rec.Item2;
                    }
                }
            }

            foreach (var view in views)
            {
                view.init();
            }

            foreach (var view in views)
            {
                view.update_goods();
            }
        }

        public void AddGoods(object obj, GoodsType goods_type, uint goods_id, int price_id, int price_count)
        {
            goods.Add(new GoodsData
            {
                obj = obj,
                goods_type = goods_type,
                goods_id = goods_id,
                price_id = price_id,
                price_count = price_count,
            });

            foreach (var view in views)
            {
                view.update_goods();
            }
        }

        protected GoodsType GetGoodsType(Shop_Type.EN_Shop_Type st)
        {
            switch (st)
            {
                case Shop_Type.EN_Shop_Type.设备:
                    return GoodsType.device;
                case Shop_Type.EN_Shop_Type.角色:
                    return GoodsType.role;
                case Shop_Type.EN_Shop_Type.遗物掉落:
                    return GoodsType.drops;
                case Shop_Type.EN_Shop_Type.遗物商人:
                    return GoodsType.relic;
                default:
                    return GoodsType.device;
            }
        }

        public void RemoveGoods(uint goods_id)
        {
            for (int i = goods.Count - 1; i >= 0; i--)
            {
                if (goods[i].goods_id == goods_id)
                {
                    goods.RemoveAt(i);
                    break;
                }
            }

            foreach (var view in views)
            {
                view.update_goods();
            }
        }
    }

    public class GoodsData
    {
        public object obj;

        public GoodsType goods_type;
        public uint goods_id;
        public int price_id;
        public int price_count;

        public void InitObj()
        {
            switch (goods_type)
            {
                case GoodsType.device:
                    var device = new Device();
                    device_alls.TryGetValue($"{goods_id},0", out var device_record);
                    device.InitData(device_record);
                    obj = device;
                    break;
                case GoodsType.role:
                    roles.TryGetValue(goods_id.ToString(), out var character_record);
                    var character = Safe_Area_Helper.CreateCharacter(character_record);
                    obj = character;
                    break;
                case GoodsType.relic:
                    break;
                case GoodsType.drops:
                    break;
                default:
                    break;
            }
        }
    }

    public enum GoodsType
    {
        device,
        role,
        relic,
        drops,
    }
}
