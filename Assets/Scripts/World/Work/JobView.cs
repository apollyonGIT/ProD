using Commons;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Work.jobs;

namespace World.Work
{
    public class JobView : MonoBehaviour
    {
        public CubicleView cubicle_prefab;
        public List<CubicleView> cubicles = new();
        public Transform cubicle_content;
        public Slider amount_slider;
        public Slider e_level_slider;
        public TextMeshProUGUI job_name;


        public Job data;

        public void Init(Job job)
        {
            job_name.text = Localization_Utility.get_localization(job._name);
            if(job.max_value == 0)
            {
                amount_slider.gameObject.SetActive(false);
            }
            else
            {
                amount_slider.gameObject.SetActive(true);
                amount_slider.maxValue = job.max_value;
                amount_slider.value = job.current_value;
            }
            if(job is IEmergency e)
            {
                e_level_slider.gameObject.SetActive(true);
            }
            else
            {
                e_level_slider.gameObject.SetActive(false);
            }

            data = job;

            foreach(var c in job.cubicles)
            {
                var cv = Instantiate(cubicle_prefab, cubicle_content, false);
                cv.Init(c);
                cv.gameObject.SetActive(true);

                cubicles.Add(cv);
            }
        }

        public void Tick()
        {
            if (amount_slider.gameObject.activeInHierarchy)
            {
                amount_slider.maxValue = data.max_value;
                amount_slider.value = data.current_value;
            }

            if (e_level_slider.gameObject.activeInHierarchy)
            {
                e_level_slider.maxValue = (data as IEmergency).e_max_value;
                e_level_slider.value = (data as IEmergency).e_current_value;
            }
        }
    }
}
