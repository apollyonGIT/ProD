namespace World.Work
{
    public class WorldWorkView :WorkView
    {
        public override void tick()
        {
            foreach (var jv in jobs)
            {
                jv.Tick();
            }
        }

        public override void init(Work work)
        {
            owner = work;
            hp.gameObject.SetActive(false);
            true_hp.gameObject.SetActive(false);    //血条不需要
            name_text.gameObject.SetActive(false);  //work名字不需要
            foreach (var job in owner.jobs)
            {
                var jv = Instantiate(job_prefab, job_content, false);
                jv.Init(job);
                jv.gameObject.SetActive(true);
                jobs.Add(jv);
            }
        }
    }
}
