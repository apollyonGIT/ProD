using UnityEngine;
using UnityEngine.EventSystems;

namespace World.Work.panels
{
    public class AimController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public AimControlPanel panel;

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            panel.isDragging = true;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), eventData.position, WorldSceneRoot.instance.uiCamera, out var pos);

            GetComponent<RectTransform>().position = pos;

            limit_move();
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            panel.isDragging = false;
            transform.position = panel.center.position;
        }

        private void limit_move()
        {
            var pos = GetComponent<RectTransform>().anchoredPosition;

            if (pos.x > 100)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(100, pos.y);
            }
            else if (pos.x < -100)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, pos.y);
            }

            pos = GetComponent<RectTransform>().anchoredPosition;

            if (pos.y > 100)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x,100);
            }
            else if (pos.y < -100)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, -100);
            }
        }
    }
}
