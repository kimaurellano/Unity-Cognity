using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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

            if (_currentNumber > 10) {
                EndGame();

                return;
            }

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
            questionHeader.SetText($"Question #{_currentNumber}");
        }

        public void IsAnswerCorrect() {
            TMP_InputField inputField = (TMP_InputField)_uiManager.GetUI(UIManager.UIType.InputField, "answer");
            string userAnswer = inputField.text;

            string correctAnswer = _mathBanks[_useKey].Answer;

            if (userAnswer.Equals(correctAnswer)) {
                FindObjectOfType<AudioManager>().PlayClip("sfx_correct");

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
                FindObjectOfType<AudioManager>().PlayClip("sfx_incorrect");

                TextMeshProUGUI textUI = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "score change");
                textUI.color = Color.red;
                textUI.text = "-10";

                Animation anim = (Animation) _uiManager.GetUI(UIManager.UIType.AnimatedSingleState, "score change");
                anim.Play();

                Animator animator = (Animator)_uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "answer");
                animator.SetTrigger("wrong");
            }

            // Set text with new score
            TextMeshProUGUI scoreText = (TextMeshProUGUI) _uiManager.GetUI(UIManager.UIType.Text, "score");
            scoreText.SetText($"Score:{_score}");

            PrepareQuestion();

            // Clear input field
            TMP_InputField answerField = (TMP_InputField) _uiManager.GetUI(UIManager.UIType.InputField, "answer");
            answerField.text = string.Empty;

            if(_currentNumber > _keys.Count) {
                EndGame();
            }
        }

        public override void EndGame() {
            base.EndGame();

            _timerManager.ChangeTimerState();

            _gameDone = !_gameDone;

            BaseScoreHandler baseScoreHandler = new BaseScoreHandler(0, 100);
            baseScoreHandler.AddScore(_score);
            baseScoreHandler.SaveScore(UserStat.GameCategory.ProblemSolving);

            SceneManager.LoadScene(GetNextScene());
        }
    }
}