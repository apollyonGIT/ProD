using Commons;
using Foundations.MVVM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World.Progresss
{
    public class ProgressUiView : MonoBehaviour, IProgressView
    {
        #region CONST
        private const float PROGRESS_LENGTH_COEF = 0.5F;
        private const float PROGRESS_LENGTH_MAX = 2000F;

        private const float NOTICE_POS_Y_END = -600F;
        private const float NOTICE_POS_Y_DELTA = 400f;

        private const float NOTICE_SIZE_END = 250F;
        private const float NOTICE_SIZE_DELTA = -75F;
        #endregion

        Progress owner;

        public Slider progress_slider;
        public TextMeshProUGUI progress_text;

        public Image notice;
        public TextMeshProUGUI notice_text;

        public GameObject plot_prefab_vital;
        public GameObject plot_prefab_trivial;

        //==================================================================================================

        void IModelView<Progress>.attach(Progress owner)
        {
            this.owner = owner;
        }


        void IModelView<Progress>.detach(Progress owner)
        {
            this.owner = null;
        }


        void IProgressView.notify_on_tick()
        {
            progress_slider.value = owner.current_progress;

            var distance_remaining = (owner.total_progress - owner.current_progress) * 2;  // unit to meters = 2.0;
            progress_text.text = $"{distance_remaining:F0} m";
        }

        void IProgressView.notify_notice_encounter(float p, bool b)
        {
            if (!b)
            {
                notice.gameObject.SetActive(false);
                progress_text.color = Color.white;
                return;
            }

            if (p - owner.current_progress < Config.current.notice_length_1 && p - owner.current_progress > Config.current.notice_length_2)
            {
                notice.gameObject.SetActive(true);
                var distance_remaining_noticed = p - owner.current_progress;
                notice_text.text = $"{distance_remaining_noticed * 2:F1} m";     // unit to meters = 2.0;
                var t = 1 - Mathf.Exp(-(distance_remaining_noticed - Config.current.notice_length_2) * 0.1f);
                var y = NOTICE_POS_Y_END + t * NOTICE_POS_Y_DELTA;
                notice.rectTransform.anchoredPosition = new Vector2(0, y);
                var size = NOTICE_SIZE_END + t * NOTICE_SIZE_DELTA;
                notice.rectTransform.sizeDelta = new Vector2(size, size);
                progress_text.color = Color.red;
            }
        }

        void IProgressView.notify_add_progress_event(ProgressEvent pe)
        {

        }

        void IProgressView.notify_remove_progress_event(ProgressEvent pe)
        {

        }

        void IProgressView.notify_init()
        {
            progress_slider.maxValue = owner.total_progress;
            progress_slider.GetComponent<RectTransform>().sizeDelta
                = new Vector2(Mathf.Min(owner.total_progress * PROGRESS_LENGTH_COEF, PROGRESS_LENGTH_MAX), progress_slider.GetComponent<RectTransform>().sizeDelta.y);

            foreach (var p in owner.single_plots)
            {
                var percent = p.trigger_progress / owner.total_progress;
                var plot = Instantiate(p.ui_visible ? plot_prefab_vital : plot_prefab_trivial, progress_slider.transform);
                plot.GetComponent<RectTransform>().anchoredPosition = new Vector2(percent * progress_slider.GetComponent<RectTransform>().sizeDelta.x - progress_slider.GetComponent<RectTransform>().sizeDelta.x / 2, 0);
                plot.gameObject.SetActive(true);
            }
        }
    }
}

