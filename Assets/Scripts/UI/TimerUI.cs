using System.Collections;
using Tools;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public class TimerUI : MonoBehaviour
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int ScalePunch = Animator.StringToHash("ScalePunch");

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TimeText timeText;
        [SerializeField] private Animator animator;

        private float timeElapsed;
        public int SecondsElapsed => (int)timeElapsed;
        private Coroutine timerCoroutine;

        private void OnValidate()
        {
            if (animator == null || animator.gameObject != this.gameObject)
            {
                animator = GetComponent<Animator>();
            }
        }

        public void StartTimer()
        {
            timeText.SetColor(UIManager.ActiveNumberTextColor);
            animator.CrossFade(ScalePunch, 0);
            CoroutineHost.InterruptAndStartCoroutine(TimerRoutine(), ref timerCoroutine);
        }

        public void StopTimer()
        {
            timeText.SetColor(UIManager.InactiveNumberTextColor);
            CoroutineHost.StopHostedCoroutine(ref timerCoroutine);
            timerCoroutine = null;
        }

        public void ResetTimer()
        {
            StopTimer();
            timeElapsed = 0;
            UpdateTimeText();
        }

        private IEnumerator TimerRoutine()
        {
            while (true)
            {
                int secondsPassed = (int)timeElapsed;
                int minutes = secondsPassed / 60;
                int seconds = secondsPassed % 60;
                UpdateTimeText();
                
                timeElapsed += Time.deltaTime;

                // Max out the timer at just under one hour.
                if (minutes == 59 && seconds == 59)
                {
                    StopTimer();
                    yield break;
                }

                yield return null;
            }
        }

        private void UpdateTimeText()
        {
            timeText.SetTimeText((int)timeElapsed);
        }
    }
}