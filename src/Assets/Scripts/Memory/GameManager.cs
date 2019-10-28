using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Player;
using Assets.Scripts.GlobalScripts.Managers;
using UnityEngine;
using static Assets.Scripts.GlobalScripts.Player.BaseScoreHandler;
using TMPro;

namespace Assets.Scripts.Memory {
    public class GameManager : CoreGameBehaviour {

        private List<Transform> _lockedCardList;

        private GameManager _gameManager;

        private TimerManager _timerManager;

        private UIManager _uiManager;

        private float _seconds;

        private int _lockCount;

        [SerializeField] private Transform _firstPick;

        [SerializeField] private Transform _secondPick;

        public Transform FirstPick {
            get => _firstPick;
            set => _firstPick = value;
        }

        public Transform SecondPick {
            get => _secondPick;
            set => _secondPick = value;
        }

        public int TouchCount { get; set; }

        public bool OnFlip { get; set; }

        private void Start() {
            _uiManager = FindObjectOfType<UIManager>();

            _timerManager = GetComponent<TimerManager>();

            _lockedCardList = new List<Transform>();

            TimerManager.OnPreGameTimerEndEvent += StartGameTimer;

            TimerManager.OnGameTimerEndEvent += EndGame;

            TouchManager.OnCardLockEvent += IncrementLocks;
        }

        private void StartGameTimer() {
            TimerManager.OnPreGameTimerEndEvent -= StartGameTimer;

            _timerManager.StartTimerAt(0, 45f);
        }

        private void IncrementLocks() {
            _lockCount++;

            if(_lockCount == 7) {
                GameResult(true);
            }
        }

        public override void EndGame() {
            base.EndGame();

            GameResult(false);
        }

        private void GameResult(bool success) {
            TimerManager.OnGameTimerEndEvent -= EndGame;

            // Reset
            _lockCount = 0;

            // Add score 
            BaseScoreHandler baseScoreHandler = new BaseScoreHandler();
            baseScoreHandler.AddScore(_seconds, GameType.Memory);

            // Load finished scene
            Transform gameResultPanel = (Transform)_uiManager.GetUI(UIManager.UIType.Panel, "game result");
            gameResultPanel.gameObject.SetActive(true);

            TextMeshProUGUI gameResultText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "game result text");
            gameResultText.SetText(success ? "SUCCESS!" : "FAILED");
        }

        public IEnumerator WaitForFlip() {
            OnFlip = true;

            yield return new WaitForSeconds(1f);

            _firstPick.GetComponent<Animator>().SetBool("flip", true);
            _secondPick.GetComponent<Animator>().SetBool("flip", true);

            _firstPick = null;
            _secondPick = null;

            OnFlip = false;
        }

        public override void Pause() {
            base.Pause();

            // This should avoid the user from picking the card's gameobjects when paused
            if (IsPaused) {
                foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card")) {
                    if (card.GetComponent<Card>().Locked) {
                        continue;
                    }

                    // Get all non-locked cards on pause
                    _lockedCardList.Add(card.GetComponent<Transform>());

                    // Lock them temporarily
                    card.GetComponent<Card>().Locked = true;
                }
            } else {
                // Unlock the cards from the list
                foreach (Transform card in _lockedCardList) {
                    card.GetComponent<Card>().Locked = false;
                }

                // Set free the temporary locked cards
                _lockedCardList.Clear();
            }
        }
    }
}
