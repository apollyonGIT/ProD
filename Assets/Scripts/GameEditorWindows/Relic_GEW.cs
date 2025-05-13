using Commons;
using Foundations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using World.Relic;

namespace GameEditorWindows
{
    public class Relic_GEW : EditorWindow
    {
        string relic_id;

        [MenuItem("GameEditorWindow/Relic")]

        public static void ShowWindow()
        {
            GetWindow(typeof(Relic_GEW));
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("请先运行游戏");
                return;
            }

            GUILayout.Label("relic_id");
            {
                relic_id = GUILayout.TextField(relic_id);
            }

            if (GUILayout.Button("AddRelic"))
            {
                Mission.instance.try_get_mgr(Config.RelicMgr_Name, out RelicMgr rmgr);
                rmgr.AddRelic(relic_id);
            }

            if (GUILayout.Button("RelicStore"))
            {
                List<string> relics_id = new() {"1","2","3","4","6","7","8" };
                Mission.instance.try_get_mgr(Config.RelicMgr_Name, out RelicMgr rmgr);
                rmgr.InstantiateRelicStore(relics_id);
            }
        }
    }
}
