using System;
using UnityEngine;
using World.Caravans;
using World.Enemys;

namespace World.Devices
{
    public class DeviceCollider : MonoBehaviour
    {
        public Device device;

        public string collider_name;

        public Action<ITarget> enter_event;

        public Action<ITarget> stay_event;

        public Action<ITarget> exit_event;
        
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if(device.faction == WorldEnum.Faction.player)
            {
                if(collision.TryGetComponent<EnemyHitbox>(out var eb))
                    enter_event?.Invoke(eb.view.owner);
            }
            else if(device.faction == WorldEnum.Faction.opposite)
            {
                if(collision.TryGetComponent<DeviceHitBox>(out var deviceHitBox))
                {
                    enter_event?.Invoke(deviceHitBox.view.owner);
                }
               
                if(collision.TryGetComponent<CaravanHitBox>(out var caravanHitBox))
                {
                    enter_event?.Invoke(caravanHitBox.view.owner);
                }
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            if (device.faction == WorldEnum.Faction.player)
            {
                collision.TryGetComponent<EnemyHitbox>(out var eb);
                stay_event?.Invoke(eb.view.owner);
            }
            else if (device.faction == WorldEnum.Faction.opposite)
            {
                if (collision.TryGetComponent<DeviceHitBox>(out var deviceHitBox))
                {
                    stay_event?.Invoke(deviceHitBox.view.owner);
                }

                if (collision.TryGetComponent<CaravanHitBox>(out var caravanHitBox))
                {
                    stay_event?.Invoke(caravanHitBox.view.owner);
                }
            }
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            if (device.faction == WorldEnum.Faction.player)
            {
                collision.TryGetComponent<EnemyHitbox>(out var eb);
                exit_event?.Invoke(eb.view.owner);
            }
            else if (device.faction == WorldEnum.Faction.opposite)
            {
                if (collision.TryGetComponent<DeviceHitBox>(out var deviceHitBox))
                {
                    exit_event?.Invoke(deviceHitBox.view.owner);
                }

                if (collision.TryGetComponent<CaravanHitBox>(out var caravanHitBox))
                {
                    exit_event?.Invoke(caravanHitBox.view.owner);
                }
            }
        }
    }
}
