using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MazeScreen : Screen
    {
        public event Action<MazeScreen> OnRestartMaze; 

        [SerializeField] private Button restartButton;
        [SerializeField] private TimerUI timerUI;
        [SerializeField] private ControlsTutorial controlsTutorial;
        
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

        private void HandleRestartButtonClicked()
        {
            controlsTutorial.Hide();
            OnRestartMaze?.Invoke(this);
        }

        public void BeginMazeTimer()
        {
            timerUI.StartTimer();
        }

        public void PlayControlsTutorial()
        {
            controlsTutorial.Play();
        }
    }
}