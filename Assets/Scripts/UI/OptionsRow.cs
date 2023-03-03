using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OptionsRow : MonoBehaviour
    {
        /// <summary>
        /// Called when the number actually changes. Argument is the new value.
        /// </summary>
        public event Action<int> OnNumberChanged;
        
        [SerializeField] private TextMeshProUGUI number;
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;

        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 99;

        private int numberInternal = 1;

        private void OnEnable()
        {
            upButton.onClick.AddListener(HandleUpButtonClicked);
            downButton.onClick.AddListener(HandleDownButtonClicked);
        }
        
        private void OnDisable()
        {
            upButton.onClick.RemoveListener(HandleUpButtonClicked);
            downButton.onClick.RemoveListener(HandleDownButtonClicked);
        }
        
        private void HandleUpButtonClicked()
        {
            int prevNumber = numberInternal;
            numberInternal = Mathf.Min(maxValue, numberInternal + 1);
            ChangeNumberText(prevNumber);
        }
        
        private void HandleDownButtonClicked()
        {
            int prevNumber = numberInternal;
            numberInternal = Mathf.Max(minValue, numberInternal - 1);
            ChangeNumberText(prevNumber);
        }

        private void ChangeNumberText(int prevNumber)
        {
            number.text = numberInternal.ToString();
            if (numberInternal != prevNumber)
            {
                OnNumberChanged?.Invoke(numberInternal);
            }
        }

        public void SetNumber(int newValue)
        {
            int prevNumber = numberInternal;
            numberInternal = Mathf.Clamp(newValue, minValue, maxValue);
            ChangeNumberText(prevNumber);
        }
    }
}