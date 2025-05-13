using UnityEngine;
using UnityEngine.UI;
using World.Helpers;
using TMPro;
using Commons;

namespace World.Progresss
{
    public class ProgressEventView : MonoBehaviour
    {
        private const float FIXED_TRANSFORM_Z = 10F;

        public ProgressEvent pe;
        public Slider progress_slider;
        public Image icon;
        public TextMeshProUGUI cd_TX;
        public Button interact_btn;

        public ProgressStationView station;

        private float btn_x_offset_coef;


        #region 触发按钮的逻辑相关
        private bool btn_interactable = false;



        #endregion

        public void Init(ProgressEvent pe)
        {
            var pos = pe.pos;
            this.pe = pe;
            transform.position = new Vector3(pos.x, pos.y, FIXED_TRANSFORM_Z);
            station.Init(pe.module);
            progress_slider.maxValue = pe.max_value;
            btn_x_offset_coef = 12.8f / Config.current.trigger_length - 1;
        }

        public void Destroy()
        {
            //清空module
            Character_Module_Helper.EmptyModule(station.module);
            Destroy(gameObject);
        }

        public void tick()
        {
            var pos = pe.pos;
            progress_slider.value = pe.current_value;
            transform.position = new Vector3(pos.x, pos.y, FIXED_TRANSFORM_Z);
            Vector3 delta = transform.position - (Vector3)WorldContext.instance.caravan_pos;
            interact_btn.image.rectTransform.anchoredPosition = new Vector2(delta.x * btn_x_offset_coef, delta.y * -0.1f);
            
            if(WorldContext.instance.caravan_velocity.x > 0.1)
            {
                interact_btn.interactable = false;
            }
            else
            {
                interact_btn.interactable = true;
            }

            cd_TX.text = pe.cd_TX;

            station.tick();
        }

        public void AddValue()
        {
            pe.current_value = pe.max_value;
        }
    }
}
