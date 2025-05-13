using Addrs;
using AutoCodes;
using Foundations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace World.Characters
{
    public class CharacterView : MonoBehaviour, IPointerClickHandler
    {
        public Character character;
        public GameObject panel;

        public TextMeshProUGUI role_name;

        public GameObject focus, working;

        public Transform properties_content;
        public PropertyDetail property_prefab;
        public void init(Character c)
        {
            character = c;

            Addressable_Utility.try_load_asset(c.desc.portrait_big, out Sprite s);
            if (s != null)
                panel.GetComponent<Image>().sprite = s;

            role_name.text = Commons.Localization_Utility.get_localization(c.desc.name);

            foreach(var p in c.character_properties)
            {
                propertiess.TryGetValue(p.desc.id.ToString(), out var p_record);
                var pp = Instantiate(property_prefab, properties_content, false);
                pp.Init(Commons.Localization_Utility.get_localization(p_record.name),Commons.Localization_Utility.get_localization(p_record.desc));
                pp.gameObject.SetActive(true);
            }
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Mission.instance.try_get_mgr(Commons.Config.CharacterMgr_Name, out CharacterMgr cmgr);
            cmgr.SelectCharacter(character);
        }

        public void tick()
        {

        }
    }
}
