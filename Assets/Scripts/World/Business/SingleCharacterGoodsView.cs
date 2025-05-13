using Addrs;
using AutoCodes;
using Commons;
using Foundations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Characters;
using World.Helpers;

namespace World.Business
{
    public class SingleCharacterGoodsView : MonoBehaviour
    {
        public Business b;

        public Image icon;
        public TextMeshProUGUI name_text;
        public Image price_image;
        public TextMeshProUGUI price_text;

        public Transform properties_content;
        public PropertyDetail property_prefab;

        public GoodsData data;

        public void Init(GoodsData g,Business b)
        {
            this.b = b;
            data = g;
            roles.TryGetValue(g.goods_id.ToString(), out role c);
            name_text.text = Localization_Utility.get_localization(c.name);
            Addressable_Utility.try_load_asset<Sprite>(c.portrait_big, out var s);
            icon.sprite = s;

            loots.TryGetValue(g.price_id.ToString(), out loot l);
            Addressable_Utility.try_load_asset<Sprite>(l.view, out var s1);
            price_image.sprite = s1;
            price_text.text = g.price_count.ToString();

            var character = (data.obj as Character);

            foreach (var p in character.character_properties)
            {
                propertiess.TryGetValue(p.desc.id.ToString(), out var p_record);
                var pp = Instantiate(property_prefab, properties_content, false);
                pp.Init(Localization_Utility.get_localization(p_record.name),Localization_Utility.get_localization(p_record.desc));
                pp.gameObject.SetActive(true);
            }
        }

        public void Buy()
        {
            Mission.instance.try_get_mgr(Config.CharacterMgr_Name, out CharacterMgr cmgr);
            if (data.price_count <= Safe_Area_Helper.GetLootCount(data.price_id))
            {
                Safe_Area_Helper.SpendLootCount(data.price_id, data.price_count);
                b.RemoveGoods(data.goods_id);
                cmgr.AddCharacter(data.obj as Character);
            }
        }
    }
}
