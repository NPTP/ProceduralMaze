using System;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// UI-covering colored fade overlay for transitions.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class FadeOverlay : MonoBehaviour
    {
        public event Action OnFadeCompleted;
    
        [SerializeField] private Image fadeImage;

        private readonly Tween fadeTween = new Tween();

        private void OnValidate()
        {
            if (fadeImage == null || fadeImage.gameObject != this.gameObject)
            {
                fadeImage = GetComponent<Image>();
            }

            fadeImage.raycastTarget = false;
            SetAlpha(0);
        }

        private void Awake()
        {
            // Start by gracefully uncovering the screen
            fadeImage.color = Color.black;
            SetAlpha(1);
        }

        private void Start()
        {
            Fade(Color.black, 0, 1);
        }

        /// <summary>
        /// Fade the overlay with the given color to the given alpha in the given duration.
        /// Interrupts any previous fade.
        /// Optionally specify scaled or unscaled time (default unscaled).
        /// </summary>
        public void Fade(Color color, float alpha, float duration, bool raycastTargetOnComplete = false)
        {
            fadeTween.Stop();

            float initialAlpha = GetAlpha();
            fadeImage.color = color;
            SetAlpha(initialAlpha);
        
            fadeImage.raycastTarget = true;

            fadeTween.Start(GetAlpha, SetAlpha, alpha, duration,
                callbackArgument: () =>
                {
                    fadeImage.raycastTarget = raycastTargetOnComplete;
                    OnFadeCompleted?.Invoke();
                });
        }

        private float GetAlpha()
        {
            return fadeImage.color.a;
        }

        private void SetAlpha(float alpha)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }
}