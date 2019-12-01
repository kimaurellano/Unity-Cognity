using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.RunningFigure {
    public class GameManager : CoreGameBehaviour {

        [SerializeField] private TextMeshProUGUI _numberText;
        [SerializeField] private Image _runningImageHolder;
        [SerializeField] private LevelScript[] _level;

        private List<GameObject> _figurePrefabs;
        private BaseScoreHandler _baseScoreHandler;
        private TimerManager _timerManager;
        private Vector2 _screenBounds;
        private const int _maxScore = 15;
        private int _curLevel = (int)LevelScript.Level.Easy;
        private float _moveSpeed, _spawnRate;
        private float _score;
        private int lives = 3;
        private bool _gameStarted;

        public Image RunningImage { get => _runningImageHolder; private set => _runningImageHolder = value; }

        public float Number { get; set; }

        private void Start() {
            _timerManager = gameObject.AddComponent<TimerManager>();

            _figurePrefabs = new List<GameObject>();

            _baseScoreHandler = new BaseScoreHandler(0, _maxScore);

            _screenBounds =
                Camera.main.ScreenToWorldPoint(new Vector3(
                    Screen.width,
                    Screen.height,
                    Camera.main.transform.position.z));

            SceneManager.activeSceneChanged += RemoveEvents;

            TimerManager.OnPreGameTimerEndEvent += StartSpawn;

            TouchManager.OnImageCatchEvent += ChangeFigure;
            TouchManager.OnImageCatchEvent += IncreaseScore;

            _moveSpeed = 2.5f;
            _spawnRate = 1.5f;
        }

        private void Update() {
            // Increase difficulty every 20 seconds
            if (!_timerManager.Active && _gameStarted) {
                _timerManager.StartTimerAt(0, 20f);

                _curLevel++;

                if (_curLevel > 2) {
                    EndGame();

                    return;
                } 

                ChangeFigure();
            }
        }

        private void StartSpawn() {
            TimerManager.OnPreGameTimerEndEvent -= StartSpawn;

            _timerManager.StartTimerAt(0, 20f);

            _gameStarted = true;

            ChangeFigure();

            StartCoroutine(SpawnAnswer());
        }

        private IEnumerator SpawnAnswer() {
            while (true) {
                int randomPrefabToSpawn = Random.Range(0, _figurePrefabs.Count - 1);
                // Random instantiation of prefab
                GameObject spawnedPrefab = Instantiate(
                    _figurePrefabs[randomPrefabToSpawn],
                    // Top screen + offset to offscreen the prefabs instantiation
                    new Vector3(Random.Range(-_screenBounds.x + 0.5f, _screenBounds.x - 0.5f),
                        _screenBounds.y + 0.5f, 0f),
                    Quaternion.identity);

                FigureScript figureScript = spawnedPrefab.GetComponent<FigureScript>();
                figureScript.Number = Random.Range((int)Number - 2, (int)Number + 2);
                if ((LevelScript.Level) Enum.ToObject(typeof(LevelScript.Level), _curLevel) != LevelScript.Level.Easy) {
                    figureScript.Number += 0.5f;
                }
                figureScript.MoveSpeed = _moveSpeed;

                // Prevent spawning multiple time at n second
                yield return new WaitForSeconds(_spawnRate);
            }
        }

        private void IncreaseScore() {
            _score++;
            _baseScoreHandler.AddScore(_score);

            if (_score > _maxScore) {
                EndGame();
            }
        }

        private void ChangeFigure() {
            LevelScript.Level level = (LevelScript.Level) Enum.ToObject(typeof(LevelScript.Level), _curLevel);
            LevelScript levelScript = Array.Find(_level, i => i.GameLevel == level);

            _figurePrefabs.Clear();

            foreach (var levelScriptPrefab in levelScript._prefabs) {
                _figurePrefabs.Add(levelScriptPrefab);
            }

            float rndNumber = (int)Random.Range(levelScript.RangeFrom, levelScript.RangeTo);
            if (level != LevelScript.Level.Easy) {
                rndNumber += 0.5f;
                _numberText.SetText(rndNumber.ToString("##.0"));
            } else {
                _numberText.SetText(rndNumber.ToString());
            }

            // Tell to spawn figure with number near this value
            Number = rndNumber;

            // Random display of what figure to catch
            _runningImageHolder.sprite =
                _figurePrefabs[Random.Range(0, _figurePrefabs.Count - 1)]
                    .GetComponent<SpriteRenderer>().sprite;
        }

        public void DecreaseLife() {
            lives--;
            if (lives <= 0) {
                EndGame();
            }
        }

        private void RemoveEvents(Scene current, Scene next) {
            SceneManager.activeSceneChanged -= RemoveEvents;
            TouchManager.OnImageCatchEvent -= ChangeFigure;
            TouchManager.OnImageCatchEvent -= IncreaseScore;
        }

        public override void EndGame() {
            _baseScoreHandler.SaveScore(UserStat.GameCategory.Speed);

            base.EndGame();
        }
    }
}
