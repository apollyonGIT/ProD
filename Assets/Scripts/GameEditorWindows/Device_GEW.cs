using Commons;
using Foundations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using World;
using World.Devices;
using World.Helpers;

namespace GameEditorWindows
{
    public class Device_GEW :EditorWindow
    {
        string device_id;
        string slot_pos;

        [SerializeField]
        public  Device_GEW_CombinationData data;


        [MenuItem("GameEditorWindow/Device")]

        public static void ShowWindow()
        {
            GetWindow(typeof(Device_GEW));
        }

        bool upgrade_device;
        List<bool> upgrade_bool = new() {false,false,false,false,false,false,false};

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("请先运行游戏");
                return;
            }

            var ctx = WorldContext.instance;
            if (ctx == null) return;

            GUILayout.Label("device_id");
            {
                device_id = GUILayout.TextField(device_id);
            }

            GUILayout.Label("slot_name");
            {
                slot_pos = GUILayout.TextField(slot_pos);
            }
            if (GUILayout.Button("InstallDevice"))
            {
                Device_Slot_Helper.InstallDevice(device_id.ToString(),slot_pos);
            }

            if (GUILayout.Button("AddCombination"))
            {
                data.devices_id.Add(device_id);
                data.slots.Add(slot_pos);
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssetIfDirty(data);
            }

            for (int i = 0;i < data.devices_id.Count;i++)
            {
                var pair = (data.devices_id[i], data.slots[i]);    
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{pair.Item1}      {pair.Item2}");
                if (GUILayout.Button("Install"))
                {
                    Device_Slot_Helper.InstallDevice(pair.Item1,pair.Item2);
                }
                if (GUILayout.Button("Remove"))
                {
                    data.devices_id.RemoveAt(i);
                    data.slots.RemoveAt(i);

                    EditorUtility.SetDirty(data);
                    AssetDatabase.SaveAssetIfDirty(data);

                    Repaint();
                }

                GUILayout.EndHorizontal();
            }

            upgrade_device = EditorGUILayout.Foldout(upgrade_device, "UpgradeDevice");
            if (upgrade_device)
            {
                Mission.instance.try_get_mgr(Commons.Config.DeviceMgr_Name, out DeviceMgr dmgr);
                int index = 0;
                index = 0;
                foreach(var (slot,device) in dmgr.slots_device)
                {
                    GUILayout.Label($"{Localization_Utility.get_localization(device.desc.name)}");

                    upgrade_bool[index] = EditorGUILayout.Foldout(upgrade_bool[index], "Upgrades");
                    if (upgrade_bool[index])
                    {
                        foreach (var upgrade in device.upgrades)
                        {
                            if (GUILayout.Button($"{upgrade.desc.script} status: {upgrade.Applied()}"))
                            {
                                if (upgrade.Applied())
                                {
                                    upgrade.RemoveUpgrade(device);
                                }
                                else
                                {
                                    upgrade.ApplyUpgrade(device);
                                }
                            }
                        }
                    }

                    index ++;
                }
                
            }
        }
    }
}
