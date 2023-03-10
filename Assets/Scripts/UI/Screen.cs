using System;
using Tools;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Abstract base class for a UI screen. Handles basic transition logic.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Screen : MonoBehaviour
    {
        public event Action OnScreenFadeInCompleted;
        public event Action OnScreenFadeOutCompleted;
        
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

            OnStartFadeOut();
            
            if (instant)
            {
                canvasGroup.alpha = 0;
                return;
            }
            
            fadeTween.Stop();
            fadeTween.Start(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 0, SCREEN_FADE_TIME,
                () => OnScreenFadeOutCompleted?.Invoke());
        }
        
        public void FadeIn(bool instant = false)
        {
            canvasGroup.blocksRaycasts = true;

            OnStartFadeIn();
            
            if (instant)
            {
                canvasGroup.alpha = 1;
                return;
            }
            
            fadeTween.Stop();
            fadeTween.Start(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 1, SCREEN_FADE_TIME,
                () => OnScreenFadeInCompleted?.Invoke());
        }
        
        /// <summary>
        /// Performed at beginning of fade out. Override for custom behaviour at this time.
        /// </summary>
        protected virtual void OnStartFadeOut() { }

        /// <summary>
        /// Performed at beginning of fade in. Override for custom behaviour at this time.
        /// </summary>
        protected virtual void OnStartFadeIn() { }
    }
}