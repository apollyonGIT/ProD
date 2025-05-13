using Addrs;
using Commons;
using Foundations.MVVM;
using Spine;
using UnityEngine;
using UnityEngine.UI;
using World.Helpers;
using World.Ui;
using World.Widgets;

namespace World.Caravans
{
    public interface IUiFix
    {
        void Fix();
    }

    public class CaravanUiView : MonoBehaviour, ICaravanView, IUiView
    {
        public Caravan owner;

        public Slider true_hp;
        public Slider hp;
        public Slider speed_slider;
        public Slider lever_slider;

        public CaravanFixView cfv;
        public Button push_button;

        public CaravanStationView driving;
        public CaravanStationView fix;

        public Image high_lever_indicator;

        private const float hp_change_speed = 5;
        private float current_view_hp;
        Skeleton ICaravanView.sk => throw new System.NotImplementedException();

        Vector2 IUiView.pos => transform.position;

        public void Brake()
        {
            owner.Brake();
        }

        public void Push()
        {
            Widget_PushCar_Context.instance.PushCaravan();
        }

        void IModelView<Caravan>.attach(Caravan owner)
        {
            this.owner = owner;

            true_hp.maxValue = WorldContext.instance.caravan_hp_max;
            true_hp.value = WorldContext.instance.caravan_hp;
            hp.maxValue = WorldContext.instance.caravan_hp_max;
            hp.value = WorldContext.instance.caravan_hp;

            driving.Init(Widget_DrivingLever_Context.instance.driving_module);
            fix.Init(Widget_Fix_Context.instance.fix_module);

            Ui_Pos_Helper.ui_views.Add(this);

            lever_slider.maxValue = Widget_DrivingLever_Context.MAX_LEVER;
        }

        void IModelView<Caravan>.detach(Caravan owner)
        {
            if (this.owner != null)
                this.owner = null;
            Destroy(gameObject);
        }

        void ICaravanView.notify_on_hurt()
        {

        }

        void ICaravanView.notify_on_tick()
        {

            var wctx = WorldContext.instance;
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

            var car_speed_kmh = wctx.caravan_velocity.magnitude * 7.2f;   // 7.2F = 3.6F * 2F, where 3.6F is the conversion factor from m/s to km/h, and 2F is the factor to convert from the speed in unit/s to the speed in m/s.

            lever_slider.value = WorldContext.instance.driving_lever;
            speed_slider.value = (Mathf.Log(car_speed_kmh + 4, 2) - 2f) * 0.2f;  //车速刻度取对数显示

            update_module();
        }

        private void update_module()
        {
            if (Character_Module_Helper.GetModule(driving.module) != null)
            {
                Addressable_Utility.try_load_asset(Character_Module_Helper.GetModule(driving.module).desc.portrait_small, out Sprite s);
                driving.character_image.sprite = s;
                driving.character_image.color = new Color(1, 1, 1, 1f);    //显示角色头像
            }
            else
            {
                driving.character_image.color = new Color(1, 1, 1, 0f);
                driving.character_image.sprite = null;
            }

            cfv.tick();

            if (Character_Module_Helper.GetModule(fix.module) != null)
            {
                Addressable_Utility.try_load_asset(Character_Module_Helper.GetModule(fix.module).desc.portrait_small, out Sprite s);
                fix.character_image.sprite = s;
                fix.character_image.color = new Color(1, 1, 1, 1f);    //显示角色头像
            }
            else
            {
                fix.character_image.sprite = null;
                fix.character_image.color = new Color(1, 1, 1, 0f);
            }


            if (Widget_DrivingLever_Context.instance.can_switch_into_high_lever())
                high_lever_indicator.color = Color.green;
            else if (Widget_DrivingLever_Context.instance.will_switch_into_high_lever())
                high_lever_indicator.color = Color.yellow;
            else
                high_lever_indicator.color = Color.red;

            if (Widget_PushCar_Context.instance.AbleToPush_CheckCD)
                if (Widget_PushCar_Context.instance.AbleToPush())
                    if (WorldContext.instance.driving_lever > 0.95f && WorldContext.instance.caravan_acc.x <= 0)    //拉杆到95%以上，加速度却不大于0，提醒推车
                        set_push_button_status(true, Color.green);    //CD好了，不仅可以推，更是高亮提醒要推，显示绿色
                    else
                        set_push_button_status(true, Color.white);    //CD好了，可以推，显示白色
                else
                    set_push_button_status(false, Color.red);    //CD好了，但不能推，显示红色
            else
                set_push_button_status(false, Color.black);    //CD没好，一定显示黑色
            #region 子函数set_push_button_status
            void set_push_button_status(bool interactable, Color color)
            {
                push_button.interactable = interactable;
                push_button.image.color = color;
            }
            #endregion
        }

        void ICaravanView.notify_on_tick1()
        {

        }
    }
}
