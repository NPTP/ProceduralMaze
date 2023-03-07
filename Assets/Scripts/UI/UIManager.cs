using System;
using Input;
using Maze;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Controller for UI screens and top-level UI behaviours.
    /// Simple system for showing screens keeps only one screen visible at a time.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private const string PLAYERPREFS_BEST_TIME_KEY = "BestTime";
        
        public static event Action OnMazeRestart;
        
        public static readonly Color InactiveNumberTextColor = new Color(1, 1, 1, 1);
        public static readonly Color ActiveNumberTextColor = new Color(1, 1, 0.5f, 1);

        [SerializeField] private GenerationScreen generationScreen;
        [SerializeField] private MazeScreen mazeScreen;
        [SerializeField] private EndScreen endScreen;
        [Space]
        [SerializeField] private FadeOverlay fadeOverlay;

        private Screen activeScreen;

        private void Awake()
        {
            MazeGenerator.OnMazeGenerationStarted += HandleMazeGenerationStarted;
            MazeGenerator.OnMazeGenerationCompleted += HandleMazeGenerationCompleted;
            MazeGenerator.OnPlayerEnteredEndBlock += HandlePlayerEnteredEndBlock;
            PlayerControls.OnPlayerControlsEnabled += HandlePlayerControlsEnabled;
            PlayerControls.OnPlayerControlsDisabled += HandlePlayerControlsDisabled;
            RestartButton.OnAnyRestartButtonClicked += HandleAnyRestartButtonClicked;
        }

        private void OnDestroy()
        {
            MazeGenerator.OnMazeGenerationStarted -= HandleMazeGenerationStarted;
            MazeGenerator.OnMazeGenerationCompleted -= HandleMazeGenerationCompleted;
            MazeGenerator.OnPlayerEnteredEndBlock -= HandlePlayerEnteredEndBlock;
            PlayerControls.OnPlayerControlsEnabled -= HandlePlayerControlsEnabled;
            PlayerControls.OnPlayerControlsDisabled -= HandlePlayerControlsDisabled;
            RestartButton.OnAnyRestartButtonClicked -= HandleAnyRestartButtonClicked;
        }

        private void HandlePlayerControlsEnabled(PlayerControls playerControls)
        {
            // Subscribe only once the controls are available.
            playerControls.OnInputActionPerformed += HandleInputActionPerformed;
        }
        
        private void HandlePlayerControlsDisabled(PlayerControls playerControls)
        {
            // Unsubscribe when the controls are disabled
            playerControls.OnInputActionPerformed -= HandleInputActionPerformed;
        }

        private void HandleInputActionPerformed(PlayerControls playerControls)
        {
            // Unsubscribe immediately, as we only need to catch the first player input
            // to start the timer.
            playerControls.OnInputActionPerformed -= HandleInputActionPerformed;
            mazeScreen.BeginMazeTimer();
        }

        private void Start()
        {
            SetUpSceneStartUI();
        }

        private void SetUpSceneStartUI()
        {
            HideAllScreens();
            SetScreenActive(generationScreen, true, instant: true);

            // Start the scene by fading in gracefully from black
            fadeOverlay.Fade(Color.black, alpha: 0, duration: 1, fromAlpha: 1);
        }

        private void HandleMazeGenerationStarted(MazeGenerator mazeGenerator)
        {
            SetScreenActive(mazeScreen, true);
        }

        private void HandleMazeGenerationCompleted(MazeGenerator mazeGenerator)
        {
            // Don't show maze screen if player wins maze immediately.
            if (MazeGenerator.NumRows < 2 && MazeGenerator.NumCols < 2)
            {
                return;
            }
            
            SetScreenActive(mazeScreen, true);
            mazeScreen.ShowRestartAndTimerGroup();
        }
        
        private void HandlePlayerEnteredEndBlock(MazeGenerator mazeGenerator)
        {
            mazeScreen.StopMazeTimer();

            int time = mazeScreen.TimerSecondsElapsed;
            endScreen.SetFinishTimeText(time);
            
            int bestTime = PlayerPrefs.GetInt(PLAYERPREFS_BEST_TIME_KEY, int.MaxValue);
            if (time < bestTime)
            {
                endScreen.ShowNewRecord();
                PlayerPrefs.SetInt(PLAYERPREFS_BEST_TIME_KEY, time);
            }
            
            SetScreenActive(endScreen, true);
        }

        private void HandleAnyRestartButtonClicked()
        {
            RestartMaze();
        }

        private void RestartMaze()
        {
            fadeOverlay.OnFadeCompleted += handleFadeCompleted;
            fadeOverlay.Fade(Color.black, alpha: 1, duration: 0.5f);

            void handleFadeCompleted(FadeOverlay fo)
            {
                fadeOverlay.OnFadeCompleted -= handleFadeCompleted;
                OnMazeRestart?.Invoke();
                SetUpSceneStartUI();
            }
        }

        private void HideAllScreens()
        {
            SetScreenActive(generationScreen, false, instant: true);
            SetScreenActive(mazeScreen, false, instant: true);
            SetScreenActive(endScreen, false, instant: true);
        }
        
        /// <summary>
        /// Set the given screen active/inactive. Keeps track of the currently active screen
        /// and prevents two screens from being shown at once, fading between them.
        /// </summary>
        /// <param name="screen">The screen to set active</param>
        /// <param name="active">If true, set the screen active. If false, set inactive</param>
        /// <param name="instant">If true, perform the transition instantly</param>
        private void SetScreenActive(Screen screen, bool active, bool instant = false)
        {
            if (screen == null)
            {
                return;
            }
            
            if (active)
            {
                if (screen == activeScreen)
                {
                    return;
                }

                if (activeScreen != null)
                {
                    activeScreen.FadeOut(instant);
                }

                screen.FadeIn(instant);
                activeScreen = screen;
            }
            else
            {
                screen.FadeOut(instant);

                if (screen == activeScreen)
                {
                    activeScreen = null;
                }
            }
        }
    }
}