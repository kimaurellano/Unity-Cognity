using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GlobalScripts.Player;
using Assets.Scripts.GlobalScripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Assets.Scripts.GlobalScripts.Player.BaseScoreHandler;

namespace Assets.Scripts.Memory {
    public class GameManager : MonoBehaviour {

        private List<Transform> _lockedCardList;

        private GameManager _gameManager;

        private float _seconds;

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

        public int LockCount { get; set; }

        public bool OnFlip { get; set; }

        public bool TimerPause { get; set; }

        private void Start() {
            _lockedCardList = new List<Transform>();

            if (_gameManager != null) {
                Destroy(_gameManager);
            } else {
                _gameManager = this;
            }

            DontDestroyOnLoad(this);
        }

        private void Update() {
            if (SceneManager.GetActiveScene().name == "GameOverMemory") {
                foreach (var button in GameObject.FindGameObjectsWithTag("Button")) {
                    if (button.name == "ButtonNo") {
                        button.GetComponent<Button>().onClick.AddListener(DestroyAllManager);
                    } else {
                        button.GetComponent<Button>().onClick.AddListener(DestroyGameManager);
                    }
                }

                Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.Name == "game result text")
                    .textMesh
                    .SetText(_seconds > 0f ? "Success" : "Failed");

                return;
            }
            
            // There are 7 pairs
            if (LockCount < 7) {
                return;
            }

            // Reset
            LockCount = 0;

            _seconds = GameObject.Find("CardSpawn").GetComponent<SpawnManager>().Sec;

            // Add score 
            BaseScoreHandler baseScoreHandler = new BaseScoreHandler();
            baseScoreHandler.AddScore(_seconds, GameType.Memory);

            // Load finished scene
            SceneManager.LoadScene("GameOverMemory");
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

        public void Pause() {
            TimerPause = !TimerPause;

            // This should avoid the user from picking the card's gameobjects when paused
            if (TimerPause) {
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

            Time.timeScale = TimerPause ? 0f : 1f;
        }

        private void DestroyAllManager() {
            Destroy(gameObject);

            // Prevents playing the audio on quit
            Destroy(GameObject.Find("AudioManager").transform.gameObject);
        }

        private void DestroyGameManager() {
            Destroy(gameObject);
        }
    }
}
