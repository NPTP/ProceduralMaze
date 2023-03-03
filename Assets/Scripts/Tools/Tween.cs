using System;
using System.Collections;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Class for easy management of tweens between values (float tweens only for the sake of this exercise).
    /// Tweens move on a parabolic rather than linear curve.
    /// </summary>
    public class Tween
    {
        private Coroutine tweenCoroutine;
        private Func<float> getter;
        private Action<float> setter;

        public Tween() { }
        
        public Tween(Func<float> getter, Action<float> setter)
        {
            AssignProperties(getter, setter);
        }

        private void AssignProperties(Func<float> getterArgument, Action<float> setterArgument)
        {
            this.getter = getterArgument;
            this.setter = setterArgument;
        }
        
        public void Start(Func<float> getterArgument, Action<float> setterArgument, float endValueArgument, float durationArgument, Action callbackArgument = null)
        {
            AssignProperties(getterArgument, setterArgument);
            Start(endValueArgument, durationArgument, callbackArgument);
        }

        public void Start(float endValue, float duration, Action callback = null)
        {
            if (getter == null || setter == null)
            {
                Debug.LogError("Null getter or setter given, cannot execute Tween");
                return;
            }
            
            tweenCoroutine = CoroutineHost.StartHostedCoroutine(TweenRoutine(getter, setter, endValue, duration, callback));
        }

        public void Stop()
        {
            if (tweenCoroutine != null)
            {
                CoroutineHost.StopHostedCoroutine(ref tweenCoroutine);
            }
        }
        
        private static IEnumerator TweenRoutine(Func<float> getter, Action<float> setter, float endValue, float duration, Action callback)
        {
            float elapsed = 0f;
            float initialValue = getter();
            float difference = endValue - initialValue;

            if (difference == 0)
            {
                yield break;
            }
            
            while (elapsed < duration)
            {
                float percentParabolic = elapsed / duration;
                percentParabolic *= percentParabolic;
                setter(initialValue + (difference * percentParabolic));

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            setter(initialValue + difference);

            callback?.Invoke();
        }
    }
}