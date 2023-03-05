using System.Collections;
using TMPro;
using Tools;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TextMeshProUGUI timeText;

        private int minutes;
        private int seconds;
        private Coroutine timerCoroutine;

        private void OnValidate()
        {
            if (rectTransform == null || rectTransform.gameObject != this.gameObject)
            {
                rectTransform = GetComponent<RectTransform>();
            }
        }

        public void StartTimer()
        {
            timeText.color = UIManager.ActiveNumberTextColor;
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
            minutes = 0;
            seconds = 0;
            UpdateTimeText();
        }

        private IEnumerator TimerRoutine()
        {
            float timeElapsed = 0;
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