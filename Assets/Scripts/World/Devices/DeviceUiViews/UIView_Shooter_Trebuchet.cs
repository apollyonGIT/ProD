using AutoCodes;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace World.Devices.DeviceUiViews
{
    public class UIView_Shooter_Trebuchet : DeviceUiView
    {
        private const int UI_AMMO_MAX = 6;
        private const short BLINK_TICK_MAX = 15;

        public TextMeshProUGUI ammoText;
        public Image ammo_progress;
        public Image manual_reload_indicator;
        public Button btn_reload;
        public Button btn_shoot;
        public Slider dir_controller;

        public Image ammo_slot;
        public List<Image> ammo;

        new private Shooter_Trebuchet owner;

        private short blink_tick;
        private bool blink_on;
        private Image btn_bg;
        private bool ammo_ui_exact_num;

        public override void attach(Device owner)
        {
            base.attach(owner);
            this.owner = base.owner as Shooter_Trebuchet;
        }
        public override void init()
        {
            base.init();
            btn_bg = btn_reload.GetComponent<Image>();

            fire_logics.TryGetValue(owner.desc.fire_logic.ToString(), out var record);
            var max_ammo = (int)record.capacity;
            ammo_ui_exact_num = max_ammo <= UI_AMMO_MAX;
            if (ammo_ui_exact_num)
                ammo_slot.rectTransform.sizeDelta = new UnityEngine.Vector2(max_ammo * 18 + 6, ammo_slot.rectTransform.sizeDelta.y);

            dir_controller.value = owner.Get_Dir_X();
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
                btn_reload.gameObject.SetActive(owner.Current_Ammo < owner.Max_Ammo);
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

            if ((owner.Current_Ammo > 0) ^ btn_shoot.gameObject.activeSelf)
                btn_shoot.gameObject.SetActive(!btn_shoot.gameObject.activeSelf);

            dir_controller.value = dir_controller.value >= 0 ? 1 : -1;
        }

        public void UI_Trigger_Reload()
        {
            owner.UI_Controlled_Reloading();
        }

        public void UI_Trigger_Shoot()
        {
            owner.UI_Controlled_Shooting();
        }

        public void UI_Trigger_Turn_Dir()
        {
            owner.UI_Controlled_Turn_Dir(dir_controller.value);
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
