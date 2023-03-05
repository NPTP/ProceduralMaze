using System.Collections;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Animator animator;

        private float timeElapsed;
        private int minutes;
        private int seconds;
        private Coroutine timerCoroutine;

        private void OnValidate()
        {
            if (animator == null || animator.gameObject != this.gameObject)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.X))
            {
                StartTimer();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.C))
            {
                StopTimer();
            }
        }

        public void StartTimer()
        {
            timeText.color = UIManager.ActiveNumberTextColor;
            animator.CrossFade(ScalePunch, 0);
            CoroutineHost.InterruptAndStartCoroutine(TimerRoutine(), ref timerCoroutine);
        }

        public void StopTimer()
        {
            timeText.color = UIManager.InactiveNumberTextColor;
            CoroutineHost.StopHostedCoroutine(ref timerCoroutine);
            timerCoroutine = null;
        }

        public void ResetTimer()
        {
            StopTimer();
            timeElapsed = 0;
            minutes = 0;
            seconds = 0;
            UpdateTimeText();
        }

        private IEnumerator TimerRoutine()
        {
            while (true)
            {
                int secondsPassed = (int)timeElapsed;
                minutes = secondsPassed / 60;
                seconds = secondsPassed % 60;
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
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}