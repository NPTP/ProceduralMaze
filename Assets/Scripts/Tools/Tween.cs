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
        public enum Curve
        {
            Linear = 0,
            Quadratic,
            EaseOutBack
        }

        private Coroutine tweenCoroutine;
        private Func<float> getter;
        private Action<float> setter;

        public Tween()
        {
        }

        public Tween(Func<float> getter, Action<float> setter, Curve curve = Curve.Quadratic)
        {
            AssignProperties(getter, setter);
        }

        private void AssignProperties(Func<float> getterArgument, Action<float> setterArgument)
        {
            this.getter = getterArgument;
            this.setter = setterArgument;
        }

        public void Start(Func<float> getterArgument, Action<float> setterArgument, float endValueArgument,
            float durationArgument, Action callbackArgument = null, Curve curveArgument = Curve.Quadratic)
        {
            AssignProperties(getterArgument, setterArgument);
            Start(endValueArgument, durationArgument, callbackArgument, curveArgument);
        }

        public void Start(float endValue, float duration, Action callback = null, Curve curveArgument = Curve.Quadratic)
        {
            if (getter == null || setter == null)
            {
                Debug.LogError("Null getter or setter given, cannot execute Tween");
                return;
            }

            tweenCoroutine = CoroutineHost.StartHostedCoroutine(
                TweenRoutine(getter, setter, endValue, duration, callback,curveArgument));
        }

        public void Stop()
        {
            if (tweenCoroutine != null)
            {
                CoroutineHost.StopHostedCoroutine(ref tweenCoroutine);
            }
        }

        private static IEnumerator TweenRoutine(Func<float> getter, Action<float> setter, float endValue,
            float duration, Action callback, Curve curve)
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
                float percent = elapsed / duration;
                setter(initialValue + (difference * GetOutputFromCurve(curve, percent)));

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            setter(initialValue + difference);

            callback?.Invoke();
        }

        private static float GetOutputFromCurve(Curve curve, float input)
        {
            switch (curve)
            {
                case Curve.Linear:
                    return input;
                case Curve.Quadratic:
                    return input * input;
                case Curve.EaseOutBack:
                    float constant1 = 1.70158f;
                    float constant2 = constant1 + 1;
                    return 1 + constant2 * Mathf.Pow(input - 1, 3) + constant1 * Mathf.Pow(input - 1, 2);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}