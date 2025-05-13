using AutoCodes;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using World.Devices.Device_AI;

namespace World.Devices.DeviceUiViews
{
    public class BasicShooterUiView : DeviceUiView
    {
        private const int UI_AMMO_MAX = 6;
        private const short BLINK_TICK_MAX = 15;

        public TextMeshProUGUI ammoText;
        public Image ammo_progress;
        public Image manual_reload_indicator;
        public Button btn_reload;

        public Image ammo_slot;
        public List<Image> ammo;

        new private NewBasicShooter owner;

        private short blink_tick;
        private bool blink_on;
        private Image btn_bg;
        private bool ammo_ui_exact_num;

        public override void attach(Device owner)
        {
            base.attach(owner);
            this.owner = base.owner as NewBasicShooter;

            btn_bg = btn_reload.GetComponent<Image>();

            fire_logics.TryGetValue(owner.desc.fire_logic.ToString(), out var record);
            var max_ammo = (int)record.capacity;
            ammo_ui_exact_num = max_ammo <= UI_AMMO_MAX;
            if (ammo_ui_exact_num)
                ammo_slot.rectTransform.sizeDelta = new UnityEngine.Vector2(ammo_slot.rectTransform.sizeDelta.x, max_ammo * 18 + 6);
        }

        public override void init()
        {
            base.init();
        }

        public override void notify_on_tick()
        {
            base.notify_on_tick();

            if (owner.reloading)
            {
                btn_reload.gameObject.SetActive(false);
                if (owner.reload_by_stage)
                    ammoText.text = ((float)owner.reload_stage_current / owner.reload_stage_max).ToString("P0");
                else
                    ammoText.text = $"{owner.Current_Ammo}/{owner.Max_Ammo}";
            }
            else
            {
                ammoText.text = $"{owner.Current_Ammo}/{owner.Max_Ammo}";
                btn_reload.gameObject.SetActive(true);
                if (owner.Current_Ammo == 0)
                    btn_blink();
                else
                    btn_bg.color = UnityEngine.Color.white;
            }

            ammo_progress.fillAmount = owner.Reloading_Process;
            manual_reload_indicator.gameObject.SetActive(owner.can_manual_reload);

            if (ammo_ui_exact_num)
            {
                for (int i = 0; i < owner.Max_Ammo; i++)
                    if (ammo[i].gameObject.activeSelf != (i < owner.Current_Ammo))
                        ammo[i].gameObject.SetActive(!ammo[i].gameObject.activeSelf);
            }
            else
            {
                var ammo_showed = UI_AMMO_MAX * owner.Current_Ammo / owner.Max_Ammo;
                for (int i = 0; i < UI_AMMO_MAX; i++)
                    if (ammo[i].gameObject.activeSelf != (i < ammo_showed))
                        ammo[i].gameObject.SetActive(!ammo[i].gameObject.activeSelf);
            }
        }

        public void reloading_start()
        {
            if (owner.Current_Ammo < owner.Max_Ammo)
                owner.UI_Controlled_Reloading();
        }

        private void btn_blink()
        {
            if (--blink_tick <= 0)
            {
                blink_tick = BLINK_TICK_MAX;
                blink_on = !blink_on;
            }

            btn_bg.color = blink_on ? UnityEngine.Color.red : new UnityEngine.Color(255f, 129f, 129f);
        }
    }
}
