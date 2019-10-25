using Assets.Scripts.GlobalScripts.Managers;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.PopNumber {
    public class GameManager : CoreGameBehaviour {

        [SerializeField] private GameObject _numberPrefab;
        [SerializeField] private float _spawnPositionMinX;
        [SerializeField] private float _spawnPositionMaxX;
        [SerializeField] private float _spawnPositionY;

        private TimerManager _timerManager;
        private float _quarter = 45f;
        private float _speedIncrease = 1f;
        private float _spawnTime = 1.5f;
        private float _wait;

        private void Start() {
            _timerManager = GetComponent<TimerManager>();
            _timerManager.StartTimerAt(1, 0f);

            StartCoroutine(StartSpawning());
        }

        private void Update() {
            if (_timerManager.Seconds == _quarter) {
                _wait = 5f;
                _spawnTime -= 0.3f;
                _speedIncrease++;

                _quarter = _timerManager.Seconds - 15f;
                Debug.Log(_quarter);
            }
        }

        private IEnumerator StartSpawning() {
            while (!_timerManager.TimerUp) {
                GameObject spawnedPrefab = Instantiate(
                    _numberPrefab,
                    new Vector3(Random.Range(_spawnPositionMinX, _spawnPositionMaxX), _spawnPositionY, 0f),
                    Quaternion.identity);

                spawnedPrefab.GetComponent<NumberScript>().MoveSpeed = _speedIncrease;

                yield return new WaitForSeconds(_spawnTime);

                while (_wait > -1) {
                    yield return new WaitForSeconds(1f);
                    _wait--;
                    Debug.Log("Wait time:" + _wait);
                }
            }

            yield break;
        }
    }
}