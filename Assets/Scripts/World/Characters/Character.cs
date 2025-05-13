using AutoCodes;
using Commons;
using System;
using System.Collections.Generic;
using World.Characters.CharacterProperties;
using World.Devices;
using World.Helpers;

namespace World.Characters
{
    public enum EatingState
    {
        Full,
        Hungry,
        ExtremelyHungry,
        RawFood,
        CookedFood,
        Feast,
    }

    public class Character
    {
        public role desc;
        public List<CharacterProperty> character_properties = new List<CharacterProperty>();

        public float current_mood;
        public bool is_working;

        //可以吃多少次
        public int eat_times;

        //每次吃多少份
        public int per_eat;
        public EatingState es => m_es;
        private EatingState m_es;

        //检查是否需要踢出工作的间隔
        public int check_interval = CHECK_INTERVAL;
        private const int CHECK_INTERVAL = 600;

        protected float default_mood = 50f;
        protected float default_ability = 100f;

        public bool IsStrike => strike_tick > 0;
        protected int strike_tick;

        public int tick_time;
        public int work_tick;

        #region delegate

        public List<Func<Device,float, float>> device_properties_func = new();
        public List<Func<Device, bool>> device_can_work_func = new ();

        public Action start_work;
        public Action end_work;

        public Action enter_safe_area;
        public Action leave_safe_area;

        public Action<uint> after_eat;

        #endregion
        public void tick()
        {
            tick_time++;
            if (is_working)
            {
                work_tick++;
                if (current_mood < 20)                     //开始怠工检测
                {
                    if (check_interval > 0)
                        check_interval--;
                    else
                    {
                        check_interval = CHECK_INTERVAL;
                        if (UnityEngine.Random.Range(0, 20) > current_mood)
                        {
                            strike_tick = UnityEngine.Random.Range(1200, 3600);
                            Character_Module_Helper.FreeCharacter(this);
                        }
                    }
                }
            }

            if (strike_tick > 0)
            {
                strike_tick--;
            }
        }

        public virtual float GetAbility(Device d)
        {
            float current_ability;
            current_ability = MoodEffectsAbility(default_ability);

            if (device_properties_func != null)
            {
                foreach (var device_properties_func in device_properties_func)
                {
                    current_ability = device_properties_func.Invoke(d, current_ability);
                }
            }

            return current_ability;
        }

        public virtual bool CanWork(Device d)
        {
            if (device_can_work_func != null)
            {
                foreach (var device_can_work_func in device_can_work_func)
                {
                    if (!device_can_work_func.Invoke(d))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private float MoodEffectsAbility(float ability)
        {
            if (current_mood > 80)
            {
                return ability + 80;
            }
            else if (current_mood > 60)
            {
                return ability + 40;
            }
            else if (current_mood > 40)
            {
                return ability;
            }
            else if (current_mood > 20)
            {
                return ability - 30;
            }
            else
            {
                return ability - 50;
            }
        }

        public virtual void UpdateMood(float delta)
        {
            current_mood = UnityEngine.Mathf.Clamp(current_mood + delta, 0, 100);
        }

        public virtual void SetStrikeTick(int tick)
        {
            strike_tick = tick;
        }

        public void SetEatingState(EatingState eating_state)
        {
            UpdateMood(-GetEatingStateDelta(m_es));
            m_es = eating_state;
            UpdateMood(GetEatingStateDelta(m_es));
        }

        /// <summary>
        /// 获取进食状态对心情的修正
        /// </summary>
        /// <param name="es"></param>
        /// <returns></returns>
        private float GetEatingStateDelta(EatingState es)
        {
            switch (es)
            {
                case EatingState.Full:
                    return 0;
                case EatingState.Hungry:
                    return -25f;
                case EatingState.ExtremelyHungry:
                    return -50;
                case EatingState.RawFood:
                    return -10f;
                case EatingState.CookedFood:
                    return 15f;
                case EatingState.Feast:
                    return 40f;
                default:
                    return 0;
            }
        }

        public virtual void EnterSafeArea()
        {

            if (m_es == EatingState.Hungry)
            {
                SetEatingState(EatingState.ExtremelyHungry);
            }
            else if (m_es != EatingState.ExtremelyHungry)
            {
                SetEatingState(EatingState.Hungry);
            }
            enter_safe_area?.Invoke();
        }

        public virtual void LeaveSafeArea()
        {
            leave_safe_area?.Invoke();

            tick_time = 0;
            work_tick = 0;
            eat_times = 1;  //恢复可吃数
        }

        public virtual void StartWork()
        {
            is_working = true;
            start_work?.Invoke();
        }

        public virtual void EndWork()
        {
            is_working = false;
            end_work?.Invoke();
        }

        public void Init(role rc)
        {
            desc = rc;
            m_es = EatingState.Full;
            current_mood = default_mood;
            per_eat = 1;
            eat_times = 1;

            if (desc.properties != null)
            {
                foreach (var p in desc.properties)
                {
                    var cp = CharacterUtility.GetProperty(this, p);
                    cp.Init();
                    character_properties.Add(cp);
                }
            }

            List<uint> rnd_p = new List<uint>(desc.properties_rnd);
            var rnd_count = UnityEngine.Random.Range(desc.properties_num.Item1, desc.properties_num.Item2 + 1);

            for (int i = 0; i < rnd_count ; i++)
            {
                var id = UnityEngine.Random.Range(0, rnd_p.Count);

                var cp = CharacterUtility.GetProperty(this, rnd_p[id]);
                rnd_p.RemoveAt(id);

                cp.Init();
                character_properties.Add(cp);
            }
        }

        public void Start()
        {
            foreach (var p in character_properties)
            {
                p.Start();
            }
        }

        //只管角色的吃就完事了，不考虑消耗什么的
        public void Eat(uint food_id)
        {
            eat_times--;

            if (food_id == Config.current.food_id_1)
            {
                SetEatingState(EatingState.RawFood);
            }
            else if (food_id == Config.current.food_id_2)
            {
                SetEatingState(EatingState.Full);
            }
            else if (food_id == Config.current.food_id_3)
            {
                SetEatingState(EatingState.CookedFood);
            }
            else if (food_id == Config.current.food_id_4)
            {
                SetEatingState(EatingState.Feast);
            }
            else
            {
                UnityEngine.Debug.Log($"吃什么怪东西了 {food_id}");
            }

            after_eat?.Invoke(food_id);
        }
    }
}
