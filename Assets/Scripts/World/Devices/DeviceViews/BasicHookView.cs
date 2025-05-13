using Commons;
using Foundations;
using Spine.Unity;
using UnityEngine;
using World.Devices.Device_AI;
using static World.Devices.Device_AI.NewBasicHook;

namespace World.Devices.DeviceViews
{
    public class BasicHookView : DeviceView
    {
        public SkeletonUtilityBone hook;
        public override void notify_on_tick()
        {
            transform.localPosition = owner.position;
            transform.localRotation = EX_Utility.look_rotation_from_left(WorldContext.instance.caravan_dir);
            if (anim != null)
            {
                anim.Update(Config.PHYSICS_TICK_DELTA_TIME);

                foreach (var (bone_name, dir) in owner.bones_direction)
                {
                    var bone = anim.skeleton.FindBone(bone_name);
                    if (bone != null)
                    {
                        bone.Rotation = Vector2.SignedAngle(Vector2.right, dir);
                    }
                }

                foreach (var anim_event in owner.anim_events)
                {
                    if (anim_event.anim_name == anim.AnimationName)
                    {
                        anim_event_trigger(anim_event);
                    }
                }
            }

            var hk = owner as NewBasicHook;

            switch (hk.fsm)
            {
                case Device_FSM_Hook.Shooting:
                case Device_FSM_Hook.Shooting_reeling_in:
                case Device_FSM_Hook.Hooked:
                case Device_FSM_Hook.Breaking_reeling_in:
                    hook.transform.position = new Vector3(hk.hook_position.x, hk.hook_position.y, 10);
                    break;
                default:
                    hook.transform.localPosition = Vector3.zero;
                    break;
            }         

        }
    }
}
