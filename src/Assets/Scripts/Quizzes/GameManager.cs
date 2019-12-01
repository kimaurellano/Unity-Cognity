using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Quizzes {
    public class GameManager : CoreGameBehaviour {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _question;
        [SerializeField] private GameObject _answerPrefab;
        [SerializeField] private Transform _answersContainer;

        [Header("Add questions here")]
        [Space]
        [SerializeField] private QuestionBankQuiz[] _questions;

        private AudioManager _audioManager;
        private TimerManager _timerManager;
        private List<QuestionBankQuiz> _currentQuestions;
        private QuestionBankQuiz.Difficulty _currentDifficulty;
        private BaseScoreHandler _baseScoreHandler;

        private int _point = 10;
        private int _questionLimitCount = 10;
        private int _currentQuestionNumber;
        private int _level = (int)QuestionBankQuiz.Difficulty.Easy;

        private void Start() {
            _audioManager = FindObjectOfType<AudioManager>();
            _timerManager = GetComponent<TimerManager>();

            TimerManager.OnPreGameTimerEndEvent += StartGame;
            TimerManager.OnGameTimerEndEvent += EndGame;

            OnEndGameEvent += EndGame;

            _currentQuestions = new List<QuestionBankQuiz>();

            AnswerScript.OnSelectEvent += CheckAnswer;

            SceneManager.activeSceneChanged += RemoveEvents;

            // max score = points * how many questions;
            _baseScoreHandler = new BaseScoreHandler(0, _point * _questions.Length);
        }

        private void RemoveEvents(Scene current, Scene next) {
            SceneManager.activeSceneChanged -= RemoveEvents;
            AnswerScript.OnSelectEvent -= CheckAnswer;
            TimerManager.OnGameTimerEndEvent -= EndGame;
        }

        private void StartGame() {
            TimerManager.OnPreGameTimerEndEvent -= StartGame;

            _timerManager.StartTimerAt(1, 0f);

            _currentDifficulty = QuestionBankQuiz.Difficulty.Easy;
            SetDifficulty(_currentDifficulty);
            DisplayAnswerOption();
        }

        private void SetDifficulty(QuestionBankQuiz.Difficulty difficulty) {
            foreach (var question in _questions.Where(i => i.QuestionDifficulty == difficulty)) {
                _currentQuestions.Add(question);
            }

            if (_currentQuestions.Count <= 0) {
                throw new NullReferenceException("There are no questions.");
            }
        }

        private void DisplayAnswerOption() {
            _question.SetText(_currentQuestions[_currentQuestionNumber].Question);

            // Get possible answers in the current question
            for (int possibleAnswer = 0; possibleAnswer < _currentQuestions[_currentQuestionNumber].Answers.Length; possibleAnswer++) {
                GameObject questionPrefab = Instantiate(_answerPrefab, _answersContainer);
                AnswerScript answerScript = questionPrefab.GetComponent<AnswerScript>();

                // Set contents of possible answer
                answerScript.AnswerText = _currentQuestions[_currentQuestionNumber].Answers[possibleAnswer].AnswerText;
                answerScript.IsCorrect = _currentQuestions[_currentQuestionNumber].Answers[possibleAnswer].IsCorrect;
            }
        }

        private void CheckAnswer(bool isCorrect) {
            Debug.Log(isCorrect ? "Correct" : "Incorrect");

            if (isCorrect) {
                _audioManager.PlayClip("sfx_correct");

                _baseScoreHandler.AddScore(_point);
                _scoreText.SetText($"Score:{_baseScoreHandler.Score}");
            } else {
                _audioManager.PlayClip("sfx_incorrect");
            }

            ClearAnswersContainer();

            _currentQuestionNumber++;
            if (_currentQuestionNumber > _questions.Length - 1) {
                Debug.Log("End game");

                EndGame();

                return;
            }

            // Every 10 questions increase difficulty
            if (_currentQuestionNumber > _questionLimitCount) {
                _level++;
                SetDifficulty((QuestionBankQuiz.Difficulty)Enum.ToObject(typeof(QuestionBankQuiz.Difficulty), _level));

                _questionLimitCount += 10;
            }

            DisplayAnswerOption();
        }

        private void ClearAnswersContainer() {
            for (int i = 0; i < _answersContainer.childCount; i++) {
                Destroy(_answersContainer.GetChild(i).gameObject);
            }
        }

        public override void EndGame() {
            _baseScoreHandler.SaveScore(UserStat.GameCategory.Language);

            base.EndGame();
        }
    }
}
