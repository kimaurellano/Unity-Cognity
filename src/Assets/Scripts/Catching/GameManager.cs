using System.Collections;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Catching {
    [RequireComponent(typeof(ActionManager))]
    public class GameManager : CoreGameBehaviour {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GameObject _fallingObject;

        private BaseScoreHandler _baseScoreHandler;
        private Vector2 _screenBounds;
        private float _moveSpeed;
        private float _spawnRate;
        private float _score;
        private int _life = 3;

        private void Start() {
            SceneManager.activeSceneChanged += RemoveEvent;

            FallingObjectScript.OnMissedEvent += DecreaseLife;

            TimerManager.OnPreGameTimerEndEvent += StartSpawn;

            _baseScoreHandler = new BaseScoreHandler(0, 10);

            _screenBounds =
                Camera.main.ScreenToWorldPoint(new Vector3(
                    Screen.width, 
                    Screen.height, 
                    Camera.main.transform.position.z));

            // Starting speed
            _moveSpeed = 0.5f;
            _spawnRate = 3f;

            _scoreText.SetText("0/10");
        }

        private void RemoveEvent(Scene current, Scene next) {
            SceneManager.activeSceneChanged -= RemoveEvent;
            FallingObjectScript.OnMissedEvent -= DecreaseLife;
        }

        private void StartSpawn() {
            TimerManager.OnPreGameTimerEndEvent -= StartSpawn;

            StartCoroutine(SpawnAnswer());
        }

        private IEnumerator SpawnAnswer() {
            while (true) {
                GameObject spawnedPrefab = Instantiate(
                    _fallingObject,
                    // Top screen + offset to offscreen the prefabs instantiation
                    new Vector3(Random.Range(-_screenBounds.x + 0.5f, _screenBounds.x - 0.5f),
                        _screenBounds.y + 0.5f, 0f),
                    Quaternion.identity);

                spawnedPrefab.GetComponent<FallingObjectScript>().MoveSpeed = _moveSpeed;

                // Prevent spawning multiple time at n second
                yield return new WaitForSeconds(_spawnRate);
            }
        }

        public override void EndGame() {
            _baseScoreHandler.SaveScore(UserStat.GameCategory.Speed);

            ShowGraph(
                UserStat.GameCategory.Speed,
                _baseScoreHandler.Score,
                _baseScoreHandler.ScoreLimit);

            base.EndGame();
        }

        private void DecreaseLife() {
            _life--;
            if (_life <= 0) {
                EndGame();
            }
        }

        public void IncreasePoint() {
            _score++;
            if(_score > 14) {
                EndGame();

                return;
            }

            _scoreText.SetText($"{(int)_score}/10");

            IncreaseDifficulty();
        }

        private void IncreaseDifficulty() {
            Debug.Log("Increasing speed");

            _moveSpeed += 0.5f;
            foreach (var fallingObject in FindObjectsOfType<FallingObjectScript>()) {
                fallingObject.MoveSpeed = _moveSpeed;
            }
        }
    }
}
