using System.Collections;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets.Scripts.WordMemory {
    [RequireComponent(typeof(ActionManager))]
    public class GameManager : CoreGameBehaviour {
        [SerializeField] private string[] _listOfWords;
        [SerializeField] private TextMeshProUGUI _word;

        private BaseScoreHandler _baseScoreHandler;
        private TimerManager _timerManager;
        private UIManager _uiManager;
        private TextMeshProUGUI _mistakeText;
        private string _temp;
        private string _str;
        private int _mistake;
        private int _displayedWords;

        private void Start() {
            _baseScoreHandler = new BaseScoreHandler(0, 15);

            _timerManager = GetComponent<TimerManager>();
            _uiManager = FindObjectOfType<UIManager>();

            SceneManager.activeSceneChanged += RemoveEvents;
            TimerManager.OnPreGameTimerEndEvent += StartGame;

             _mistakeText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "mistake");
             _mistakeText.SetText($"Mistakes: {_mistake}/3");
        }

        private void RemoveEvents(Scene current, Scene next) {
            TimerManager.OnGameTimerEndEvent -= DisplayWord;
        }

        private void StartGame() {
            TimerManager.OnPreGameTimerEndEvent -= StartGame;

            _timerManager.StartTimerAt(0, 10f);

            DisplayWord();
        }

        private void DisplayWord() {
            _displayedWords++;
            if (_displayedWords > 15) {
                EndGame();

                return;
            }
            
            _str = _listOfWords[Random.Range(0, _listOfWords.Length - 1)];
            _word.SetText(_str);

            Debug.Log($"<color=orange>Current word:{_str}</color>");
        }

        public override void EndGame() {
            _baseScoreHandler.SaveScore(UserStat.GameCategory.Memory);

            ShowGraph(
                UserStat.GameCategory.Memory,
                _baseScoreHandler.Score,
                _baseScoreHandler.ScoreLimit);

            base.EndGame();
        }

        public void Button(string userAnswer) {
            string answer = _str == _temp ? "true" : "false";

            if (userAnswer.Equals(answer)) {
                _timerManager.StartTimerAt(0, 10f);

                TextMeshProUGUI textUI = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "score change");
                textUI.color = Color.green;
                textUI.text = "correct!";

                _baseScoreHandler.AddScore(1);
            } else {
                _mistake++;
                if (_mistake > 3) {
                    TextMeshProUGUI textUI = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "score change");
                    textUI.color = Color.red;
                    textUI.text = "wrong";

                    EndGame();

                    return;
                }

                _mistakeText.SetText($"Mistakes: {_mistake}/3");
            }

            Animation anim = (Animation)_uiManager.GetUI(UIManager.UIType.AnimatedSingleState, "score change");
            anim.Play();

            // Remember the last displayed word
            _temp = _str;
            Debug.Log($"Last word is now:{_temp}");

            StartCoroutine(ChangeWord());
        }

        private IEnumerator ChangeWord() {
            _word.SetText(string.Empty);
            yield return new WaitForSeconds(0.5f);
            DisplayWord();
        }
    }
}
