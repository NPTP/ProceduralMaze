using MazeGeneration;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Controller for UI screens and top-level UI behaviours.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GenerationScreen generationScreen;
        [SerializeField] private MazeScreen mazeScreen;
        [SerializeField] private EndScreen endScreen;
        [Space]
        [SerializeField] private FadeOverlay fadeOverlay;

        private Screen activeScreen;

        private void Awake()
        {
            MazeGenerator.OnMazeGenerationCompleted += HandleMazeGenerationCompleted;

            if (mazeScreen != null)
            {
                mazeScreen.OnRestartMaze += HandleRestartMaze;
            }
        }

        private void OnDestroy()
        {
            MazeGenerator.OnMazeGenerationCompleted -= HandleMazeGenerationCompleted;
            
            if (mazeScreen != null)
            {
                mazeScreen.OnRestartMaze -= HandleRestartMaze;
            }
        }

        private void Start()
        {
            SetUpSceneStartUI();
        }

        private void SetUpSceneStartUI()
        {
            SetScreenActive(generationScreen, true, instant: true);
            SetScreenActive(mazeScreen, false, instant: true);
            SetScreenActive(endScreen, false, instant: true);

            // Start the scene by fading in gracefully from black
            fadeOverlay.Fade(Color.black, alpha: 0, duration: 1, fromAlpha: 1);
        }

        private void HandleMazeGenerationCompleted()
        {
            SetScreenActive(mazeScreen, true);
        }
        
        private void HandleRestartMaze(MazeScreen ms)
        {
            SetScreenActive(mazeScreen, false);
            fadeOverlay.OnFadeCompleted += handleFadeCompleted;
            fadeOverlay.Fade(Color.black, alpha: 1, duration: 0.5f);

            void handleFadeCompleted(FadeOverlay fo)
            {
                fadeOverlay.OnFadeCompleted -= handleFadeCompleted;
                MazeGenerator.TearDown();
                SetUpSceneStartUI();
            }
        }

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
            }
            else
            {
                screen.FadeOut(instant);
            }
        }
    }
}