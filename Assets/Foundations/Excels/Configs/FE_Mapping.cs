using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Foundations.Excels
{
    public class FE_Mapping
    {
        public static Dictionary<string, Type> type_mapping = new()
        {
            { "uint", typeof(uint) },
            { "int", typeof(int) },
            { "float", typeof(float) },
            { "string", typeof(string) },
            { "bool", typeof(bool) },

            { "Vector2", typeof(Vector2) },
            { "Vector2?", typeof(Vector2?) },
            { "List<string>", typeof(List<string>) },
            { "uint[]", typeof(List<uint>) },
            { "int[]", typeof(List<int>) },
            { "float[]", typeof(List<float>) },
            { "(uint,float,float,float)[]", typeof(List<(uint,float,float,float)>) },
            { "(uint,int,int)[]", typeof(List<(uint,int,int)>) },
            { "float?", typeof(float?) },
            { "string[]", typeof(List<string>) },
            { "(float,float)", typeof((float,float)) },
            { "(float,float)?", typeof((float,float)?) },
            { "bool[]", typeof(List<bool>) },
            { "(int,int)", typeof((int,int)) },
            { "(string,string)", typeof((string,string)) },

            { "HaHa", typeof(HaHa)},
            { "spawn_type", typeof(Spawn_Type) },
            { "slotType", typeof(Slot_Type) },
            { "deviceType", typeof(Device_Type) },
            { "siteType", typeof(Site_Type) },
            { "shopType", typeof(Shop_Type) },

            { "dict<slotType,(string,string)>", typeof(Dictionary<Slot_Type, (string,string)>) },
            { "dict<string,int>", typeof(Dictionary<string, int>) },
            { "dict<uint,float>", typeof(Dictionary<uint, float>) },
            { "dict<uint,int>", typeof(Dictionary<uint, int>) },
        };


        public static object ChangeType(object obj, Type type)
        {
            object ret = null;

            foreach (var method in typeof(FE_Mapping).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Where(t => t.Name.Contains("converter") && t.ReturnType == type))
            {
                ret = method.Invoke(null, new object[] { obj });
                break;
            }

            if (obj == null && ret == null) return null;

            if (!type.IsValueType)
            {
                ret ??= Activator.CreateInstance(type, obj);
            }
            
            return ret;
        }


        static List<string> converter_list_string(object obj)
        {
            if (obj == null) return new();

            var str = (string)obj;
            List<string> ret = new();

            var strs = str.Split(';');
            foreach (var _str in strs)
            {
                ret.Add(_str);
            }

            return ret;
        }


        static List<uint> converter_list_uint(object obj)
        {
            if (obj == null) return new();

            var str = "";
            if (obj.GetType() == typeof(string))
            {
                str = (string)obj;
            }
            else
            {
                var _d = (double)obj;
                str = _d.ToString();
            }
            
            List<uint> ret = new();

            var strs = str.Split(';');
            foreach (var _str in strs)
            {
                ret.Add(uint.Parse(_str));
            }

            return ret;
        }


        static List<int> converter_list_int(object obj)
        {
            if (obj == null) return new();

            var str = "";
            if (obj.GetType() == typeof(string))
            {
                str = (string)obj;
            }
            else
            {
                var _d = (double)obj;
                str = _d.ToString();
            }

            List<int> ret = new();

            var strs = str.Split(';');
            foreach (var _str in strs)
            {
                ret.Add(int.Parse(_str));
            }

            return ret;
        }


        static List<float> converter_list_float(object obj)
        {
            if (obj == null) return new();

            var str = "";
            if (obj.GetType() == typeof(string))
            {
                str = (string)obj;
            }
            else
            {
                var _d = (double)obj;
                str = _d.ToString();
            }

            List<float> ret = new();

            var strs = str.Split(';');
            foreach (var _str in strs)
            {
                ret.Add(float.Parse(_str));
            }

            return ret;
        }


        static List<bool> converter_list_bool(object obj)
        {
            if (obj == null) return new();

            var str = (string)obj;
            List<bool> ret = new();

            var strs = str.Split(';');
            foreach (var _str in strs)
            {
                ret.Add(bool.Parse(_str));
            }

            return ret;
        }


        static List<(uint, float, float, float)> converter_list_tuple_uint_float_float_float(object obj)
        {
            if (obj == null) return new();

            var str = (string)obj;
            List<(uint, float, float, float)> ret = new();

            var strs = str.Split(';');
            foreach (var items in strs)
            {
                var item = items.Split('~');

                ret.Add((uint.Parse(item[0]), float.Parse(item[1]), float.Parse(item[2]), float.Parse(item[3])));
            }

            return ret;
        }


        static List<(uint, int, int)> converter_list_tuple_uint_int_int(object obj)
        {
            if (obj == null) return new();

            var str = (string)obj;
            List<(uint, int, int)> ret = new();

            var strs = str.Split(';');
            foreach (var items in strs)
            {
                var item = items.Split('~');

                ret.Add((uint.Parse(item[0]), int.Parse(item[1]), int.Parse(item[2])));
            }

            return ret;
        }


        static float? converter_float_nullable(object obj)
        {
            float? ret = new();
            if (obj == null) return null;

            var d = (double)obj;
            ret = (float)d;

            return ret;
        }


        static Vector2 converter_Vector2(object obj)
        {
            if (obj == null) return default;

            var str = (string)obj;
            Vector2 ret = new();

            var strs = str.Split(',');
            ret = new(float.Parse(strs[0]), float.Parse(strs[1]));

            return ret;
        }


        static Vector2? converter_Vector2_nullable(object obj)
        {
            if (obj == null) return null;

            return converter_Vector2(obj);
        }


        static (float, float) converter_tuple_float_float(object obj)
        {
            if (obj == null) return default;

            var str = (string)obj;
            (float, float) ret = new();

            var strs = str.Split('~');
            ret = (float.Parse(strs[0]), float.Parse(strs[1]));

            return ret;
        }


        static (float, float)? converter_tuple_float_float_nullable(object obj)
        {
            if (obj == null) return null;

            return converter_tuple_float_float(obj);
        }


        static (int, int) converter_tuple_int_int(object obj)
        {
            if (obj == null) return default;

            var str = (string)obj;
            (int, int) ret = new();

            var strs = str.Split('~');
            ret = (int.Parse(strs[0]), int.Parse(strs[1]));

            return ret;
        }


        static (string, string) converter_tuple_string_string(object obj)
        {
            if (obj == null) return default;

            var str = (string)obj;
            (string, string) ret = new();

            var strs = str.Split('~');
            ret = (strs[0], strs[1]);

            return ret;
        }


        static Dictionary<Slot_Type, (string, string)> converter_dic_slotType_tuple_string_string(object obj)
        {
            if (obj == null) return new();

            var str = (string)obj;
            Dictionary<Slot_Type, (string, string)> ret = new();

            var strs = str.Split(';');
            foreach (var items in strs)
            {
                var item = items.Split('=');
                var key = item[0];
                var value = item[1];

                ret.Add(new(key), converter_tuple_string_string(value));
            }

            return ret;
        }


        static Dictionary<string, int> converter_dic_string_int(object obj)
        {
            if (obj == null) return new();

            var str = (string)obj;
            Dictionary<string, int> ret = new();

            var strs = str.Split(';');
            foreach (var items in strs)
            {
                var item = items.Split('=');
                var key = item[0];
                var value = item[1];

                ret.Add(key, int.Parse(value));
            }

            return ret;
        }


        static Dictionary<uint, float> converter_dic_uint_float(object obj)
        {
            if (obj == null) return new();

            var str = (string)obj;
            Dictionary<uint, float> ret = new();

            var strs = str.Split(';');
            foreach (var items in strs)
            {
                var item = items.Split('=');
                var key = item[0];
                var value = item[1];

                ret.Add(uint.Parse(key), float.Parse(value));
            }

            return ret;
        }


        static Dictionary<uint, int> converter_dic_uint_int(object obj)
        {
            if (obj == null) return new();

            var str = (string)obj;
            Dictionary<uint, int> ret = new();

            var strs = str.Split(';');
            foreach (var items in strs)
            {
                var item = items.Split('=');
                var key = item[0];
                var value = item[1];

                ret.Add(uint.Parse(key), int.Parse(value));
            }

            return ret;
        }
    }
}

