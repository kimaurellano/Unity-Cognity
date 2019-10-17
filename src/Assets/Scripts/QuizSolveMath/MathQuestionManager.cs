using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GlobalScripts.Player;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using static Assets.Scripts.GlobalScripts.Player.BaseScoreHandler;

namespace Assets.Scripts.QuizSolveMath {
#pragma warning disable 649
    public class MathQuestionManager : CoreGameBehaviour {

        [SerializeField] private MathBank[] _mathBanks;

        private UIManager _uiManager;
        private AudioManager _audioManager;
        private TimerManager _timerManager;
        private List<int> _keys;

        private int _currentNumber;
        private int _inputLim;
        private int _useKey;
        private bool _gameDone;
        private bool _paused;
        private int _randomKey;
        private int _score;

        private void Start() {
            _uiManager = FindObjectOfType<UIManager>();

            _audioManager = FindObjectOfType<AudioManager>();

            _keys = new List<int>();

            _timerManager = GetComponent<TimerManager>();

            TimerManager.OnPreGameTimerEndEvent += StartGame;
        }

        private void StartGame() {
            TimerManager.OnPreGameTimerEndEvent -= StartGame;

            _timerManager.StartTimer();

            TimerManager.OnGameTimerEndEvent += EndGame;

            for (var i = 0; i < _mathBanks.Length; i++) {
                _keys.Add(i);
            }

            PrepareQuestion();
        }

        /// <summary>
        ///     Randomizes question
        /// </summary>
        public void PrepareQuestion() {
            // Monitor current question we are at
            _currentNumber++;

            // Generate random spawn
            _randomKey = Random.Range(0, _keys.Count);

            // Set random spawn point
            _useKey = _keys.ElementAt(_randomKey);

            // Set random question cached at TextCollection for later comparison with the user answer
            TextMeshProUGUI questionText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "question");
            questionText.SetText(_mathBanks[_useKey].Problem);

            // Avoid using the same spawn point
            _keys.RemoveAt(_randomKey);

            TextMeshProUGUI questionHeader = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "question header");
            questionHeader.SetText(string.Format("Question #{0}", _currentNumber));
        }

        public void IsAnswerCorrect() {
            TMP_InputField inputField = (TMP_InputField)_uiManager.GetUI(UIManager.UIType.InputField, "answer");
            string userAnswer = inputField.text;

            string correctAnswer = _mathBanks[_useKey].Answer;

            if (userAnswer.Equals(correctAnswer)) {
                TextMeshProUGUI scoreChange = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "score change");
                scoreChange.color = Color.green;
                scoreChange.text = "+10";

                // Get the Animation component
                Animation anim = (Animation)_uiManager.GetUI(UIManager.UIType.AnimatedSingleState, "score change");
                anim.Play();

                Animator animator = (Animator)_uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "answer");
                animator.SetTrigger("correct");

                _score += 10;
            } else {
                TextMeshProUGUI textUI = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "score change");
                textUI.color = Color.red;
                textUI.text = "-10";

                Animation anim = (Animation) _uiManager.GetUI(UIManager.UIType.AnimatedSingleState, "score change");
                anim.Play();

                Animator animator = (Animator)_uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "answer");
                animator.SetTrigger("wrong");

                _score -= 10;
            }

            // Avoid negative result
            if (_score <= 0) {
                _score = 0;
            }

            // Set text with new score
            TextMeshProUGUI scoreText = (TextMeshProUGUI) _uiManager.GetUI(UIManager.UIType.Text, "score");
            scoreText.SetText(string.Format("Score:{0}", _score));

            PrepareQuestion();

            // Clear input field
            TMP_InputField answerField = (TMP_InputField) _uiManager.GetUI(UIManager.UIType.InputField, "answer");
            answerField.text = string.Empty;

            if(_currentNumber > _keys.Count) {
                EndGame();
            }
        }

        public override void EndGame() {
            _timerManager.ChangeTimerState();

            _gameDone = !_gameDone;

            SaveScore(_score, GameType.ProblemSolving);

            TextMeshProUGUI gameResulText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "game result");
            gameResulText.SetText(_score > 0 ? "SUCCESS!" : "FAILED");

            Transform panel = (Transform)_uiManager.GetUI(UIManager.UIType.Panel, "game result");
            panel.gameObject.SetActive(true);
        }

        public override void Pause() {
            base.Pause();
        }
    }
}