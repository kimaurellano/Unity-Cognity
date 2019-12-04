using TMPro;
using UnityEngine;

namespace Assets.Scripts.Quizzes {
    public class AnswerScript : MonoBehaviour {
        public delegate void OnSelect(bool correct);

        public static event OnSelect OnSelectEvent;

        [SerializeField] private TextMeshProUGUI _answerText;

        public string AnswerText { get; set; }

        public bool IsCorrect { get; set; }

        private void Start() {
            _answerText.SetText(AnswerText);
        }

        public void SelectedAnswer() {
            OnSelectEvent?.Invoke(IsCorrect);
        }
    }
}
