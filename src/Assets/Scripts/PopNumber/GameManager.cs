﻿using Assets.Scripts.GlobalScripts.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Assets.Scripts.GlobalScripts.Game;
using TMPro;
using UnityEngine;
// ReSharper disable All

namespace Assets.Scripts.PopNumber {
    public class GameManager : CoreGameBehaviour {

        [Header("List of questions")]
        [SerializeField] private QuestionBank[] _questionBank;
        [Space]
        [SerializeField] private GameObject _numberPrefab;
        [SerializeField] private float _spawnRate;
        [SerializeField] private TextMeshProUGUI _problemText;

        private Vector2 _screenBounds;
        private UIManager _uiManager;
        private Coroutine _spawnCoroutine, _spawnAnswer;
        private List<QuestionBank> _questionList;
        private PreScoreManager _preScoreManager;
        private TimerManager _timerManager;
        private int _catIdx;
        private int _correctCount;
        private int _wrongCount;
        private int _questionIdx;
        private float _speed = 0.5f;
        private float _score;

        private void Start() {
            _uiManager = FindObjectOfType<UIManager>();
            _preScoreManager = new PreScoreManager();
            _questionList = new List<QuestionBank>();
            _timerManager = GetComponent<TimerManager>();

            _screenBounds =
                Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                    Camera.main.transform.position.z));

            // Check the popped number
            NumberScript.OnNumberPopEvent += CheckNumber;
            // When the number hits the bottom
            NumberScript.OnBottomHitEvent += CheckAndDestroy;

            TimerManager.OnGameTimerEndEvent += IncreaseDifficulty;

            // Ready questions
            CategoryAddToList((QuestionBank.Category)_catIdx);
            // Dsiplay first question
            _problemText.SetText(_questionList[_questionIdx].Problem);
            // Start timer
            _timerManager.StartTimerAt(0, 10f);

            _spawnCoroutine = StartCoroutine(RandomSpawn());

            StartCoroutine(SpawnAnswer());
        }

        private void StopSpawning() {
            StopCoroutine(_spawnCoroutine);

            Debug.Log("Spawn coroutine stopped");
        }

        // Spawn the answer
        private IEnumerator SpawnAnswer() {
            Debug.Log("Random spawn answer started");
            while (true) {
                if (Mathf.RoundToInt(_timerManager.Seconds) == 5 ||
                    Mathf.RoundToInt(_timerManager.Seconds) == 10 ||
                    Mathf.RoundToInt(_timerManager.Seconds) == 15) {
                    GameObject spawnedPrefab = Instantiate(
                        _numberPrefab,
                        new Vector3(Random.Range(-_screenBounds.x + 0.5f, _screenBounds.x - 0.5f),
                            _screenBounds.y + 0.5f, 0f),
                        Quaternion.identity);

                    int answer = int.Parse(_questionList[_questionIdx].Answer);

                    NumberScript script = spawnedPrefab.GetComponent<NumberScript>();
                    script.MoveSpeed = _speed;
                    script.Content = answer.ToString();

                    Debug.Log("Answer spawned:" + answer);

                    // Prevent spawning multiple time at n second
                    yield return new WaitForSeconds(1);
                }

                yield return null;
            }
        }

        // Spawn random answers and resets time
        private IEnumerator RandomSpawn() {
            Debug.Log("Random spawn started");
            while (true) {
                GameObject spawnedPrefab = Instantiate(
                    _numberPrefab,
                    new Vector3(Random.Range(-_screenBounds.x + 0.5f, _screenBounds.x - 0.5f), _screenBounds.y + 0.5f, 0f),
                    Quaternion.identity);

                int answer = int.Parse(_questionList[_questionIdx].Answer);

                NumberScript script = spawnedPrefab.GetComponent<NumberScript>();
                script.MoveSpeed = _speed;
                script.Content = Random.Range(answer - 5, answer + 5).ToString();

                yield return new WaitForSeconds(_spawnRate);
            }
        }

        private void IncreaseDifficulty() {
            Debug.Log("<color=red>difficulty increased</color>");

            Animator anim = (Animator)_uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "speed increase");
            anim.Play("SpeedIncrease");

            _spawnRate -= 0.1f;
            if (_spawnRate < 0) {
                _spawnRate = 0.1f;
            }

            _speed += .3f;
            if (_speed > 2f) {
                _speed = 2f;
            }

            _timerManager.ResetTimer();

            foreach (var item in FindObjectsOfType<NumberScript>()) {
                item.MoveSpeed = _speed;
            }
        }

        // When the number hits the bottom
        private void CheckAndDestroy(int number) {
            // Once the correct number hits the bottom. Proceed.
            if (number == int.Parse(_questionList[_questionIdx].Answer)) {
                ProceedToNextQuestion();
            }
        }

        // Check the popped number
        private void CheckNumber(int number) {
            if (number == int.Parse(_questionList[_questionIdx].Answer)) {
                _timerManager.ResetTimer();

                _correctCount++;
                if (_correctCount == 5) {
                    _correctCount = 0;
                    IncreaseDifficulty();
                    Debug.Log("<color=red>Increasing difficulty</color>");
                }

                _score += 10;
            } else {
                _wrongCount++;
                if(_wrongCount == 3) {
                    EndGame();
                }

                _score -= 10;
                if (_score < 0) {
                    _score = 0;
                }
            }

            _preScoreManager.AddScore(_score);

            ProceedToNextQuestion();
        }

        private void DestroyNumbers() {
            foreach (var item in FindObjectsOfType<NumberScript>()) {
                Destroy(item.gameObject);
            }
        }

        private void ProceedToNextQuestion() {
            StopSpawning();

            // Ready next question
            _questionIdx++;

            // Proceeds to next category
            if (_questionIdx > _questionList.Count - 1) {
                _catIdx++;

                // We only have 4 categories
                if (_catIdx > 4) {
                    EndGame();
                }

                CategoryAddToList((QuestionBank.Category)_catIdx);

                _questionIdx = 0;

                _problemText.SetText(_questionList[_questionIdx].Problem);
            } else {
                _problemText.SetText(_questionList[_questionIdx].Problem);
            }

            // Start spawning the possible answers
            _spawnCoroutine = StartCoroutine(RandomSpawn());

            Debug.Log("Proceeding to next question...");
        }

        private void CategoryAddToList(QuestionBank.Category category) {
            _questionList.Clear();

            foreach (var item in _questionBank.Where(q => q.category == category)) {
                _questionList.Add(item);
            }

            System.Random rng = new System.Random();

            // Randomize the order of the questions
            List<QuestionBank> temp = _questionList.OrderBy(o => rng.Next()).ToList();

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