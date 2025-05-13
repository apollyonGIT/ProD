using UnityEngine;
using World.Helpers;

namespace World.Work.panels
{
    public class AimControlPanel : WorkControlPanel
    {
        public Transform control;
        public Transform center;
        public Transform aim;
        public bool isDragging = false;

        private const float speed = 5f;

        public override void open()
        {
            base.open();
            aim.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }

        public override void tick()
        {
            base.tick();
            if (isDragging)
            {
                var dir = control.transform.position - center.position;
                aim.transform.position += dir * speed * Commons.Config.PHYSICS_TICK_DELTA_TIME;
            }

            var screen_pos = WorldSceneRoot.instance.uiCamera.WorldToScreenPoint(aim.transform.position);

            var pos = WorldSceneRoot.instance.mainCamera.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, 10 - WorldSceneRoot.instance.mainCamera.transform.position.z));

        }

        public override void close()
        {
            base.close();
        }
    }
}
