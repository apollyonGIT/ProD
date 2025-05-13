using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using World.Helpers;

namespace World.Work
{
    public class CubicleView : MonoBehaviour,IPointerClickHandler
    {
        public Cubicle data;
        public Image cubicle_image;
        public void Init(Cubicle c)
        {
            data = c;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                Cubicle_Info_Helper.EmptyCubicle(data);
            }
            else
            {
                var c = Cubicle_Info_Helper.GetSelectCharacter();
                if (c != null)
                {
                    var past_c = Cubicle_Info_Helper.GetCubicle(data);
                    if (past_c != null)
                        Cubicle_Info_Helper.EmptyCubicle(data);
                    Cubicle_Info_Helper.SetCubicle(c, data);
                }
                else
                {
                    var character = Cubicle_Info_Helper.GetCubicle(data);
                    if (character != null)
                    {
                        Cubicle_Info_Helper.SetSelectCharacter(character);
                    }
                }
            }
        }
    }
}
