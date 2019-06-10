using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Quiz.Mono {
    [Serializable]
    public struct UIManagerParameters {
        [Header("Answers Options")]
        [SerializeField]
        private float margins;

        public float Margins => margins;

        [Header("Resolution Screen Options")]
        [SerializeField]
        private Color correctBGColor;

        public Color CorrectBGColor => correctBGColor;

        [SerializeField] private Color incorrectBGColor;

        public Color IncorrectBGColor => incorrectBGColor;

        [SerializeField] private Color finalBGColor;

        public Color FinalBGColor => finalBGColor;
    }

    [Serializable]
    public struct UIElements {
        [SerializeField] private RectTransform answersContentArea;

        public RectTransform AnswersContentArea => answersContentArea;

        [SerializeField] private TextMeshProUGUI questionInfoTextObject;

        public TextMeshProUGUI QuestionInfoTextObject => questionInfoTextObject;

        [SerializeField] private TextMeshProUGUI scoreText;

        public TextMeshProUGUI ScoreText => scoreText;

        [Space] [SerializeField] private Animator resolutionScreenAnimator;

        public Animator ResolutionScreenAnimator => resolutionScreenAnimator;

        [SerializeField] private Image resolutionBG;

        public Image ResolutionBG => resolutionBG;

        [SerializeField] private TextMeshProUGUI resolutionStateInfoText;

        public TextMeshProUGUI ResolutionStateInfoText => resolutionStateInfoText;

        [SerializeField] private TextMeshProUGUI resolutionScoreText;

        public TextMeshProUGUI ResolutionScoreText => resolutionScoreText;

        [Space] [SerializeField] private TextMeshProUGUI highScoreText;

        public TextMeshProUGUI HighScoreText => highScoreText;

        [SerializeField] private CanvasGroup mainCanvasGroup;

        public CanvasGroup MainCanvasGroup => mainCanvasGroup;

        [SerializeField] private RectTransform finishUIElements;

        public RectTransform FinishUIElements => finishUIElements;
    }

    public class UIManager : MonoBehaviour {
        /// <summary>
        ///     Function that is used to update new question UI information.
        /// </summary>
        private void UpdateQuestionUI(Question question) {
            UIElements.QuestionInfoTextObject.text = question.Info;
            CreateAnswers(question);
        }

        /// <summary>
        ///     Function that is used to display resolution screen.
        /// </summary>
        private void DisplayResolution(ResolutionScreenType type, int score) {
            UpdateResUI(type, score);
            UIElements.ResolutionScreenAnimator.SetInteger(resStateParaHash, 2);
            UIElements.MainCanvasGroup.blocksRaycasts = false;

            if (type != ResolutionScreenType.Finish) {
                if (IE_DisplayTimedResolution != null) {
                    StopCoroutine(IE_DisplayTimedResolution);
                }

                IE_DisplayTimedResolution = DisplayTimedResolution();
                StartCoroutine(IE_DisplayTimedResolution);
            }
        }

        private IEnumerator DisplayTimedResolution() {
            yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
            UIElements.ResolutionScreenAnimator.SetInteger(resStateParaHash, 1);
            UIElements.MainCanvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        ///     Function that is used to display resolution UI information.
        /// </summary>
        private void UpdateResUI(ResolutionScreenType type, int score) {
            var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

            switch (type) {
                case ResolutionScreenType.Correct:
                    UIElements.ResolutionBG.color = parameters.CorrectBGColor;
                    UIElements.ResolutionStateInfoText.text = "CORRECT!";
                    UIElements.ResolutionScoreText.text = "+" + score;
                    break;
                case ResolutionScreenType.Incorrect:
                    UIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    UIElements.ResolutionStateInfoText.text = "WRONG!";
                    UIElements.ResolutionScoreText.text = events.CurrentFinalScore <= 0 ? string.Empty : "-10";
                    break;
                case ResolutionScreenType.Finish:
                    UIElements.ResolutionBG.color = parameters.FinalBGColor;
                    UIElements.ResolutionStateInfoText.text = "FINAL SCORE: " + events.CurrentFinalScore;
                    UIElements.ResolutionScoreText.text = string.Empty;
                    UIElements.FinishUIElements.gameObject.SetActive(true);
                    break;
            }
        }

        /// <summary>
        ///     Function that is used to calculate and display the score.
        /// </summary>
        private IEnumerator CalculateScore() {
            var scoreValue = 0;
            while (scoreValue < events.CurrentFinalScore) {
                scoreValue++;
                UIElements.ResolutionScoreText.text = scoreValue.ToString();

                yield return null;
            }
        }

        /// <summary>
        ///     Function that is used to create new question answers.
        /// </summary>
        private void CreateAnswers(Question question) {
            EraseAnswers();

            float offset = 0 - parameters.Margins;
            for (int i = 0; i < question.Answers.Length; i++) {
                AnswerData newAnswer = Instantiate(answerPrefab, UIElements.AnswersContentArea);
                newAnswer.UpdateData(question.Answers[i].Info, i);

                newAnswer.Rect.anchoredPosition = new Vector2(0, offset);

                offset -= newAnswer.Rect.sizeDelta.y + parameters.Margins;
                UIElements.AnswersContentArea.sizeDelta =
                    new Vector2(UIElements.AnswersContentArea.sizeDelta.x, offset * -1);

                currentAnswers.Add(newAnswer);
            }
        }

        /// <summary>
        ///     Function that is used to erase current created answers.
        /// </summary>
        private void EraseAnswers() {
            foreach (var answer in currentAnswers) {
                Destroy(answer.gameObject);
            }

            currentAnswers.Clear();
        }

        /// <summary>
        ///     Function that is used to update score text UI.
        /// </summary>
        private void UpdateScoreUI() {
            if (events.CurrentFinalScore <= 0) {
                events.CurrentFinalScore = 0;
            }

            UIElements.ScoreText.text = "score: " + events.CurrentFinalScore;
        }

        #region Variables

        public enum ResolutionScreenType {
            Correct,
            Incorrect,
            Finish
        }

        [Header("References")]
        [SerializeField]
        private GameEvents events = null;

        [Header("UI Elements (Prefabs)")]
        [SerializeField]
        private AnswerData answerPrefab = null;

        [SerializeField] private UIElements UIElements = new UIElements();

        [Space] [SerializeField] private UIManagerParameters parameters = new UIManagerParameters();

        private List<AnswerData> currentAnswers = new List<AnswerData>();

        private int resStateParaHash;

        private IEnumerator IE_DisplayTimedResolution;

        #endregion

        #region Default Unity methods

        /// <summary>
        ///     Function that is called when the object becomes enabled and active
        /// </summary>
        private void OnEnable() {
            events.UpdateQuestionUI += UpdateQuestionUI;
            events.DisplayResolutionScreen += DisplayResolution;
            events.ScoreUpdated += UpdateScoreUI;
        }

        /// <summary>
        ///     Function that is called when the behaviour becomes disabled
        /// </summary>
        private void OnDisable() {
            events.UpdateQuestionUI -= UpdateQuestionUI;
            events.DisplayResolutionScreen -= DisplayResolution;
            events.ScoreUpdated -= UpdateScoreUI;
        }

        /// <summary>
        ///     Function that is called when the script instance is being loaded.
        /// </summary>
        private void Start() {
            UpdateScoreUI();
            resStateParaHash = Animator.StringToHash("ScreenState");
        }

        #endregion
    }
}
