using UnityEngine;

namespace World.Devices.DeviceViews
{
    public class ShooterView : DeviceView
    {
        public LineRenderer aim_line;

        public LineRenderer radius_line;
        public LineRenderer inner_radius_line;

        public override void notify_on_tick()
        {
            base.notify_on_tick();

            if (owner.player_oper && dkp.Count > 0 && aim_line != null)
            {
                aim_line.positionCount = 2;
                var cv = WorldContext.instance.caravan_dir.normalized;
                var bv = owner.bones_direction["roll_control"];
                var v1 = dkp[0].transform.position;
                var v2 = new Vector3(v1.x + (bv.x * cv.x - bv.y * cv.y) * 100, v1.y + (bv.x * cv.y + bv.y * cv.x) * 100, v1.z);
                aim_line.SetPosition(0, v1);
                aim_line.SetPosition(1, v2);
            }


            if(radius_line!=null && radius_line.positionCount != 0)
            {
                DrawCircle(transform.position, owner.desc.basic_range.Item2, radius_line);
            }

            if(inner_radius_line!=null && inner_radius_line.positionCount != 0)
            {
                DrawCircle(transform.position, owner.desc.basic_range.Item1, inner_radius_line);
            }
        }

        public override void notify_player_oper(bool oper)
        {
            if (aim_line != null)
            {
                if (oper)
                    aim_line.enabled = true;
                else
                    aim_line.enabled = false;
            }
        }

        public override void notify_attack_radius(bool show)
        {
            if (show)
            {
                DrawCircle(transform.position, owner.desc.basic_range.Item2, radius_line);
                DrawCircle(transform.position, owner.desc.basic_range.Item1, inner_radius_line);
            }
            else
            {
                radius_line.positionCount = 0;
                inner_radius_line.positionCount = 0;
            }
        }

        private void DrawCircle(Vector3 center,float radius,LineRenderer lineRenderer)
        {
            lineRenderer.positionCount = 181;
            float x;
            float y;
            float z = 10;
            float angle = 0f ;
            for (int i = 0; i < 181; i++)
            {
                x = center.x + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                y = center.y + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                lineRenderer.SetPosition(i, new Vector3(x, y, z));
                angle += (360f /180);
            }
        }
    }
}
