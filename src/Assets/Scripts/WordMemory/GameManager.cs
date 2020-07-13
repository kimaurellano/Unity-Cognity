using System.Collections;
using System.Linq;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.WordMemory {
    [RequireComponent(typeof(ActionManager))]
    public class GameManager : CoreGameBehaviour {
        [SerializeField] private string[] _listOfWords;
        [SerializeField] private TextMeshProUGUI _word;

        private Randomizer<string> _randomizer;
        private BaseScoreHandler _baseScoreHandler;
        private TimerManager _timerManager;
        private UIManager _uiManager;
        private TextMeshProUGUI _mistakeText;
        private int _mistake;
        private int _displayedWords;

        private void Start() {
            _randomizer = new Randomizer<string>();

            _baseScoreHandler = new BaseScoreHandler(0, 15);

            _timerManager = GetComponent<TimerManager>();
            _uiManager = FindObjectOfType<UIManager>();

            SceneManager.activeSceneChanged += RemoveEvents;
            TimerManager.OnPreGameTimerEndEvent += StartGame;
            TimerManager.OnGameTimerEndEvent += EndGame;

            _mistakeText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "mistake");
             _mistakeText.SetText($"Mistakes: {_mistake}/3");
        }

        private void RemoveEvents(Scene current, Scene next) {
            TimerManager.OnGameTimerEndEvent -= DisplayWord;
        }

        private void StartGame() {
            TimerManager.OnPreGameTimerEndEvent -= StartGame;

            _timerManager.StartTimerAt(0, 10f);

            for (int i = 0; i < 15; i++) {
                _randomizer.AddToList(_listOfWords[Random.Range(0, _listOfWords.Length)]);
            }

            DisplayWord();

            StartCoroutine(ChangeWord());
        }

        private void DisplayWord() {
            ++_displayedWords;
            if (_displayedWords > 16) {
                EndGame();

                return;
            }
            
            _word.SetText(_randomizer.GetRandomItem());
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
            string lastItem = _randomizer.GetItem(_randomizer.Index - 1);
            string currentItem = _randomizer.GetCurrentItem();
            string answer = lastItem == currentItem ? "true" : "false";

            if (userAnswer.Equals(answer)) {
                _timerManager.StartTimerAt(0, 10f);

                _baseScoreHandler.AddScore(1);
            } else {
                _mistake++;
                if (_mistake > 3) {
                    EndGame();

                    return;
                }

                _mistakeText.SetText($"Mistakes: {_mistake}/3");
            }

            StartCoroutine(ChangeWord());
        }

        private IEnumerator ChangeWord() {
            if (_displayedWords == 0) {
                ((Transform) _uiManager.GetUI(UIManager.UIType.Button, "button true"))
                    .GetComponent<Button>().interactable = false;
                ((Transform)_uiManager.GetUI(UIManager.UIType.Button, "button false"))
                    .GetComponent<Button>().interactable = false;
                yield return new WaitForSeconds(2.5f);
                ((Transform)_uiManager.GetUI(UIManager.UIType.Button, "button true"))
                    .GetComponent<Button>().interactable = true;
                ((Transform)_uiManager.GetUI(UIManager.UIType.Button, "button false"))
                    .GetComponent<Button>().interactable = true;
            }

            _word.GetComponent<Animator>().Play("ChangeWord");

            yield return new WaitForSeconds(1.5f);

            _word.GetComponent<Animator>().Play("Idle");

            DisplayWord();
        }
    }
}
