using Assets.Scripts.GlobalScripts.Managers;
using System.Collections.Generic;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.PickInSequence {
    public class GameManager : CoreGameBehaviour {

        [SerializeField] private GameObject _numberPrefab;

        [SerializeField] private int _setOfSequence;
        [SerializeField] private float _minY, _minX;
        [SerializeField] private float _maxY, _maxX;

        private System.Random _rnd;
        private List<int> _rndSequence;
        private NumberScriptPick _numberScriptPick;
        private BaseScoreHandler _baseScoreHandler;
        private TimerManager _timeManager;
        private int _curElement;
        private int _curSetOfSequence;
        private float _score;
        private bool _restartTimer;

        public float MinY => _minY;
        public float MinX => _minX;
        public float MaxY => _maxY;
        public float MaxX => _maxX;

        private void Start() {
            _timeManager = GetComponent<TimerManager>();

            NumberScriptPick.OnNumberPopPickEvent += CheckAnswer;
            TimerManager.OnGameTimerEndEvent += ProceedToNextSequence;

            InstantiateNumber(RandomSequence(5));

            _timeManager.StartTimerAt(0, 10f);

            _baseScoreHandler = new BaseScoreHandler(0, 100);
        }

        private void Update() {
            if (_restartTimer) {
                _restartTimer = false;
                _timeManager.StartTimerAt(0, 10f);
            }
        }

        private void InstantiateNumber(int[] sequence) {
            foreach (var number in sequence) {
                GameObject numPrefab = Instantiate(
                    _numberPrefab,
                    new Vector3(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY), 0f),
                    Quaternion.identity);

                NumberScriptPick scriptPick = numPrefab.GetComponent<NumberScriptPick>();
                scriptPick.Content = number.ToString();
            }
        }

        private int[] RandomSequence(int length) {
            _rndSequence = new List<int>();

            _rndSequence.Clear();

            _rnd = new System.Random();
            for (int i = 0; i < length; i++) {
                int curRnd = _rnd.Next(100);
                if (_rndSequence.Exists(e => e.Equals(curRnd))) {
                    continue;
                }

                _rndSequence.Add(_rnd.Next(100));
            }
            _rndSequence.Sort();

            Debug.Log("Sequence");
            foreach (var item in _rndSequence) {
                Debug.Log(item);
            }

            return _rndSequence.ToArray();
        }

        private void ProceedToNextSequence() {
            ClearObjects();

            _restartTimer = true;

            _curSetOfSequence++;

            // All set of sequence are answered
            if (_curSetOfSequence > _setOfSequence - 1) {
                _baseScoreHandler.SaveScore(UserStat.GameCategory.Flexibility);

                // Clear event
                NumberScriptPick.OnNumberPopPickEvent -= CheckAnswer;
                TimerManager.OnGameTimerEndEvent -= ProceedToNextSequence;

                EndGame();

                SceneManager.LoadScene(GetNextScene());
            }

            InstantiateNumber(RandomSequence(5));
        }

        public void CheckAnswer(int number, Transform transform) {
            Debug.Log(
                $"Number popped:{number}\n" +
                      $"Current answer:{_rndSequence.ToArray()[_curElement]}\n" +
                      $"Transform:{transform.name}");

            if (number == _rndSequence.ToArray()[_curElement]) {
                Debug.Log("Same!");
                _curElement++;
                Destroy(transform.gameObject);
            }

            // Proceed to next set of sequence
            if(_curElement > _rndSequence.Count - 1) {
                _curElement = 0;
                IncreaseScore();
            }
        }

        private void IncreaseScore() {
            _score += 10;
            _baseScoreHandler.AddScore(_score);

            ProceedToNextSequence();
        }

        private static void ClearObjects() {
            foreach (var item in FindObjectsOfType<NumberScriptPick>()) {
                Destroy(item.gameObject);
            }
        }
    }
}