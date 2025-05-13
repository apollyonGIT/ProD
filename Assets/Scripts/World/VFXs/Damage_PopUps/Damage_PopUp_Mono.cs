using System.Collections.Generic;
using UnityEngine;

namespace World.VFXs.Damage_PopUps
{
    public class Damage_PopUp_Mono : MonoBehaviour
    {
        public List<SpriteRenderer> dmg_list = new List<SpriteRenderer>();
        public List<Sprite> norma_textures = new List<Sprite>();
        public List<Sprite> critical_textures = new List<Sprite>();

        internal int exist_delta;
        bool is_init;

        internal Dmg_Data dmg_data;

        internal IDamage_PopUp_Mover mover;

        Dictionary<string, object> m_diy_fields = new();

        //==================================================================================================

        public void init(params object[] prms)
        {
            dmg_data = (Dmg_Data)prms[0];

            var content = $"{dmg_data.dmg}";
            var pos = (Vector2)prms[1];

            SetDmgSprites(dmg_data.dmg,dmg_data.is_critical);

            transform.position = pos;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

            if (dmg_data.is_critical)
                mover = Damage_Critical_PopUp_Mover.instance;
            else
                mover = Damage_PopUp_Mover.instance;

            mover.init(this);

            is_init = true;
        }

        private void SetDmgSprites(int dmg,bool is_critical)
        {
            int i = 0;

            if (dmg != 0 && is_critical)
            {
                dmg_list[9].gameObject.SetActive(true);
            }
            else
            {
                dmg_list[9].gameObject.SetActive(false);
            }


                for (i = 0; i < 9; i++)
                {
                    int temp;

                    if (dmg < Mathf.Pow(10, i))
                    {
                        dmg_list[i].gameObject.SetActive(false);
                        continue;
                    }

                    temp = (int)(dmg / Mathf.Pow(10, i));
                    temp %= 10;

                    dmg_list[i].sprite = is_critical ? critical_textures[temp] : norma_textures[temp];
                    dmg_list[i].gameObject.SetActive(true);
                }

        }

        private void Update()
        {
            if (!is_init) return;

            if (exist_delta <= 0)
            {
                DestroyImmediate(gameObject);
                return;
            }

            exist_delta--;

            mover.move(this);
        }


        public void destroy()
        {
            DestroyImmediate(gameObject);
        }


        public bool try_query_field<T>(string field_name, out T value)
        {
            value = default;

            if (!m_diy_fields.TryGetValue(field_name, out var obj))
                return false;

            if (obj is not T) return false;

            value = (T)obj;
            return true;
        }


        public void upd_field<T>(string field_name, T value)
        {
            if (!m_diy_fields.TryGetValue(field_name, out var obj))
            {
                m_diy_fields.Add(field_name, value);
                return;
            }

            if (obj is not T)
            {
                Debug.LogError($"伤害跳字Mono，试图更新的字段类型错误，字段名:{field_name}");
                return;
            }

            m_diy_fields[field_name] = value;
        }


        public void del_field(string field_name)
        {
            if (!m_diy_fields.TryGetValue(field_name, out var obj)) return;

            m_diy_fields.Remove(field_name);
        }
    }
}

