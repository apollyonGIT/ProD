using System.Collections.Generic;
using UnityEngine;

namespace World.Lands
{
    public class LandPD : MonoBehaviour
    {
        public int half_length = 5;

        public GameObject land_model;

        internal LandMgr mgr;

        //=============================================================================================

        // Start is called before the first frame update
        void Start()
        {
            mgr = LandMgr.instance.init();

            foreach (var cell in cells())
            {
                mgr.add_cell(cell);

                var land_view = Instantiate(land_model, transform);
                land_view.transform.localPosition = cell.pos;
                land_view.name = $"floor {cell.id}";
                land_view.gameObject.SetActive(true);
            }
        }


        IEnumerable<Land> cells()
        {
            for (int i = -half_length; i < half_length + 1; i++)
            {
                Land cell = new(i);

                yield return cell;
            }
        }
        
    }
}

