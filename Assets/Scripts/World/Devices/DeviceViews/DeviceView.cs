using Commons;
using Foundations;
using Foundations.MVVM;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World.Devices.Device_AI;
using World.Devices.DeviceEmergencies;
using World.Helpers;
using World.VFXs;

namespace World.Devices.DeviceViews
{
    public class DeviceView : MonoBehaviour, IDeviceView
    {
        public Device owner;

        public SkeletonAnimation anim;

        public List<DeviceKeyPoint> dkp = new();

        public List<DeviceCollider> colliders = new();

        MaterialPropertyBlock mpb;
        //==================================================================================================

        void IModelView<Device>.detach(Device owner)
        {
            this.owner = null;
            Destroy(gameObject);
        }

        public virtual void init()
        {

        }

        public virtual void init_pos()
        {
            Mission.instance.try_get_mgr(Config.DeviceMgr_Name, out DeviceMgr dmgr);
            foreach (var (slot, device) in dmgr.slots_device)
            {
                if (device == owner)
                {
                    if (anim != null)
                    {
                        anim.GetComponent<MeshRenderer>().sortingOrder = BattleUtility.slot_2_order_in_layer(slot);
                    }
                }
            }
        }

        void IDeviceView.notify_change_anim(string anim_name, bool loop)
        {
            if (anim != null)
                anim.state.SetAnimation(0, anim_name, loop);
        }


        /// <summary>
        /// 当当前动画播放完后，重置动画触发事件，准备过渡到下一个动画
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void AnimComplete(Spine.TrackEntry entry)
        {
            var current = anim.state.GetCurrent(0);

            if (entry.Animation.Name != current.Animation.Name)
                return;

            if (current.IsComplete)
            {
                foreach (var anim_event in owner.anim_events)
                {
                    if (anim_event.anim_name == current.Animation.Name && anim_event.triggered == false)
                    {
                        anim_event.triggered = true;
                        anim_event.anim_event?.Invoke(owner);
                    }

                    if (anim_event.anim_name == current.Animation.Name)
                    {
                        anim_event.triggered = false;
                    }
                }
            }
        }

        public virtual void notify_on_tick()
        {
            transform.localPosition = owner.position;
            transform.localRotation = EX_Utility.look_rotation_from_left(WorldContext.instance.caravan_dir);

            if (owner.device_type == Device.DeviceType.melee)
                transform.localScale = Vector3.one * BattleContext.instance.melee_scale_factor;

            if (anim != null)
            {
                anim.Update(Config.PHYSICS_TICK_DELTA_TIME);

                foreach (var (bone_name, dir) in owner.bones_direction)
                {
                    var bone = anim.skeleton.FindBone(bone_name);
                    if (bone != null)
                    {
                        if (Vector2.SignedAngle(Vector2.right, dir) > 90)
                        {
                            anim.skeleton.FlipX = true;
                            bone.Rotation = Vector2.SignedAngle(dir, Vector2.left);
                        }
                        else if (Vector2.SignedAngle(Vector2.right, dir) < -90)
                        {
                            anim.skeleton.FlipX = true;
                            bone.Rotation = Vector2.SignedAngle(dir, Vector2.left);
                        }
                        else
                        {
                            anim.skeleton.FlipX = false;
                            bone.Rotation = Vector2.SignedAngle(Vector2.right, dir);
                        }
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
        }

        protected void anim_event_trigger(AnimEvent ae)
        {
            if (ae.triggered)
                return;
            var current = anim.state.GetCurrent(0);
            var duration_frame = Mathf.CeilToInt(current.Animation.Duration * Config.PHYSICS_TICKS_PER_SECOND);   //动画帧数总长
            var current_frame = Mathf.CeilToInt(current.AnimationTime * Config.PHYSICS_TICKS_PER_SECOND);
            var trigger_frame = Mathf.CeilToInt(duration_frame * ae.percent);

            if (current_frame >= trigger_frame && ae.triggered == false && current_frame < duration_frame)
            {
                ae.triggered = true;
                ae.anim_event?.Invoke(owner);
            }

            //根据ManualUpdate  动画帧和逻辑帧应该同步
        }

        void IDeviceView.notify_open_collider(string _name, Action<ITarget> enter_e = null, Action<ITarget> stay_e = null, Action<ITarget> exit_e = null)
        {
            foreach (var collider in colliders)
            {
                if (collider.collider_name == _name)
                {
                    collider.gameObject.SetActive(true);
                    collider.enter_event += enter_e;
                    collider.stay_event += stay_e;
                    collider.exit_event += exit_e;
                }
            }
        }

        void IDeviceView.notify_close_collider(string _name)
        {
            foreach (var collider in colliders)
            {
                if (collider.collider_name == _name)
                {
                    collider.gameObject.SetActive(false);
                    collider.enter_event = null;
                    collider.stay_event = null;
                    collider.exit_event = null;
                }
            }
        }

        void IDeviceView.notify_change_anim_speed(float f)
        {
            anim.timeScale = f;
        }

        void IDeviceView.notify_hurt(int dmg)
        {
            var r = anim.GetComponent<MeshRenderer>();
            mpb = new MaterialPropertyBlock();
            mpb.SetFloat("_FillPhase", 1);
            r.SetPropertyBlock(mpb);
            StopCoroutine("IColorRevert");
            StartCoroutine("IColorRevert");
        }

        IEnumerator IColorRevert()
        {
            yield return new WaitForSeconds(0.1f);

            var p = 1f;
            var r = anim.GetComponent<MeshRenderer>();
            mpb = new MaterialPropertyBlock();
            while (p >= 0)
            {
                p -= 0.02f;
                mpb.SetFloat("_FillPhase", p);
                r.SetPropertyBlock(mpb);
                yield return null;
            }

        }

        public virtual void attach(Device owner)
        {
            this.owner = owner;
            transform.localPosition = owner.position;
            if (anim != null)
            {
                anim.state.Complete += AnimComplete;
            }

            foreach (var collider in colliders)
            {
                collider.device = owner;
            }
        }

        void IDeviceView.notify_set_station(DeviceModule dm)
        {

        }

        protected VFX smoke_vfx;

        void IDeviceView.notify_disable()
        {
            Vfx_Helper.InstantiateVfx(Config.current.devicedestroy_vfx, 240, (Vector2)transform.position);

            DeviceKeyPoint p = dkp.Find(x => x.key_name == "fire");

            smoke_vfx = Vfx_Helper.InstantiateVfx(Config.current.devicesmoke_vfx, 9999,p.transform,true);
        }

        void IDeviceView.notify_enable()
        {
            if (smoke_vfx != null)
                smoke_vfx.duration = 0;
        }

        public virtual void notify_player_oper(bool oper)
        {

        }

        public virtual void notify_attack_radius(bool show)
        {

        }

        protected VFX fire_vfx;
        void IDeviceView.notify_add_emergency(DeviceEmergency de)
        {
            DeviceKeyPoint p = dkp.Find(x => x.key_name == "fire");

            if (de is DeviceFired)
            {
                if(p != null)
                    fire_vfx = Vfx_Helper.InstantiateVfx("fire_vfx",9999,p.transform, true);
            }
            Debug.Log("发生紧急情况");
        }

        void IDeviceView.notify_remove_emergency(DeviceEmergency de)
        {
            if(de is DeviceFired)
            {
                if(fire_vfx!=null)
                    fire_vfx.duration = 0;
            }
            Debug.Log("解除紧急情况");
        }
    }
}

