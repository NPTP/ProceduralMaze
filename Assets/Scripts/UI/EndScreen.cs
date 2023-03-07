using TMPro;
using Tools;
using UnityEngine;

namespace UI
{
    public class EndScreen : Screen
    {
        [SerializeField] private TextMeshProUGUI newRecordText;
        [SerializeField] private TimeText timeText;
        [SerializeField] private CanvasGroup restartButtonCanvasGroup;

        private Tween restartButtonFadeTween;
        
        private void OnEnable()
        {
            newRecordText.gameObject.SetActive(false);
        }

        protected override void OnStartFadeIn()
        {
            base.OnStartFadeIn();
            restartButtonCanvasGroup.alpha = 0;

            OnScreenFadeInCompleted += handleScreenFadeInCompleted;

            void handleScreenFadeInCompleted()
            {
                OnScreenFadeInCompleted -= handleScreenFadeInCompleted;
                restartButtonFadeTween = new Tween(() => restartButtonCanvasGroup.alpha,
                    a => restartButtonCanvasGroup.alpha = a);
                restartButtonFadeTween.Start(1, 0.5f);
            }
        }

        protected override void OnStartFadeOut()
        {
            base.OnStartFadeOut();
            if (restartButtonFadeTween != null)
            {
                restartButtonFadeTween.Stop();
                restartButtonFadeTween = null;
            }

            newRecordText.gameObject.SetActive(false);
        }
        
        public void SetFinishTimeText(int secondsElapsed)
        {
            timeText.SetTimeText(secondsElapsed);
        }

        public void ShowNewRecord()
        {
            newRecordText.gameObject.SetActive(true);
        }
    }
}