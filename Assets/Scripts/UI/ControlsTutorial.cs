using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class ControlsTutorial : MonoBehaviour
    {
        private static readonly int Invisible = Animator.StringToHash("Invisible");
        private static readonly int PlayTutorial = Animator.StringToHash("PlayTutorial");
        private static readonly int Idle = Animator.StringToHash("Idle");
        
        [SerializeField] private Animator animator;

        public void Play()
        {
            animator.CrossFade(PlayTutorial, 0);
        }

        public void Hide()
        {
            animator.CrossFade(Invisible, 0.25f);
        }
    }
}