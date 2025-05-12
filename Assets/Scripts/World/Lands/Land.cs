using UnityEngine;

namespace World.Lands
{
    public class Land
    {
        public int id;
        public Vector2 pos;

        //=============================================================================================

        public Land(int id)
        {
            this.id = id;
            pos = new(id * -1.5f, 0);
        }
    }
}

