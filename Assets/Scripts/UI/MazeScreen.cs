using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MazeScreen : Screen
    {
        public event Action<MazeScreen> OnRestartMaze; 

        [SerializeField] private Button restartButton;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(HandleRestartButtonClicked);
        }
        
        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(HandleRestartButtonClicked);
        }

        private void HandleRestartButtonClicked()
        {
            OnRestartMaze?.Invoke(this);
        }
    }
}