using System.Collections.Generic;
using UnityEngine;

namespace World.Devices.DeviceEmergencies
{
    public class DeviceFiredView : DeviceEmergencyView
    {
        public SingleFireView prefab;
        public List<SingleFireView> fire_list = new();
        public void InitFire()
        {
            var f = Instantiate(prefab, transform,false);
            var rect = GetComponent<RectTransform>().rect;
            var rnd_offset = new Vector2(Random.Range(-rect.width/2,rect.width/2),Random.Range(-rect.height / 2,rect.height / 2));

            f.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition + rnd_offset;
            fire_list.Add(f);

            f.gameObject.SetActive(true);
        }

        public override void tick()
        {
            if(fire_list.Count != 0 && owner is DeviceFired)
            {
                fire_list[0].fire_value = (owner as DeviceFired).fire_value;
            }
            foreach(var fire in fire_list)
            {
                fire.tick();
            }
        }

        public void ReduceFire()
        {
            if(owner is DeviceFired df)
            {
                df.ChangeFire(-20);
            }
        }
    }
}
