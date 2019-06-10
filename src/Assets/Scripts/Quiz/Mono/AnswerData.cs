using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Quiz.Mono {
    public class AnswerData : MonoBehaviour {
        /// <summary>
        ///     Function that is called to update the answer data.
        /// </summary>
        public void UpdateData(string info, int index) {
            infoTextObject.text = info;
            _answerIndex = index;
        }

        /// <summary>
        ///     Function that is called to reset values back to default.
        /// </summary>
        public void Reset() {
            Checked = false;
            UpdateUI();
        }

        /// <summary>
        ///     Function that is called to switch the state.
        /// </summary>
        public void SwitchState() {
            Checked = !Checked;
            UpdateUI();

            events.UpdateQuestionAnswer?.Invoke(this);
        }

        /// <summary>
        ///     Function that is called to update UI.
        /// </summary>
        private void UpdateUI() {
            if (toggle == null) return;

            toggle.sprite = Checked ? checkedToggle : uncheckedToggle;
        }

        #region Variables

        [Header("UI Elements")]
        [SerializeField]
        private TextMeshProUGUI infoTextObject = null;

        [SerializeField] private Image toggle = null;

        [Header("Textures")] [SerializeField] private Sprite uncheckedToggle = null;

        [SerializeField] private Sprite checkedToggle = null;

        [Header("References")]
        [SerializeField]
        private GameEvents events = null;

        private RectTransform _rect;

        public RectTransform Rect {
            get {
                if (_rect == null) {
                    _rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
                }

                return _rect;
            }
        }

        private int _answerIndex = -1;

        public int AnswerIndex => _answerIndex;

        private bool Checked;

        #endregion
    }
}
