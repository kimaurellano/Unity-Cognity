using Assets.Scripts.GlobalScripts.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.PopNumber {
    public class GameManager : CoreGameBehaviour {

        [Header("List of questions")]
        [SerializeField] private QuestionBank[] _questionBank;
        [Space]
        [SerializeField] private GameObject _numberPrefab;
        [SerializeField] private float _spawnPositionMinX;
        [SerializeField] private float _spawnPositionMaxX;
        [SerializeField] private float _spawnPositionY;
        [SerializeField] private TextMeshProUGUI _problemText;

        private List<QuestionBank> _questionList;
        private TimerManager _timerManager;
        private int _questionIdx = 0;
        private float _quarter = 45f;
        private float _speedIncrease = 1f;
        private float _spawnTime = 1.5f;
        private float _wait;

        private void Start() {
            _questionList = new List<QuestionBank>();
            _timerManager = GetComponent<TimerManager>();
            _timerManager.StartTimerAt(4, 0f);

            NumberScript.OnNumberPopEvent += ProceedToNextQuestion;

            StartCoroutine(StartSpawning());

            _problemText.SetText(_questionList[_questionIdx].Problem);
        }

        private void Update() {
            if (_timerManager.Seconds == _quarter) {
                _wait = 5f;
                _spawnTime -= 0.3f;
                _speedIncrease++;
                _quarter = _timerManager.Seconds - 15f;

                ProceedToNextQuestion();
            }
        }

        private IEnumerator StartSpawning() {
            while (!_timerManager.TimerUp) {
                if (_timerManager.Seconds == 0f) {
                    switch (_timerManager.Minutes) {
                        case 4:
                            RandomAddToList(QuestionBank.Category.Add);
                            break;
                        case 3:
                            RandomAddToList(QuestionBank.Category.Sub);
                            break;
                        case 2:
                            RandomAddToList(QuestionBank.Category.Mult);
                            break;
                        case 1:
                            RandomAddToList(QuestionBank.Category.Div);
                            break;
                    }
                }

                GameObject spawnedPrefab = Instantiate(
                    _numberPrefab,
                    new Vector3(Random.Range(_spawnPositionMinX, _spawnPositionMaxX), _spawnPositionY, 0f),
                    Quaternion.identity);

                int answer = int.Parse(_questionList[_questionIdx].Answer);

                NumberScript script = spawnedPrefab.GetComponent<NumberScript>();
                script.MoveSpeed = _speedIncrease;
                int minRand = answer - 5;
                int maxRand = answer + 5;
                script.Content = Random.Range(minRand, maxRand).ToString();

                yield return new WaitForSeconds(_spawnTime);

                while (_wait > -1) {
                    yield return new WaitForSeconds(1f);
                    _wait--;
                    Debug.Log("Wait time:" + _wait);
                }
            }

            yield break;
        }

        private void ProceedToNextQuestion() {
            if(_questionIdx > _questionList.Count) {
                return;
            }

            _questionIdx++;
            _problemText.SetText(_questionList[_questionIdx].Problem);
        }

        private void RandomAddToList(QuestionBank.Category category) {
            _questionList.Clear();

            foreach (var item in _questionBank.Where(q => q.category == category)) {
                _questionList.Add(item);
            }

            System.Random rng = new System.Random();

            List<QuestionBank> temp = new List<QuestionBank>();
            foreach (var item in _questionList.OrderBy(o => rng.Next())) {
                temp.Add(item);
            }

            _questionList.Clear();
            _questionList = temp;
        }

        public int GetCurrentAnswer() {
            return int.Parse(_questionList[_questionIdx].Answer);
        }

        [System.Serializable]
        private class QuestionBank {
            public enum Category {
                Add,
                Sub,
                Mult,
                Div
            }

            public Category category;

            public string Problem;

            public string Answer;
        }
    }
}