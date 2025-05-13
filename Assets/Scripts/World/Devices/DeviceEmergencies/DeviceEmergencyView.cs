using Foundations.MVVM;
using UnityEngine;

namespace World.Devices.DeviceEmergencies
{
    public class DeviceEmergencyView : MonoBehaviour, IDeviceEmergencyView
    {
        public DeviceEmergency owner;

        void IModelView<DeviceEmergency>.attach(DeviceEmergency owner)
        {
            this.owner = owner;
        }

        void IModelView<DeviceEmergency>.detach(DeviceEmergency owner)
        {
            if(this.owner!=null)
                this.owner = null;
            Destroy(gameObject);
        }

        public virtual void tick()
        {
            
        }
    }
}
