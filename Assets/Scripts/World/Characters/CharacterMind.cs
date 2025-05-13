using UnityEngine;
using UnityEngine.UI;

namespace World.Characters
{
    [RequireComponent(typeof(Image))]
    public class CharacterMind : MonoBehaviour
    {
        public Sprite happy, sad, normal;
        public void UpdateMind(float percent)
        {
            switch (percent)
            {
                case float n when (n >= 0.7f):
                    GetComponent<Image>().sprite = happy;
                    break;
                case float n when (n >= 0.3f):
                    GetComponent<Image>().sprite = normal;
                    break;
                case float n when (n >= 0.0f):
                    GetComponent<Image>().sprite = sad;
                    break;
            }
        }
    }
}
