using Maze;
using Tools;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// The screen displayed when the maze is generated, active and being played.
    /// </summary>
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
            RestartButton.OnAnyRestartButtonClicked += HandleAnyRestartButtonClicked;
            MazeGenerator.OnFocusedOnEndBlock += HandleFocusedOnEndBlock;
        }
        
        private void OnDisable()
        {
            RestartButton.OnAnyRestartButtonClicked -= HandleAnyRestartButtonClicked;
            MazeGenerator.OnFocusedOnEndBlock -= HandleFocusedOnEndBlock;
            timerUI.ResetTimer();
            controlsTutorial.Hide();
        }

        private void HandleFocusedOnEndBlock(MazeGenerator mazeGenerator)
        {
            PlayExitSplash();
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

        private void HandleAnyRestartButtonClicked()
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

        private void PlayExitSplash()
        {
            exitSplashAnimator.CrossFade(Splash, 0);
        }
    }
}