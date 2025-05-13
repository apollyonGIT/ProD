using Addrs;
using AutoCodes;
using Commons;
using Foundations;
using UnityEngine;
using World.Devices;
using World.Devices.DeviceUiViews;
using World.Devices.DeviceViews;

namespace World.Helpers
{
    public class Device_Slot_Helper
    {
        public static void InstallDevice(string device_id,string slot)
        {
            WorldContext ctx = WorldContext.instance;

            Mission.instance.try_get_mgr(Config.DeviceMgr_Name, out DeviceMgr dmgr);
            device_alls.TryGetValue(device_id, out var rc1);
            var device = dmgr.GetDevice(rc1.behavior_script);

            device.InitData(rc1);
            InstallDevice(device,slot);
        }

        public static Device RemoveDevice(string slot)
        {
            Device remove_device = null;
            Mission.instance.try_get_mgr(Config.DeviceMgr_Name, out DeviceMgr dmgr);
            dmgr.slots_device.TryGetValue(slot, out remove_device);
            if (remove_device != null)
            {
              
                dmgr.slots_device[slot] = null;
                remove_device.remove_all_views();
            }
            return remove_device;
        }
        
        public static Device InstallDevice(Device device,string slot)
        {
            WorldContext ctx = WorldContext.instance;

            var remove_device = RemoveDevice(slot);
            Mission.instance.try_get_mgr(Config.DeviceMgr_Name, out DeviceMgr dmgr);

            Addressable_Utility.try_load_asset<DeviceView>(device.desc.prefeb, out var d1view);
            var device_view = GameObject.Instantiate(d1view, dmgr.pd.transform, false);
            device.add_view(device_view);

            Addressable_Utility.try_load_asset<DeviceUiView>(device.desc.ui_prefab, out var device_ui_view);
            var device_ui = GameObject.Instantiate(device_ui_view, dmgr.pd.ui_content.transform, false);
            device.add_view(device_ui);

            device.key_points.Clear();
            foreach (var kp in device_view.dkp)
            {
                device.key_points.Add(kp.key_name, kp.transform);
            }

            dmgr.InstallDevice(slot, device);

            WorldContext.instance.caravan_slots.TryGetValue(slot, out var slot_t);
            if (slot_t != null)
            {
                var v = new Vector2(slot_t.x, slot_t.y);
                var angle = Vector2.SignedAngle(Vector2.right, WorldContext.instance.caravan_dir);
                var new_v = Quaternion.AngleAxis(angle, Vector3.forward) * v;

                var cp = WorldContext.instance.caravan_pos;

                device.position = WorldContext.instance.caravan_pos + new Vector2(new_v.x, new_v.y);
            }
            device.InitPos();

            //规则：设备安装时，添加其重量
            ctx.total_weight += device.desc.weight;

            //规则：如果为轮子，添加相关参数
            if (device_wheels.TryGetValue(device.desc.id.ToString()+",0", out var wheel_rc))
            {
                ctx.tractive_force_max = wheel_rc.tractive_force_max;
                ctx.feedback_0 = wheel_rc.feedback_0;
                ctx.feedback_1 = wheel_rc.feedback_1;
            }


            device.Start();

            return remove_device;
        }

        public static Device GetDevice(string slot)
        {
            Mission.instance.try_get_mgr(Config.DeviceMgr_Name, out DeviceMgr dmgr);
            dmgr.slots_device.TryGetValue(slot, out var device);
            return device;
        }
    }
}
