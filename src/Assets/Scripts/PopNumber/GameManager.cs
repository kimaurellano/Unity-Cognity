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

        private Coroutine spawnCoroutine, spawnAnswer;
        private List<QuestionBank> _questionList;
        private PreScoreManager _preScoreManager;
        private TimerManager _timerManager;
        private int _catIdx = 0;
        private int _questionIdx = 0;
        private float _speedIncrease = 1f;
        private float _spawnTime = 1.5f;
        private float _score;
        private bool _proceeded;

        private void Start() {
            _preScoreManager = new PreScoreManager();
            _questionList = new List<QuestionBank>();
            _timerManager = GetComponent<TimerManager>();

            // Check the popped number
            NumberScript.OnNumberPopEvent += CheckNumber;
            // When the number hits the bottom
            NumberScript.OnBottomHitEvent += CheckAndDestroy;

            TimerManager.OnGameTimerEndEvent += StopSpawning;

            // Ready questions
            CategoryAddToList((QuestionBank.Category)_catIdx);
            // Dsiplay first question
            _problemText.SetText(_questionList[_questionIdx].Problem);
            // Start timer
            _timerManager.StartTimerAt(0, 20f);

            spawnCoroutine = StartCoroutine(TimeCheck());

            StartCoroutine(SpawnAnswer());
        }

        private void Update() {
            // When proceeded to next question
            if (_proceeded) {
                Debug.Log("Proceeded");
                _proceeded = false;
                _timerManager.StopTimer();
                _timerManager.StartTimerAt(0, 20f);
            }

            if (_timerManager.TimerUp) {
                _timerManager.StartTimerAt(0, 20f);
            }
        }

        private void IncreaseDifficulty() {
            _spawnTime -= 0.2f;
            if (_spawnTime < 0) {
                _spawnTime = 0.03f;
            }
            _speedIncrease++;

            // Prevents different number prefab move speed
            OverrideActiveNumberSpeed(_speedIncrease);
        }

        private void StopSpawning() {
            StopCoroutine(spawnCoroutine);

            ProceedToNextQuestion();
        }

        // Spawn the answer
        private IEnumerator SpawnAnswer() {
            while (true) {
                if (_timerManager.Seconds == 15f) {
                    GameObject spawnedPrefab = Instantiate(
                    _numberPrefab,
                    new Vector3(Random.Range(_spawnPositionMinX, _spawnPositionMaxX), _spawnPositionY, 0f),
                    Quaternion.identity);

                    int answer = int.Parse(_questionList[_questionIdx].Answer);

                    NumberScript script = spawnedPrefab.GetComponent<NumberScript>();
                    script.MoveSpeed = _speedIncrease;
                    script.Content = answer.ToString();

                    Debug.Log("Answer spawned:" + answer.ToString());

                    // Prevent spawning multiple time at n second
                    yield return new WaitForSeconds(1);
                }

                yield return null;
            }
        }

        // Spawn random answers and resets time
        private IEnumerator TimeCheck() {
            while (true) {
                GameObject spawnedPrefab = Instantiate(
                    _numberPrefab,
                    new Vector3(Random.Range(_spawnPositionMinX, _spawnPositionMaxX), _spawnPositionY, 0f),
                    Quaternion.identity);

                int answer = int.Parse(_questionList[_questionIdx].Answer);

                NumberScript script = spawnedPrefab.GetComponent<NumberScript>();
                script.MoveSpeed = _speedIncrease;
                script.Content = Random.Range(answer - 5, answer + 5).ToString();

                yield return new WaitForSeconds(_spawnTime);
            }
        }

        // Prevents different number prefab move speed
        private void OverrideActiveNumberSpeed(float speed) {
            foreach (var item in FindObjectsOfType<NumberScript>()) {
                item.MoveSpeed = speed;
            }
        }

        // When the number hits the bottom
        private void CheckAndDestroy(int number) {
            // Once the correct number hits the bottom. Proceed.
            if (number == int.Parse(_questionList[_questionIdx].Answer)) {
                foreach (var item in FindObjectsOfType<NumberScript>()) {
                    Destroy(item.gameObject);
                }

                ProceedToNextQuestion();
            }
        }

        // Check the popped number
        private void CheckNumber(int number) {
            if (number == int.Parse(_questionList[_questionIdx].Answer)) {
                _score += 10;
            } else {
                _score -= 10;
                if (_score < 0) {
                    _score = 0;
                }
            }

            _preScoreManager.AddScore(_score);

            ProceedToNextQuestion();
        }

        private void ProceedToNextQuestion() {
            _proceeded = true;

            // Proceeds to next category
            if (_questionIdx > _questionList.Count) {
                _catIdx++;

                // We only have 4 categories
                if (_catIdx > 4) {
                    EndGame();
                }

                CategoryAddToList((QuestionBank.Category)_catIdx);

                _questionIdx = 0;

                return;
            }

            // For every question
            IncreaseDifficulty();

            // Display next question
            _questionIdx++;
            _problemText.SetText(_questionList[_questionIdx].Problem);

            // Start spawning the possible answers
            StartCoroutine(TimeCheck());
        }

        private void CategoryAddToList(QuestionBank.Category category) {
            _questionList.Clear();

            foreach (var item in _questionBank.Where(q => q.category == category)) {
                _questionList.Add(item);
            }

            System.Random rng = new System.Random();

            // Randomize the question
            List<QuestionBank> temp = new List<QuestionBank>();
            foreach (var item in _questionList.OrderBy(o => rng.Next())) {
                temp.Add(item);
            }

            _questionList.Clear();
            _questionList = temp;
        }

        public override void EndGame() {
            SaveScore(_preScoreManager.TotalScore, GlobalScripts.Player.BaseScoreHandler.GameType.Flexibility);

            base.EndGame();
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