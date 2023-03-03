using Tools;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Screen : MonoBehaviour
    {
        private const float SCREEN_FADE_TIME = 1f;
        
        [SerializeField] private CanvasGroup canvasGroup;

        private Tween fadeTween = new Tween();

        private void OnValidate()
        {
            if (canvasGroup == null || canvasGroup.gameObject != this.gameObject)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        protected void FadeOut()
        {
            canvasGroup.blocksRaycasts = false;
            fadeTween.Stop();
            fadeTween.Start(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 0, SCREEN_FADE_TIME);
        }
        
        protected void FadeIn()
        {
            canvasGroup.blocksRaycasts = true;
            fadeTween.Stop();
            fadeTween.Start(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 1, SCREEN_FADE_TIME);
        }
    }
}