
using Foundations.Tickers;
using UnityEngine;
using UnityEngine.EventSystems;
using World.Widgets;

namespace World.Caravans
{
    public class CaravanLeverUiView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public CaravanUiView caravanUiView;
        private Vector2 start_drag_pos;

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            start_drag_pos = InputController.instance.GetScreenMousePosition();

            Ticker.instance.do_when_tick_start += drag_tick;
        }

        private void drag_tick()
        {
            var current_drag_pos = InputController.instance.GetScreenMousePosition();

            float dy = current_drag_pos.y - start_drag_pos.y;
            Widget_DrivingLever_Context.instance.Drag_Lever(Mathf.Abs(dy) > 64f, dy > 0, true);
        }


        void IDragHandler.OnDrag(PointerEventData eventData)
        {

        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            Ticker.instance.do_when_tick_start -= drag_tick;
        }
    }
}
