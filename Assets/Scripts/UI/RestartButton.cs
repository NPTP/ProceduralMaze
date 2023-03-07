using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Button that sends a special restart signal to reset the scene.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class RestartButton : MonoBehaviour
    {
        public static event Action OnAnyRestartButtonClicked;
        
        [SerializeField] private Button restartButton;

        private void OnValidate()
        {
            if (restartButton == null || restartButton.gameObject != this.gameObject)
            {
                restartButton = GetComponent<Button>();
            }
        }

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
            OnAnyRestartButtonClicked?.Invoke();
        }
    }
}