using Commons;
using Foundations;

namespace World.Progresss
{
    public class ProgressPD : Producer
    {
        public override IMgr imgr => mgr;
        public ProgressUiView puv;
        public ProgressView pv;

        ProgressMgr mgr;

        //==================================================================================================

        public override void init(int priority)
        {
            mgr = new(Config.MapMgr_Name, priority);
            mgr.Init();
            mgr.progress.add_view(pv);
            mgr.progress.add_view(puv);
            mgr.InitProgress(WorldContext.instance.r_scene.scene_plot);
        }


        public override void call()
        {
        }
    }
}