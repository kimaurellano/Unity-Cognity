﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Player;
using Assets.Scripts.Quiz.Mono;
using Assets.Scripts.Quiz.ScriptableObject;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GrammarQuiz.Mono {
    public class GrammarGameManager : MonoBehaviour {
        private bool _isPaused;

        /// <summary>
        ///     Function that is called to update new selected answer.
        /// </summary>
        public void UpdateAnswers(AnswerData newAnswer) {
            if (Questions[currentQuestion].GetAnswerType == Question.AnswerType.Single) {
                foreach (var answer in PickedAnswers) {
                    if (answer != newAnswer) {
                        answer.Reset();
                    }
                }

                PickedAnswers.Clear();
                PickedAnswers.Add(newAnswer);
            } else {
                bool alreadyPicked = PickedAnswers.Exists(x => x == newAnswer);
                if (alreadyPicked) {
                    PickedAnswers.Remove(newAnswer);
                } else {
                    PickedAnswers.Add(newAnswer);
                }
            }
        }

        /// <summary>
        ///     Function that is called to clear PickedAnswers list.
        /// </summary>
        public void EraseAnswers() {
            PickedAnswers = new List<AnswerData>();
        }

        /// <summary>
        ///     Function that is called to display new question.
        /// </summary>
        private void Display() {
            EraseAnswers();
            var question = GetRandomQuestion();

            if (events.UpdateQuestionUI != null) {
                events.UpdateQuestionUI(question);
            } else {
                Debug.LogWarning(
                    "Ups! Something went wrong while trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null. Issue occured in GameManager.Display() method.");
            }
        }

        /// <summary>
        ///     Function that is called to accept picked answers and check/display the result.
        /// </summary>
        public void Accept() {
            bool isCorrect = CheckAnswers();
            FinishedQuestions.Add(currentQuestion);

            UpdateScore(isCorrect ? Questions[currentQuestion].AddScore : -Questions[currentQuestion].AddScore);

            if (IsFinished) {
                SetHighscore();
            }

            var type = IsFinished ? UIManager.ResolutionScreenType.Finish : isCorrect ? UIManager.ResolutionScreenType.Correct : UIManager.ResolutionScreenType.Incorrect;

            events.DisplayResolutionScreen?.Invoke(type, Questions[currentQuestion].AddScore);

            AudioManager.Instance.PlaySound(isCorrect ? "CorrectSFX" : "IncorrectSFX");

            if (type != UIManager.ResolutionScreenType.Finish) {
                if (IE_WaitTillNextRound != null) {
                    StopCoroutine(IE_WaitTillNextRound);
                }

                IE_WaitTillNextRound = WaitTillNextRound();
                StartCoroutine(IE_WaitTillNextRound);
            }
        }

        /// <summary>
        ///     Function that is called to check currently picked answers and return the result.
        /// </summary>
        private bool CheckAnswers() {
            return CompareAnswers();
        }

        /// <summary>
        ///     Function that is called to compare picked answers with question correct answers.
        /// </summary>
        private bool CompareAnswers() {
            if (PickedAnswers.Count > 0) {
                List<int> c = Questions[currentQuestion].GetCorrectAnswers();
                List<int> p = PickedAnswers.Select(x => x.AnswerIndex).ToList();

                List<int> f = c.Except(p).ToList();
                List<int> s = p.Except(c).ToList();

                return !f.Any() && !s.Any();
            }

            return false;
        }

        /// <summary>
        ///     Function that is called to load all questions from the Resource folder.
        /// </summary>
        private void LoadQuestions() {
            Object[] objs = Resources.LoadAll("GrammarQuizQuestions", typeof(Question));
            Questions = new Question[15];
            for (int i = 0; i < 15; i++) {
                Questions[i] = (Question) objs[i];
            }
        }

        /// <summary>
        ///     Function that is called restart the game.
        /// </summary>
        public void RestartGame() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        ///     Function that is called to quit the application.
        /// </summary>
        public void MainMenu() {
            Destroy(GameObject.Find("AudioManager").gameObject);

            SceneManager.LoadScene("BaseMenu");
        }

        public void PauseGame() {
            if (_isPaused) {
                Time.timeScale = 1;
                _isPaused = false;
            } else {
                Time.timeScale = 0;
                _isPaused = true;
            }
        }

        /// <summary>
        ///     Function that is called to set new highscore if game score is higher.
        /// </summary>
        private void SetHighscore() {
            var highscore = events.CurrentFinalScore;

            BaseScoreHandler baseScoreHandler = new BaseScoreHandler();
            baseScoreHandler.AddScore(highscore, Type.GameType.ProblemSolving);
        }

        /// <summary>
        ///     Function that is called update the score and update the UI.
        /// </summary>
        private void UpdateScore(int add) {
            events.CurrentFinalScore += add;

            events.ScoreUpdated?.Invoke();
        }

        #region Variables

        public Question[] Questions { get; private set; }

        [SerializeField] private GameEvents events = null;

        [SerializeField] private Animator timerAnimtor = null;

        [SerializeField] private TextMeshProUGUI timerText = null;

        [SerializeField] private Color timerHalfWayOutColor = Color.yellow;

        [SerializeField] private Color timerAlmostOutColor = Color.red;

        private Color timerDefaultColor = Color.white;

        private List<AnswerData> PickedAnswers = new List<AnswerData>();

        private readonly List<int> FinishedQuestions = new List<int>();

        private int currentQuestion;

        private int timerStateParaHash;

        private IEnumerator IE_WaitTillNextRound;

        private IEnumerator IE_StartTimer;

        private bool IsFinished => FinishedQuestions.Count >= Questions.Length;

        #endregion

        #region Default Unity methods

        /// <summary>
        ///     Function that is called when the object becomes enabled and active
        /// </summary>
        private void OnEnable() {
            events.UpdateQuestionAnswer += UpdateAnswers;
        }

        /// <summary>
        ///     Function that is called when the behaviour becomes disabled
        /// </summary>
        private void OnDisable() {
            events.UpdateQuestionAnswer -= UpdateAnswers;
        }

        /// <summary>
        ///     Function that is called on the frame when a script is enabled just before any of the Update methods are called the
        ///     first time.
        /// </summary>
        private void Awake() {
            events.CurrentFinalScore = 0;
        }

        /// <summary>
        ///     Function that is called when the script instance is being loaded.
        /// </summary>
        private void Start() {
            events.StartupHighscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

            timerDefaultColor = timerText.color;
            LoadQuestions();

            timerStateParaHash = Animator.StringToHash("TimerState");

            var seed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(seed);

            Display();
            
            // Start timer once
            IE_StartTimer = StartTimer();
            StartCoroutine(IE_StartTimer);

            timerAnimtor.SetInteger(timerStateParaHash, 2);
        }

        #endregion

        #region Timer Methods

        private IEnumerator StartTimer() {
            var totalTime = 20;
            var timeLeft = totalTime;

            timerText.color = timerDefaultColor;
            while (timeLeft > 0) {
                timeLeft--;

                AudioManager.Instance.PlaySound("CountdownSFX");

                if (timeLeft < totalTime / 2 && timeLeft > totalTime / 4) {
                    timerText.color = timerHalfWayOutColor;
                }

                if (timeLeft < totalTime / 4) {
                    timerText.color = timerAlmostOutColor;
                }

                timerText.text = timeLeft.ToString();
                yield return new WaitForSeconds(1.0f);
            }

            // Finalize scoring and show Finish Display Elements
            events.ScoreUpdated?.Invoke();

            SetHighscore();

            var type = UIManager.ResolutionScreenType.Finish;

            events.DisplayResolutionScreen?.Invoke(type, Questions[currentQuestion].AddScore);

            GameObject
                .Find("ResolutionCanvas/ResolutionScreen/State_Info_Text")
                .GetComponent<TextMeshProUGUI>()
                .SetText("FINAL SCORE:" + events.CurrentFinalScore.ToString());

            AudioManager.Instance.PlaySound("IncorrectSFX");
        }

        private IEnumerator WaitTillNextRound() {
            yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
            Display();
        }

        #endregion

        #region Getters

        private Question GetRandomQuestion() {
            var randomIndex = GetRandomQuestionIndex();
            currentQuestion = randomIndex;

            return Questions[currentQuestion];
        }

        private int GetRandomQuestionIndex() {
            var random = 0;
            if (FinishedQuestions.Count < Questions.Length) {
                do {
                    random = Random.Range(0, Questions.Length);
                } while (FinishedQuestions.Contains(random) || random == currentQuestion);
            }

            return random;
        }

        #endregion
    }
}
