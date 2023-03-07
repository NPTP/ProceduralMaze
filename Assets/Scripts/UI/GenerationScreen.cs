using System;
using Maze;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// The starting screen that allows the user to choose the parameters of maze generation.
    /// </summary>
    public class GenerationScreen : Screen
    {
        public static event Action OnGenerateButtonClicked;
        
        [SerializeField] private OptionsRow rowsOptionsRow;
        [SerializeField] private OptionsRow columnsOptionsRow;
        [SerializeField] private Button generateButton;

        private void OnEnable()
        {
            rowsOptionsRow.OnNumberChanged += HandleNumberRowsChanged;
            columnsOptionsRow.OnNumberChanged += HandleNumberColumnsChanged;
            generateButton.onClick.AddListener(HandleGenerateButtonClicked);
        }
        
        private void OnDisable()
        {
            rowsOptionsRow.OnNumberChanged -= HandleNumberRowsChanged;
            columnsOptionsRow.OnNumberChanged -= HandleNumberColumnsChanged;
            generateButton.onClick.RemoveListener(HandleGenerateButtonClicked);
        }

        private void Start()
        {
            rowsOptionsRow.SetNumber(MazeGenerator.NumRows);
            columnsOptionsRow.SetNumber(MazeGenerator.NumCols);
        }

        private void HandleNumberRowsChanged(int newValue)
        {
            MazeGenerator.SetNumRows(newValue);
        }

        private void HandleNumberColumnsChanged(int newValue)
        {
            MazeGenerator.SetNumCols(newValue);
        }

        private void HandleGenerateButtonClicked()
        {
            FadeOut();
            OnGenerateButtonClicked?.Invoke();
        }
    }
}