using System;
using System.Collections;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.GlobalScripts.Managers.UIManager;
using static Assets.Scripts.GlobalScripts.Player.BaseScoreHandler;

#pragma warning disable 649

namespace Assets.Scripts.PicturePuzzle {
    public class PicturePuzzleGameManager : CoreGameBehaviour {

        [SerializeField] private PicturePuzzleCollection[] _picturePuzzleCollections;
        [SerializeField] private Image _puzzlePictureContainer;

        private TimerManager _timerManager;
        private PicturePuzzleGameManager _picturePuzzleGameManager;
        private UIManager _uiManager;
        private TMP_InputField _answerField;
        private Animator _scoreAddAnimator;
        private bool _paused;
        private int _currentNumber = 1;
        private int _score;

        private void Start() {
            _timerManager = GetComponent<TimerManager>();
            _uiManager = FindObjectOfType<UIManager>();

            TimerManager.OnPreGameTimerEndEvent += StartWaitForEndTimer;
            TimerManager.OnPreGameTimerEndEvent += StartTimer;

            TimerManager.OnGameTimerEndEvent += EndGame;

            Instantiate(Array.Find(_picturePuzzleCollections, i => i.puzzleId == _currentNumber).Image, _puzzlePictureContainer.transform);
        }

        public override void Pause() {
            base.Pause();
        }

        public override void EndGame() {
            TimerManager.OnGameTimerEndEvent -= EndGame;

            SaveScore(_score, GameType.Language);

            Transform finishPanel = (Transform)_uiManager.GetUI(UIType.Panel, "panel game finish");
            finishPanel.gameObject.SetActive(true);

            TextMeshProUGUI gameResultText = (TextMeshProUGUI)_uiManager.GetUI(UIType.Text, "game result");
            gameResultText.SetText("SUCCESS!");
        }

        public void CheckAnswer() {
            string answer = Array.Find(_picturePuzzleCollections, i => i.puzzleId == _currentNumber).Answer;

            _answerField = (TMP_InputField)_uiManager.GetUI(UIType.InputField, "answer");

            _scoreAddAnimator =
                (Animator)_uiManager.GetUI(UIType.AnimatedMultipleState, "score add anim");

            if (_answerField.text.Contains(answer)) {
                _scoreAddAnimator.GetComponent<TextMeshProUGUI>().SetText("CORRECT!");
                _scoreAddAnimator.SetTrigger("correct");

                // Clear picture puzzle container before next puzzle
                Destroy(_puzzlePictureContainer.transform.GetChild(0).gameObject);

                NextPuzzle();

                _answerField.text = string.Empty;
            }
            else {
                _scoreAddAnimator.GetComponent<TextMeshProUGUI>().SetText("WRONG!");
                _scoreAddAnimator.SetTrigger("wrong");
            }
        }

        public void NextPuzzle() {
            _currentNumber++;
            
            if (_currentNumber > _picturePuzzleCollections.Length) {
                EndGame();
                return;
            }

            Instantiate(Array.Find(_picturePuzzleCollections, i => i.puzzleId == _currentNumber).Image, _puzzlePictureContainer.transform);

            // Add up the time left for each answered puzzle 
            _score += (int) _timerManager.Seconds;
        }

        private void StartWaitForEndTimer() {
            TimerManager.OnPreGameTimerEndEvent -= StartWaitForEndTimer;

            StartCoroutine(IEWaitForEndTimer());
        }

        private IEnumerator IEWaitForEndTimer() {
            yield return new WaitUntil(() => _timerManager.TimerUp);

            Transform gameFinishPanel = (Transform) _uiManager.GetUI(UIType.Panel, "panel game finish");
            gameFinishPanel.gameObject.SetActive(true);

            TextMeshProUGUI gameResultText = (TextMeshProUGUI) _uiManager.GetUI(UIType.Text, "game result");
            gameResultText.SetText("FAILED");
        }

        private void StartTimer() {
            // We do not need it
            TimerManager.OnPreGameTimerEndEvent -= StartTimer;

            _timerManager.StartTimerAt(_timerManager.Minutes, _timerManager.Seconds);
        }
    }
}
