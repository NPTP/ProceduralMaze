using System;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// UI-covering colored fade overlay for transitions.
    /// The screen is blocked by the overlay during a fade to prevent
    /// early or late button presses.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class FadeOverlay : MonoBehaviour
    {
        public event Action<FadeOverlay> OnFadeCompleted;
    
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

        /// <summary>
        /// Fade the overlay with the given color to the given alpha in the given duration.
        /// Interrupts any previous fade.
        /// </summary>
        /// <param name="color">The color of the fade</param>
        /// <param name="alpha">The alpha to fade to</param>
        /// <param name="duration">The duration of the fade</param>
        /// <param name="fromAlpha">Optional starting alpha argument. If left blank, use the current alpha</param>
        public void Fade(Color color, float alpha, float duration, float fromAlpha = -1)
        {
            fadeTween.Stop();

            float initialAlpha = fromAlpha is < 0 or > 1 ? GetAlpha() : fromAlpha;
            fadeImage.color = color;
            SetAlpha(initialAlpha);
        
            fadeImage.raycastTarget = true;

            fadeTween.Start(GetAlpha, SetAlpha, alpha, duration,
                callbackArgument: () =>
                {
                    fadeImage.raycastTarget = false;
                    OnFadeCompleted?.Invoke(this);
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