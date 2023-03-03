using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private FadeOverlay fadeOverlay;
    
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                fadeOverlay.Fade(Color.black, 1, 1);
            }
        
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                fadeOverlay.Fade(Color.black, 0, 1);
            }
        
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                fadeOverlay.Fade(Color.green, .5f, .25f);
            }
        
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                fadeOverlay.Fade(Color.magenta, .75f, 4);
            }
        }
    }
}