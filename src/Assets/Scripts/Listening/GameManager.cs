using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Listening {
    [RequireComponent(typeof(ActionManager))]
    public class GameManager : CoreGameBehaviour {
        [SerializeField] private AudioClip[] _words;
        [SerializeField] private TMP_InputField _answerField;
        [SerializeField] private TextMeshProUGUI _scoreText;

        private AudioSource _src;
        private TimerManager _timerManager;
        private BaseScoreHandler _baseScoreHandler;
        private List<int> _keys;

        private int _useKey;
        private int _currentClip;
        private int _randomKey;
        private int _currentAudio;

        private void Start() {
            _src = gameObject.AddComponent<AudioSource>();
            _timerManager = gameObject.AddComponent<TimerManager>();

            _baseScoreHandler = new BaseScoreHandler(0, _words.Length);

            _keys = new List<int>();
            for (var i = 0; i < _words.Length; i++) {
                _keys.Add(i);
            }

            _scoreText.SetText($"Words: {_baseScoreHandler.Score}/{_words.Length}");

            TimerManager.OnPreGameTimerEndEvent += StartGame;
            TimerManager.OnGameTimerEndEvent += EndGame;
        }

        private void StartGame() {
            _timerManager.StartTimerAt(1, 0f);
        }

        public void Submit() {
            if (_src.clip.name == _answerField.text) {
                _baseScoreHandler.AddScore(1);

                _scoreText.SetText($"Words: {_baseScoreHandler.Score}/{_words.Length}");
            }

            PrepareQuestion();
        }

        public void PrepareQuestion() {
            // Monitor current question we are at
            _currentClip++;

            if (_currentClip > _words.Length - 1) {
                EndGame();

                return;
            }

            // Generate random spawn
            _randomKey = Random.Range(0, _keys.Count);

            // Set random spawn point
            _useKey = _keys.ElementAt(_randomKey);

            _src.clip = _words[_useKey];

            // Avoid using the same spawn point
            _keys.RemoveAt(_randomKey);
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
