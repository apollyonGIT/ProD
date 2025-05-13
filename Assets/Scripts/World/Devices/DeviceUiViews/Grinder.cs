using UnityEngine;
using UnityEngine.EventSystems;
using World.Devices.Device_AI;

namespace World.Devices.DeviceUiViews
{
    public class Grinder : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private NewBasicMelee melee;
        private Vector2 last_tick_pos;

        private bool in_area;
        public void Init(NewBasicMelee bm)
        {
            melee = bm;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!in_area)
                return;
            last_tick_pos = InputController.instance.GetScreenMousePosition();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!in_area)
                return;
            var current_pos = InputController.instance.GetScreenMousePosition();    //此刻鼠标位置
            var x_delta = Mathf.Max(0f, current_pos.x - last_tick_pos.x);

            melee.Sharp(x_delta);

            last_tick_pos = InputController.instance.GetScreenMousePosition();
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (!in_area)
                return;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            in_area = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            in_area = false;
        }
    }
}
