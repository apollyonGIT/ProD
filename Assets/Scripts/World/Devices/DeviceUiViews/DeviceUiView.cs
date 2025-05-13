using Addrs;
using Commons;
using Foundations.MVVM;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Caravans;
using World.Devices.Device_AI;
using World.Devices.DeviceEmergencies;
using World.Helpers;
using World.Ui;

namespace World.Devices.DeviceUiViews
{
    public class DeviceUiView : MonoBehaviour, IDeviceView,IUiFix,IUiView
    {
        public Device owner;

        public TextMeshProUGUI deviceName;
        public Slider true_hp;
        public Slider hp;
        public GameObject select_border;
        public GameObject mask;
        public Image ui_icon;
        public GameObject fight;

        public List<DeviceStationView> stations = new();

        private const float hp_change_speed = 5;
        private float current_view_hp;

        Vector2 IUiView.pos => transform.position;

        public virtual void attach(Device owner)
        {
            this.owner = owner;

            deviceName.text = Localization_Utility.get_localization(owner.desc.name);
            true_hp.maxValue = owner.desc.hp;
            true_hp.value = owner.desc.hp;
            hp.maxValue = owner.desc.hp;
            hp.value = owner.desc.hp;

            if (!String.IsNullOrEmpty(owner.desc.icon))
            {
                Addressable_Utility.try_load_asset(owner.desc.icon, out Sprite s);
                ui_icon.sprite = s;
            }

            for (int i = 0; i < owner.module_list.Count; i++)
            {
                stations[i].Init(owner.module_list[i]);
            }

            Ui_Pos_Helper.ui_views.Add(this);
        }

        void IModelView<Device>.detach(Device owner)
        {
            if (this.owner != null)
                this.owner = null;

            Destroy(gameObject);
        }

        public virtual void init()
        {
            
        }

        public virtual void init_pos()
        {

        }


        void IDeviceView.notify_change_anim(string anim_name, bool loop)
        {
            
        }

        void IDeviceView.notify_change_anim_speed(float f)
        {
            
        }

        void IDeviceView.notify_close_collider(string _name)
        {
            
        }

        void IDeviceView.notify_hurt(int dmg)
        {
            
        }

        public virtual void notify_on_tick()
        {
            true_hp.value = owner.current_hp;
            if(current_view_hp <= owner.current_hp)
            {
                current_view_hp = owner.current_hp;
                hp.value = current_view_hp;
            }
            else
            {
                current_view_hp -= hp_change_speed * Config.PHYSICS_TICK_DELTA_TIME;
                hp.value = current_view_hp;
            }
            var b = owner.target_list.Count == 0&&owner.outrange_targets.Count == 0;

            foreach(var sta in stations)
            {
                sta.tick();
            }

            if (fight != null)
            {
                if (fight.activeInHierarchy == b)
                    fight.SetActive(!b);
            }
        }

        void IDeviceView.notify_open_collider(string _name, Action<ITarget> t1, Action<ITarget> t2, Action<ITarget> t3)
        {
            
        }

        void IDeviceView.notify_set_station(DeviceModule dm)
        {
            foreach(var st in stations)
            {
                if(st.module == dm)
                {
                    var character = Character_Module_Helper.GetModule(dm);
                    if(character == null)
                    {
                        st.SetImage(null);
                    }
                    else
                    {
                        Addressable_Utility.try_load_asset(character.desc.portrait_small, out Sprite s);
                        st.SetImage(s);
                    }
                }
            }
        }

        public void Fix()
        {
            owner.Fix(Mathf.CeilToInt(owner.desc.hp * Config.current.fix_device_job_effect * 3));
        }

        void IDeviceView.notify_disable()
        {
            mask.gameObject.SetActive(true);
        }

        void IDeviceView.notify_enable()
        {
            mask.gameObject.SetActive(false);
        }

        public virtual void OperateDrag(Vector2 dir)
        {
            owner.OperateDrag(dir);
        }

        public virtual void JoyStickOper(bool ret)
        {
            if (ret)
                owner.StartControl();
            else
                owner.EndControl();
        }

        public void notify_player_oper(bool oper)
        {
            select_border.gameObject.SetActive(oper);
        }

        void IDeviceView.notify_add_emergency(DeviceEmergency de)
        {
            if(de is DeviceFired df)
            {
                Addressable_Utility.try_load_asset("FiredView", out DeviceFiredView dfv);
                var dfe = Instantiate(dfv, transform, false);
                df.add_view(dfe);
                dfe.InitFire();
            }
            Debug.Log("发生紧急情况");
        }

        void IDeviceView.notify_remove_emergency(DeviceEmergency de)
        {
            if(de is DeviceFired df)
            {
                de.remove_all_views();
            }

            Debug.Log("解除紧急情况");
        }

        void IDeviceView.notify_attack_radius(bool show)
        {
            
        }
    }
}
