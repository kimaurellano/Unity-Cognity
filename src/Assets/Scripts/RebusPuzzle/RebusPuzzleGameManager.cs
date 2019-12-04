using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.GlobalScripts.Managers.UIManager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.RebusPuzzle {
    [RequireComponent(typeof(ActionManager))]
    public class RebusPuzzleGameManager : CoreGameBehaviour {

        [SerializeField] private RebusPuzzleCollection[] _rebusPuzzleCollections;
        [SerializeField] private Image _puzzlePictureContainer;

        private BaseScoreHandler _baseScoreHandler;
        private TimerManager _timerManager;
        private UIManager _uiManager;
        private TMP_InputField _answerField;
        private Animator _scoreAddAnimator;
        private AudioManager _audioManager;
        private Randomizer<Sprite> _randomizer;

        private int _currentLevel = (int) Difficulty.DifficultyLevel.Easy;

        private void Start() {
            _randomizer = new Randomizer<Sprite>();

            _timerManager = GetComponent<TimerManager>();

            _uiManager = FindObjectOfType<UIManager>();

            _audioManager = FindObjectOfType<AudioManager>();

            TimerManager.OnGameTimerEndEvent += EndGame;

            TimerManager.OnPreGameTimerEndEvent += StartTimer;

            _baseScoreHandler = new BaseScoreHandler(0, _rebusPuzzleCollections.Length);
        }

        private void StartTimer() {
            TimerManager.OnPreGameTimerEndEvent -= StartTimer;

            _timerManager.StartTimerAt(0, 45f);

            // Populate list of puzzles with current level
            SetDifficulty(Difficulty.ParseLevel(_currentLevel));

            _puzzlePictureContainer.sprite = _randomizer.GetRandomItem();
        }

        public override void EndGame() {
            TimerManager.OnGameTimerEndEvent -= EndGame;

            _baseScoreHandler.SaveScore(UserStat.GameCategory.Language);

            ShowGraph(
                UserStat.GameCategory.Language,
                _baseScoreHandler.Score,
                _baseScoreHandler.ScoreLimit);

            base.EndGame();
        }

        public void CheckAnswer() {
            _answerField = (TMP_InputField)_uiManager.GetUI(UIType.InputField, "answer");

            _scoreAddAnimator =
                (Animator)_uiManager.GetUI(UIType.AnimatedMultipleState, "score add anim");

            if (_answerField.text.ToLower().Equals(_randomizer.GetCurrentItem().name.ToLower())) {
                FindObjectOfType<AudioManager>().PlayClip("sfx_correct");

                _scoreAddAnimator.GetComponent<TextMeshProUGUI>().SetText("CORRECT!");
                _scoreAddAnimator.SetTrigger("correct");

                _baseScoreHandler.AddScore(1);

                _answerField.text = string.Empty;
            }
            else {
                FindObjectOfType<AudioManager>().PlayClip("sfx_incorrect");

                _scoreAddAnimator.GetComponent<TextMeshProUGUI>().SetText("WRONG!");
                _scoreAddAnimator.SetTrigger("wrong");
            }

            if (_randomizer.IsEmpty) {
                _currentLevel++;

                // We only have 3 levels
                if (_currentLevel > 2) {
                    EndGame();

                    return;
                }

                SetDifficulty(Difficulty.ParseLevel(_currentLevel));
            }

            _puzzlePictureContainer.sprite = _randomizer.GetRandomItem();
        }

        private void SetDifficulty(Difficulty.DifficultyLevel difficulty) {
            _randomizer.ClearList();

            // Populate question list based on a category
            foreach (var puzzle in _rebusPuzzleCollections.Where(i => i.Difficulty == difficulty)) {
                _randomizer.AddToList(puzzle.Image);
            }
        }
    }
}
