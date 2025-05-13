using Addrs;
using Commons;
using Foundations;
using Foundations.MVVM;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Caravans;
using World.Helpers;

namespace World.Work
{
    public class WorkView : MonoBehaviour, IWorkView
    {
        public Work owner;

        public TextMeshProUGUI name_text;
        public Slider true_hp;
        public Slider hp;
        public GameObject hp_block;

        public JobView job_prefab;
        public List<JobView> jobs = new();
        public Transform job_content;

        private const float hp_change_speed = 5;
        private float current_view_hp;

        void IModelView<Work>.attach(Work owner)
        {
            this.owner = owner;
        }

        void IModelView<Work>.detach(Work owner)
        {
            if (this.owner != null)
            {
                owner = null;
            }
            Destroy(gameObject);
        }

        void IWorkView.notify_add_job(Job job)
        {
            var jv = Instantiate(job_prefab, job_content, false);
            jv.Init(job);
            jv.gameObject.SetActive(true);
            jobs.Add(jv);
        }

        public virtual void tick()
        {
            hp.GetComponent<RectTransform>().position = true_hp.GetComponent<RectTransform>().position;
            hp.GetComponent<RectTransform>().sizeDelta = true_hp.GetComponent<RectTransform>().sizeDelta;

            var device_hp = 100;// Device_Slot_Helper.GetDeviceHp(owner);
            if (device_hp != -1)
            {
                true_hp.value = device_hp;
                if (current_view_hp <= device_hp)
                {
                    current_view_hp = device_hp;
                    hp.value = current_view_hp;
                }
                else
                {
                    current_view_hp -= hp_change_speed * Config.PHYSICS_TICK_DELTA_TIME;
                    hp.value = current_view_hp;
                }
            }
            else
            {
                true_hp.value = WorldContext.instance.caravan_hp;
                if (current_view_hp <= WorldContext.instance.caravan_hp)
                {
                    current_view_hp = WorldContext.instance.caravan_hp;
                    hp.value = current_view_hp;
                }
                else
                {
                    current_view_hp -= hp_change_speed * Config.PHYSICS_TICK_DELTA_TIME;
                    hp.value = current_view_hp;
                }
            }

            foreach (var jv in jobs)
            {
                jv.Tick();
            }
        }

        public virtual void init(Work work)
        {
            owner = work;
            //GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.RandomRange(-Screen.width / 2, Screen.width / 2), -Screen.height / 2 + GetComponent<RectTransform>().rect.height / 2);
            var device_name = "";// Device_Slot_Helper.GetDeviceName(owner);
            var device_hp = 100;// Device_Slot_Helper.GetDeviceHp(owner);
            if (device_name != null)
            {
                name_text.text = Localization_Utility.get_localization(device_name);
                true_hp.maxValue = device_hp;
                true_hp.value = device_hp;
                hp.maxValue = device_hp;
                hp.value = device_hp;
                current_view_hp = device_hp;
            }
            else
            {
                Mission.instance.try_get_mgr(Config.CaravanMgr_Name, out CaravanMgr cmgr);
                name_text.text = Localization_Utility.get_localization(cmgr.cell._desc.name);
                true_hp.maxValue = cmgr.cell._desc.hp;
                true_hp.value = WorldContext.instance.caravan_hp;
                hp.maxValue = cmgr.cell._desc.hp;
                hp.value = WorldContext.instance.caravan_hp;
                current_view_hp = WorldContext.instance.caravan_hp;
            }

            foreach (var job in owner.jobs)
            {
                var jv = Instantiate(job_prefab, job_content, false);
                jv.Init(job);
                jv.gameObject.SetActive(true);
                jobs.Add(jv);
            }
        }

        void IWorkView.notify_remove_job(Job job)
        {
            for (int i = jobs.Count - 1; i >= 0; i--)
            {
                var jv = jobs[i];
                if (jobs[i].data == job)
                {
                    jobs.RemoveAt(i);
                    //需要对工位上的人进行处理,先搁置
                    Destroy(jv.gameObject);
                }
            }
        }

        void IWorkView.notify_character_work(Job job, Cubicle cubicle, string sprite_path)
        {
            foreach (var jv in jobs)
            {
                if (jv.data == job)
                {
                    foreach (var c in jv.cubicles)
                    {
                        if (c.data == cubicle)
                        {
                            if (sprite_path != null && Addressable_Utility.try_load_asset<Sprite>(sprite_path, out var s))
                                c.cubicle_image.sprite = s;
                            else
                                c.cubicle_image.sprite = null;
                        }
                    }
                }
            }
        }

        void IWorkView.notify_device_destroy()
        {
            hp_block.gameObject.SetActive(true);
        }

        void IWorkView.notify_device_repair()
        {
            hp_block.gameObject.SetActive(false);
        }
    }
}
