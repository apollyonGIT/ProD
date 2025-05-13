using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using World.Characters;
using World.Devices.Device_AI;
using World.Helpers;

namespace World.Devices.DeviceUiViews
{
    public class DeviceStationView : MonoBehaviour,IPointerClickHandler
    {
        public DeviceModule module;

        public Image character_image;

        public CharacterMind mind;

        public void Init(DeviceModule dm)
        {
            module = dm;
        }

        public void SetImage(Sprite s)
        {
            character_image.sprite = s;
            character_image.color = s == null ? Color.clear : Color.white;

            mind.gameObject.SetActive(s != null);
        }

        public void tick()
        {
            var role = Character_Module_Helper.GetModule(module);
            if(role!= null)
            {
                //mind.UpdateMind(role.current_mood/(float)role.desc.power);
            }
                
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (module == null)
                return;

            if(eventData.button == PointerEventData.InputButton.Right)
            {
                Character_Module_Helper.EmptyModule(module);
            }
            else
            {
                Character_Module_Helper.SetModule(module);
            }
        }
    }
}
