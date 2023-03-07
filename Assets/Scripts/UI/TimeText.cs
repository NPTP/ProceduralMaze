using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimeText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void OnValidate()
        {
            if (text == null || text.gameObject != this.gameObject)
            {
                text = GetComponent<TextMeshProUGUI>();
            }
        }

        public void SetTimeText(int secondsElapsed)
        {
            text.text = $"{secondsElapsed / 60:00}:{secondsElapsed % 60:00}";
        }

        public void SetColor(Color color)
        {
            text.color = color;
        }
    }
}