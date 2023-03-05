using System;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MazeScreen : Screen
    {
        private static readonly int Splash = Animator.StringToHash("Splash");
        private static readonly int Invisible = Animator.StringToHash("Invisible");
        
        public event Action<MazeScreen> OnRestartMaze;

        [SerializeField] private CanvasGroup restartAndTimerGroup;
        [SerializeField] private Button restartButton;
        [SerializeField] private TimerUI timerUI;
        [SerializeField] private ControlsTutorial controlsTutorial;
        [SerializeField] private Animator exitSplashAnimator;
        
        private void OnEnable()
        {
            restartButton.onClick.AddListener(HandleRestartButtonClicked);
        }
        
        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(HandleRestartButtonClicked);
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
            OnRestartMaze?.Invoke(this);
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

        public void PlayExitSplash()
        {
            exitSplashAnimator.CrossFade(Splash, 0);
        }
    }
}