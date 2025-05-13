using UnityEngine;
using UnityEngine.EventSystems;

namespace World.Devices.DeviceEmergencies
{
    public class SingleFireView : MonoBehaviour,IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
    {
        public DeviceFiredView dfv;
        public float fire_value;  
        public void tick()
        {
            var scale = Mathf.Clamp(fire_value / 50f,0.2f,1f);
            transform.localScale = Vector3.one * scale * 2;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            dfv.ReduceFire();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            
        }
    }
}
