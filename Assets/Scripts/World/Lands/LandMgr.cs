using System.Collections.Generic;
using Common;
using UnityEngine;

namespace World.Lands
{
    public class LandMgr : Singleton<LandMgr>
    {
        public Dictionary<int, Land> cells = new();

        //=============================================================================================

        public LandMgr init()
        {
            cells = new();
            return this;
        }


        public void add_cell(Land cell)
        {
            cells.Add(cell.id, cell);
        }


        public bool try_get_land_pos(int id, out Vector2 pos)
        {
            pos = default;
            if (!cells.TryGetValue(id, out var cell)) return false;

            pos = cell.pos;
            return true;
        }
    }
}

