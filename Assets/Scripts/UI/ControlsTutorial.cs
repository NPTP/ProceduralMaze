using Audio;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// UI element that shows & animates the arrow controls onscreen.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ControlsTutorial : MonoBehaviour
    {
        private static readonly int Invisible = Animator.StringToHash("Invisible");
        private static readonly int PlayTutorial = Animator.StringToHash("PlayTutorial");
        private static readonly int Idle = Animator.StringToHash("Idle");
        
        [SerializeField] private Animator animator;

        public void Play()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            AudioPlayer.PlayOneShot(AudioPlayer.AudioContainer.UITick);
            animator.CrossFade(PlayTutorial, 0);
        }

        public void Hide()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            animator.CrossFade(Invisible, 0.25f);
        }
    }
}