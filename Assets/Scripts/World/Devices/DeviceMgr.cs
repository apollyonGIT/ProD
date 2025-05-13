using Foundations;
using Foundations.Tickers;
using System;
using System.Collections.Generic;
using UnityEngine;
using World.Devices.Device_AI;

namespace World.Devices
{
    public class DeviceMgr : IMgr
    {
        public DevicePD pd;
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        int IMgr.priority => m_mgr_priority;
        readonly int m_mgr_priority;

        //==================================================================================================
        public Dictionary<string, Device> slots_device = new();         //初始化就应该添加所有kp，保证后续不会进行remove/add 操作

        private static Dictionary<string, Func<Device>> devices_dic = new() {
            { "Shooter_Trebuchet",() => new Shooter_Trebuchet() },
            { "Shooter_Gyroscope",()=> new Shooter_Gyroscope()},
            { "BasicWheel" ,() => new BasicWheel() },
            { "Wheel_Track" ,() => new Wheel_Track() },
            { "BasicShield",() => new BasicShield()},
            { "War_Drum",() => new Unique_War_Drum()},
            { "Catching_Flower",() => new Unique_Catching_Flower()},
            { "NewBasicShooter",()=> new NewBasicShooter()},
            { "NewBasicMelee",()=>new NewBasicMelee()},
            { "NewBasicHook",()=> new NewBasicHook()},
            { "BasicRam",()=> new BasicRam()},
        };
        public static Device create_device(string id)
        {
            if (devices_dic.ContainsKey(id))
            {
                return devices_dic[id]();
            }
            UnityEngine.Debug.LogWarning($"错误设备类型 {id}");
            return new Device();
        }
        public DeviceMgr(string name, int priority, params object[] args)
        {
            m_mgr_name = name;
            m_mgr_priority = priority;

            (this as IMgr).init(args);
        }


        /// <summary>
        /// 增加槽位和设备，一般只在初始化进行
        /// </summary>
        /// <param name="slot_name"></param>
        /// <param name="device"></param>
        public void AddDevice(string slot_name, Device device)
        {
            slots_device.Add(slot_name, device);
        }

        /// <summary>
        /// 安装设备 不增加槽位
        /// </summary>
        /// <param name="slot_name"></param>
        /// <param name="device"></param>
        public void InstallDevice(string slot_name, Device device)
        {
            slots_device[slot_name] = device;
            device.equip_time = 0;
        }

        public Device GetDevice(string id)
        {
            return create_device(id);
        }

        void IMgr.init(params object[] args)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);

            var ticker = Ticker.instance;
            ticker.add_tick(m_mgr_priority, m_mgr_name, tick);
            ticker.add_tick1(m_mgr_priority, m_mgr_name, tick1);
        }
        void IMgr.fini()
        {
            Mission.instance.detach_mgr(m_mgr_name);

            var ticker = Ticker.instance;
            ticker.remove_tick(m_mgr_name);
            ticker.remove_tick1(m_mgr_name);
        }
        void tick()
        {

            foreach (var (slot, device) in slots_device)
            {
                if (device == null)
                    continue;
                WorldContext.instance.caravan_slots.TryGetValue(slot, out var slot_t);
                if (slot_t != null)
                {
                    var v = new Vector2(slot_t.x, slot_t.y);
                    var angle = Vector2.SignedAngle(Vector2.right, WorldContext.instance.caravan_dir);
                    var new_v = Quaternion.AngleAxis(angle, Vector3.forward) * v;

                    var cp = WorldContext.instance.caravan_pos;
                    Debug.DrawLine(new Vector3(cp.x, cp.y, 10), new Vector3(cp.x + new_v.x, cp.y + new_v.y, 10), Color.blue);

                    device.position = WorldContext.instance.caravan_pos + new Vector2(new_v.x, new_v.y);
                }

                device.tick();
            }

            //设备不需要reset位置，因为车的位置已经Reset过了
            /*if (WorldContext.instance.is_need_reset)
            {
                foreach (var (_, device) in slots_device)
                {
                    if (device == null)
                        continue;
                    device.position -= new Vector2(WorldContext.instance.reset_dis, 0);
                }
            }*/
        }
        void tick1()
        {
        }

        public void Init()
        {
            foreach (var (slot, device) in slots_device)
            {
                WorldContext.instance.caravan_slots.TryGetValue(slot, out var slot_t);
                if (slot_t != null && device != null)
                {
                    var v = new Vector2(slot_t.x, slot_t.y);
                    var angle = Vector2.SignedAngle(Vector2.right, WorldContext.instance.caravan_dir);
                    var new_v = Quaternion.AngleAxis(angle, Vector3.forward) * v;

                    var cp = WorldContext.instance.caravan_pos;

                    device.position = WorldContext.instance.caravan_pos + new Vector2(new_v.x, new_v.y);
                }
                device?.InitPos();
            }
        }
    }
}