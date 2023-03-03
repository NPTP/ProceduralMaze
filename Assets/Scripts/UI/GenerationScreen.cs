using MazeGeneration;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GenerationScreen : Screen
    {
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
            rowsOptionsRow.SetNumber(PlayerPrefs.GetInt(MazeGenerator.NUM_ROWS_PLAYERPREFS_KEY, 1));
            columnsOptionsRow.SetNumber(PlayerPrefs.GetInt(MazeGenerator.NUM_COLS_PLAYERPREFS_KEY, 1));
        }

        private void HandleNumberRowsChanged(int newValue)
        {
            PlayerPrefs.SetInt(MazeGenerator.NUM_ROWS_PLAYERPREFS_KEY, newValue);
        }

        private void HandleNumberColumnsChanged(int newValue)
        {
            PlayerPrefs.SetInt(MazeGenerator.NUM_COLS_PLAYERPREFS_KEY, newValue);
        }

        private void HandleGenerateButtonClicked()
        {
            FadeOut();
            MazeGenerator.GenerateMaze();
        }
    }
}