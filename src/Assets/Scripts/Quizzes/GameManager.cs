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
    [RequireComponent(typeof(ActionManager))]
    public class GameManager : CoreGameBehaviour {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _question;
        [SerializeField] private GameObject _answerPrefab;
        [SerializeField] private Transform _answersContainer;

        [Header("Add questions here")]
        [Space]
        [SerializeField] private QuestionBankQuiz[] _questions;

        private Randomizer<QuestionBankQuiz> _randomizer;
        private AudioManager _audioManager;
        private TimerManager _timerManager;
        private List<QuestionBankQuiz> _currentQuestions;
        private Difficulty.DifficultyLevel _currentDifficulty;
        private BaseScoreHandler _baseScoreHandler;

        private int _point = 10;
        private int _currentQuestionNumber;
        private int _level = (int)Difficulty.DifficultyLevel.Easy;

        private void Start() {
            _randomizer = new Randomizer<QuestionBankQuiz>();

            _audioManager = FindObjectOfType<AudioManager>();
            _timerManager = GetComponent<TimerManager>();

            TimerManager.OnPreGameTimerEndEvent += StartGame;
            TimerManager.OnGameTimerEndEvent += EndGame;

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

            _currentDifficulty = Difficulty.DifficultyLevel.Easy;

            SetDifficulty(_currentDifficulty);

            DisplayAnswerOption();
        }

        private void SetDifficulty(Difficulty.DifficultyLevel difficulty) {
            _randomizer.ClearList();

            // Populate question list based on a category
            foreach (var question in _questions.Where(i => i.Difficulty == difficulty)) {
                _randomizer.AddToList(question);
            }
        }

        private void DisplayAnswerOption() {
            QuestionBankQuiz questionBankQuiz = _randomizer.GetRandomItem();

            _question.SetText(questionBankQuiz.Question);

            // Get possible answers in the current question
            for (int possibleAnswer = 0; possibleAnswer < questionBankQuiz.Answers.Length; possibleAnswer++) {
                GameObject questionPrefab = Instantiate(_answerPrefab, _answersContainer);
                AnswerScript answerScript = questionPrefab.GetComponent<AnswerScript>();

                // Set contents of possible answer
                answerScript.AnswerText = questionBankQuiz.Answers[possibleAnswer].AnswerText;
                answerScript.IsCorrect = questionBankQuiz.Answers[possibleAnswer].IsCorrect;
            }
        }

        private void CheckAnswer(bool isCorrect) {
            Debug.Log(isCorrect ? "Correct" : "Incorrect");

            if (isCorrect) {
                GetAttachedAudioComponents().FirstOrDefault(i => i.clip.name == "CorrectSFX")?.Play();

                _baseScoreHandler.AddScore(_point);
                _scoreText.SetText($"Score:{_baseScoreHandler.Score}");
            } else {
                GetAttachedAudioComponents().FirstOrDefault(i => i.clip.name == "IncorrectSFX")?.Play();
            }

            ClearAnswersContainer();

            // Every 10 questions increase difficulty
            if (_randomizer.IsEmpty) {
                _level++;

                // Upon finishing the Hard part end the game
                if (_level > 2) {
                    EndGame();

                    return;
                }

                SetDifficulty(Difficulty.ParseLevel(_level));
            }

            DisplayAnswerOption();
        }

        private void ClearAnswersContainer() {
            for (int i = 0; i < _answersContainer.childCount; i++) {
                Destroy(_answersContainer.GetChild(i).gameObject);
            }
        }

        private AudioSource[] GetAttachedAudioComponents() {
            return GetComponents<AudioSource>();
        }

        public override void EndGame() {
            _baseScoreHandler.SaveScore(UserStat.GameCategory.Language);

            ShowGraph(
                UserStat.GameCategory.Language,
                _baseScoreHandler.Score,
                _baseScoreHandler.ScoreLimit);

            base.EndGame();
        }
    }
}
