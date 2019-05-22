using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Database.Enum;
using Assets.Scripts.GlobalScripts;
using Assets.Scripts.GlobalScripts.Player;
using Assets.Scripts.GlobalScripts.UITask;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Assets.Scripts.QuizSolveMath {
#pragma warning disable 649
    public class MathQuestionManager : MonoBehaviour {
        private int _currentNumber;

        private int _inputLim;

        private List<int> _keys;

        [SerializeField] private MathBank[] _mathBanks;

        private int _randomKey;

        private int _score;

        private ScoreManager _scoreManager;

        private Timer _timer;

        private int _useKey;

        private bool _gameDone;

        private void Start() {
            _scoreManager = new ScoreManager();

            _keys = new List<int>();

            _timer = GetComponent<Timer>();

            _timer.StartTimerAt(2, 0f);

            for (var i = 0; i < _mathBanks.Length; i++) {
                _keys.Add(i);
            }

            PrepareQuestion();
        }

        private void Update() {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_timer.Min < 0 && _timer.Sec == 0) {
                _timer.ChangeTimerState();
                _timer.TimerText.SetText("00:00");

                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name.Equals("panel failed"))
                    .Panel
                    .transform
                    .gameObject
                    .SetActive(true);
            }

            // Game finish if all question has been answered
            if (_currentNumber > _keys.Count && !_gameDone) {

                _gameDone = !_gameDone;

                Array.Find(
                        FindObjectOfType<UIManager>().PanelCollection,
                        i => i.Name.Equals(_score > 0 ? "panel success" : "panel failed"))
                    .Panel
                    .transform
                    .gameObject
                    .SetActive(true);

                BaseScoreHandler baseScoreHandler = new BaseScoreHandler();
                baseScoreHandler.SaveScore(_score, Game.GameType.ProblemSolving);
            }
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

            // Set random question      
            Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.textName.Equals("question text"))
                .textMesh
                .SetText(_mathBanks[_useKey].Problem);

            // Avoid using the same spawn point
            _keys.RemoveAt(_randomKey);
        }

        public void IsAnswerCorrect() {
            var userAnswer = Array.Find(FindObjectOfType<UIManager>().TextCollection,
                    i => i.textName.Equals("answer text"))
                .textMesh
                .text;

            var correctAnswer = _mathBanks[_useKey].Answer;

            if (userAnswer.Equals(correctAnswer)) {
                _score += 10;
            } else if (_score > 0) {
                _score -= 10;
            }

            // Set text with new score
            Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.textName.Equals("score text"))
                .textMesh
                .SetText("TotalScore:" + _score);

            ClearInput();

            PrepareQuestion();
        }

        public void GetButtonContent() {
            _inputLim++;

            if (_inputLim > 3) {
                ClearInput();

                _inputLim = 0;
            }

            var btnContent = EventSystem.current.currentSelectedGameObject.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text;

            Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.textName.Equals("answer text"))
                .textMesh
                .text += btnContent;
        }

        public void ClearInput() {
            Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.textName.Equals("answer text"))
                .textMesh
                .SetText(string.Empty);
        }
    }
}
