using System.Collections;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Catching {
    public class GameManager : CoreGameBehaviour {
        [SerializeField] private GameObject _fallingObject;

        private BaseScoreHandler _baseScoreHandler;
        private TimerManager _timerManager;
        private Vector2 _screenBounds;
        private float _moveSpeed;
        private float _spawnRate;
        private float _score;

        private void Start() {
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

            SceneManager.LoadScene(GetNextScene());
        }

        public void IncreasePoint() {
            _score++;
            if(_score > 10) {
                EndGame();
            }
        }
    }
}
