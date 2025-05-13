using Foundations;
using System.Collections.Generic;
using UnityEngine;
using World.Characters;

namespace World.SafeArea
{
    public class FoodPanel : MonoBehaviour
    {
        public Transform content;
        public RoleFoodPanel prefab;

        public List<RoleFoodPanel> foodPanels = new List<RoleFoodPanel>();

        public void Init()
        {
            foreach(var rfp in foodPanels)
            {
                Destroy(rfp.gameObject);
            }
            foodPanels.Clear();


            Mission.instance.try_get_mgr(Commons.Config.CharacterMgr_Name, out CharacterMgr cmgr);
            foreach(var character in cmgr.characters)
            {
                var rfp = Instantiate(prefab, content,false);
                rfp.gameObject.SetActive(true);
                rfp.Init(character,this);
                foodPanels.Add(rfp);
            }
        }


        public void UpdatePanel()
        {
            foreach (var f in foodPanels)
            {
                f.UpdatePanel();
            }
        }   
    }
}
