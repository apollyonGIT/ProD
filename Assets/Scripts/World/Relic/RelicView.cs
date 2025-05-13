using Addrs;
using Commons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace World.Relic
{
    public class RelicView :MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        public Relic data;
        public Image icon;

        public GameObject description_obj;
        public TextMeshProUGUI relic_name;
        public TextMeshProUGUI relic_description;
        public void Init(Relic r)
        {
            data = r;
            if (data.desc.portrait != null)
            {
                Addressable_Utility.try_load_asset<Sprite>(data.desc.portrait, out var s);
                icon.sprite = s;
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            description_obj.gameObject.SetActive(true);
            description_obj.transform.position = transform.position;
            relic_name.text = Localization_Utility.get_localization(data.desc.name);
            relic_description.text = Localization_Utility.get_localization(data.desc.description);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            description_obj.gameObject.SetActive(false);
        }
    }
}
