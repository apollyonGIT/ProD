using Foundations.MVVM;

namespace World.Devices.DeviceEmergencies
{
    public interface IDeviceEmergencyView : IModelView<DeviceEmergency>
    {
        void tick();
    }
    public class DeviceEmergency : Model<DeviceEmergency,IDeviceEmergencyView>
    {
        public bool removed = false;

        public virtual void tick()
        {

        }
    }
}
