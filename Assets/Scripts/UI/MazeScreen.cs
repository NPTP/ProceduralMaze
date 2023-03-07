using Tools;
using UnityEngine;

namespace UI
{
    public class MazeScreen : Screen
    {
        private static readonly int Splash = Animator.StringToHash("Splash");
        private static readonly int Invisible = Animator.StringToHash("Invisible");
        
        public int TimerSecondsElapsed => timerUI.SecondsElapsed;

        [SerializeField] private CanvasGroup restartAndTimerGroup;
        [SerializeField] private TimerUI timerUI;
        [SerializeField] private ControlsTutorial controlsTutorial;
        [SerializeField] private Animator exitSplashAnimator;
        
        private void OnEnable()
        {
            RestartButton.OnRestartButtonClicked += HandleRestartButtonClicked;
        }
        
        private void OnDisable()
        {
            RestartButton.OnRestartButtonClicked -= HandleRestartButtonClicked;
            timerUI.ResetTimer();
            controlsTutorial.Hide();
        }

        protected override void OnStartFadeIn()
        {
            restartAndTimerGroup.alpha = 0;
            restartAndTimerGroup.blocksRaycasts = false;
            timerUI.ResetTimer();
        }
        
        protected override void OnStartFadeOut()
        {
            timerUI.StopTimer();
            exitSplashAnimator.CrossFade(Invisible, 0.25f);
            controlsTutorial.Hide();
        }

        private void HandleRestartButtonClicked()
        {
            controlsTutorial.Hide();
        }

        public void ShowRestartAndTimerGroup()
        {
            Tween groupTween = new Tween();
            groupTween.Start(() => restartAndTimerGroup.alpha, a => restartAndTimerGroup.alpha = a,
                1, 0.25f, ()=>
                {
                    restartAndTimerGroup.blocksRaycasts = true;
                    controlsTutorial.Play();
                });
        }

        public void BeginMazeTimer()
        {
            timerUI.StartTimer();
        }

        public void StopMazeTimer()
        {
            timerUI.StopTimer();
        }

        public void PlayExitSplash()
        {
            exitSplashAnimator.CrossFade(Splash, 0);
        }
    }
}