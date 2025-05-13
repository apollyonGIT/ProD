using Commons;
using Foundations;
using UnityEditor;
using UnityEngine;
using World.Work;

namespace GameEditorWindows
{
    public class Work_GEW : EditorWindow
    {

        
        public static void ShowWindow()
        {
            GetWindow(typeof(Work_GEW));
        }


        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("请先运行游戏");
                return;
            }

            if (!Mission.instance.try_get_mgr(Config.WorkMgr_Name, out WorkMgr wmgr)) return;

            foreach(var w in wmgr.works)
            {
                foreach(var job in w.jobs)
                {
                    GUILayout.Label($"工作名称:{job._name}");
                    GUILayout.Label($"工位状态:");
                    for (int i = 0; i < job.cubicles.Count; i++)
                    {
                        GUILayout.Label($"工位{i+1}号    {(job.cubicles[i].has_worker?"有人":"无人")}");
                    }
                    GUILayout.Space(10);
                }

                if (GUILayout.Button("燃起来"))
                {
                    w.AddJob(WorkUtility.Id2Job(14101));
                }
            }
        }
    }
}
