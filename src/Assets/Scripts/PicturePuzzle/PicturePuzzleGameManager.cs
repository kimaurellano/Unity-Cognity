using System;
using System.Linq;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.GlobalScripts.Managers.UIManager;
using static Assets.Scripts.GlobalScripts.Player.BaseScoreHandler;

namespace Assets.Scripts.PicturePuzzle {
    public class PicturePuzzleGameManager : CoreGameBehaviour {

        [SerializeField] private PicturePuzzleCollection[] _picturePuzzleCollections;
        [SerializeField] private Image _puzzlePictureContainer;

        private TimerManager _timerManager;
        private PicturePuzzleGameManager _picturePuzzleGameManager;
        private UIManager _uiManager;
        private TMP_InputField _answerField;
        private Animator _scoreAddAnimator;
        private AudioManager _audioManager;
        private bool _paused;
        private int _currentNumber = 1;
        private int _score;

        private void Start() {
            _timerManager = GetComponent<TimerManager>();

            _uiManager = FindObjectOfType<UIManager>();

            _audioManager = FindObjectOfType<AudioManager>();

            //AudioSource src = gameObject.AddComponent<AudioSource>();
            //src.clip = _audioManager.GetSrc("incorrect").clip;
            //src.Play();

            Instantiate(Array.Find(_picturePuzzleCollections, i => i.puzzleId == _currentNumber).Image, _puzzlePictureContainer.transform);

            TimerManager.OnGameTimerEndEvent += EndGame;

            TimerManager.OnPreGameTimerEndEvent += StartTimer;
        }

        private void StartTimer() {
            TimerManager.OnPreGameTimerEndEvent -= StartTimer;

            _timerManager.StartTimerAt(0, 45f);
        }

        public override void Pause() {
            base.Pause();
        }

        public override void EndGame() {
            TimerManager.OnGameTimerEndEvent -= EndGame;

            SaveScore(_score, GameType.Language);

            Transform finishPanel = (Transform)_uiManager.GetUI(UIType.Panel, "game result");
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
                FindObjectOfType<AudioManager>().PlayClip("sfx_correct");

                _scoreAddAnimator.GetComponent<TextMeshProUGUI>().SetText("CORRECT!");
                _scoreAddAnimator.SetTrigger("correct");

                // Clear picture puzzle container before next puzzle
                Destroy(_puzzlePictureContainer.transform.GetChild(0).gameObject);

                NextPuzzle();

                _answerField.text = string.Empty;
            }
            else {
                FindObjectOfType<AudioManager>().PlayClip("sfx_incorrect");

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
    }
}
