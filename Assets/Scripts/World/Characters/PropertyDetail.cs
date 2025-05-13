using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace World.Characters
{
    public class PropertyDetail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject detail_panel;
        public TextMeshProUGUI detail_text;
        public TextMeshProUGUI property_name;

        [HideInInspector]
        public string detail;

        public void OnPointerEnter(PointerEventData eventData)
        {
            detail_panel.SetActive(true);

            detail_text.text = Commons.Localization_Utility.get_localization(detail);

            detail_panel.transform.position = transform.position;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            detail_panel.SetActive(false);
        }

        public void Init(string _name,string detail)
        {
            property_name.text = _name;
            this.detail = detail;
        }
    }
}
