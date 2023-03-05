using Tools;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Screen : MonoBehaviour
    {
        private const float SCREEN_FADE_TIME = 0.5f;
        
        [SerializeField] private CanvasGroup canvasGroup;

        private Tween fadeTween = new Tween();

        private void OnValidate()
        {
            if (canvasGroup == null || canvasGroup.gameObject != this.gameObject)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void FadeOut(bool instant = false)
        {
            canvasGroup.blocksRaycasts = false;

            if (instant)
            {
                canvasGroup.alpha = 0;
                return;
            }
            
            fadeTween.Stop();
            fadeTween.Start(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 0, SCREEN_FADE_TIME);
        }
        
        public void FadeIn(bool instant = false)
        {
            canvasGroup.blocksRaycasts = true;
            
            if (instant)
            {
                canvasGroup.alpha = 1;
                return;
            }
            
            fadeTween.Stop();
            fadeTween.Start(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 1, SCREEN_FADE_TIME);
        }
    }
}