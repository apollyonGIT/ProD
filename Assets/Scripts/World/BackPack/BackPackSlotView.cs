using Addrs;
using Commons;
using Foundations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace World.BackPack
{
    public class BackPackSlotView : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
    {
        public BackPackSlot data;
        public Image loot_image;
        public GameObject select;
        public TextMeshProUGUI item_text;
        public Image item_text_bg;

        public void Init(BackPackSlot slot)
        {
            data = slot;
            if(data is OverweightBackPackSlot)
            {
                GetComponent<Image>().color = Color.red;
            }
        }

        private void Update()
        {
            if (data != null)
            {
                if (data.loot == null)
                {
                    loot_image.sprite = null;
                    item_text_bg.gameObject.SetActive(false);
                }
                else
                {
                    Addressable_Utility.try_load_asset<Sprite>(data.loot.desc.view, out var sprite);
                    loot_image.sprite = sprite;
                    item_text.text = Localization_Utility.get_localization(data.loot.desc.name);
                }
            }
        }


        public void tick()
        {
/*            if(data!= null)
            {
                if (data.loot == null)
                {
                    loot_image.sprite = null;
                    item_text_bg.gameObject.SetActive(false);
                }
                else
                {
                    Addressable_Utility.try_load_asset<Sprite>(data.loot.desc.view,out var sprite);
                    loot_image.sprite = sprite;
                    item_text.text = Localization_Utility.get_localizatin(data.loot.desc.name);
                }
            }*/
            
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Mission.instance.try_get_mgr("BackPack", out BackPackMgr bmgr);
            bmgr.SelectSlot(data);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if(data.loot!=null)
                item_text_bg.gameObject.SetActive(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            item_text_bg.gameObject.SetActive(false);
        }
    }
}
