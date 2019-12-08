using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Listening {
    [RequireComponent(typeof(ActionManager))]
    public class GameManager : CoreGameBehaviour {
        [SerializeField] private AudioClip[] _words;
        [SerializeField] private GameObject _nowPlayingText;
        [SerializeField] private TMP_InputField _answerField;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _hintCountText;
        [SerializeField] private Button _hintButton;

        private AudioSource _src;
        private TimerManager _timerManager;
        private BaseScoreHandler _baseScoreHandler;
        private List<int> _keys;

        private int _hint = 2;
        private int _useKey;
        private int _currentClip;
        private int _randomKey;

        private void Start() {
            _src = gameObject.AddComponent<AudioSource>();

            _timerManager = gameObject.AddComponent<TimerManager>();

            _baseScoreHandler = new BaseScoreHandler(0, _words.Length);

            _keys = new List<int>();
            for (var i = 0; i < _words.Length; i++) {
                _keys.Add(i);
            }

            _scoreText.SetText($"Words: {_baseScoreHandler.Score}/{_words.Length}");

            TimerManager.OnPreGameTimerEndEvent += StartGame;
        }

        private void StartGame() {
            TimerManager.OnPreGameTimerEndEvent -= StartGame;

            _timerManager.StartTimerAt(0, 30f);

            PrepareWord();
        }

        private AudioSource[] GetAttachedAudioComponents() {
            return GetComponents<AudioSource>();
        }

        public void Submit() {
            if (_src.clip.name == _answerField.text) {
                GetAttachedAudioComponents().FirstOrDefault(i => i.clip.name == "CorrectSFX")?.Play();

                _baseScoreHandler.AddScore(1);

                _scoreText.SetText($"Words: {_baseScoreHandler.Score}/{_words.Length}");

                _timerManager.Seconds += 5f;
            } else {
                GetAttachedAudioComponents().FirstOrDefault(i => i.clip.name == "IncorrectSFX")?.Play();
            }

            _answerField.GetComponent<TMP_InputField>().text = string.Empty;

            PrepareWord();
        }

        public void PrepareWord() {
            _currentClip++;

            if (_currentClip > _words.Length - 1) {
                EndGame();

                return;
            }

            // Generate random spawn
            _randomKey = Random.Range(0, _keys.Count);

            // Set random spawn point
            _useKey = _keys.ElementAt(_randomKey);

            _src.clip = _words[_useKey];

            // Avoid using the same spawn point
            _keys.RemoveAt(_randomKey);

            StartCoroutine(PlayWord(_src.clip.name));

            ResetHint();
        }

        public void ResetHint() {
            // Reset hint per word
            _hint = 2;

            _hintButton.interactable = true;

            _hintCountText.SetText($"x{_hint}");
        }

        public void DecreaseHint() {
            _hint--;
            _hintCountText.SetText($"x{_hint}");
        }

        public void PlayWord() {
            StartCoroutine(PlayWord(_src.clip.name));
        }

        private IEnumerator PlayWord(string word) {
            _nowPlayingText.GetComponent<TextMeshProUGUI>().SetText("Now playing");

            Animation nowPlayingAnimation = _nowPlayingText.GetComponent<Animation>();
            nowPlayingAnimation.Play();

            _hintButton.interactable = false;

            yield return new WaitForSeconds(3f);

            _hintButton.interactable = _hint > 0;

            foreach (var src in GetAttachedAudioComponents().Where(i => i.clip.name == word)) {
                src.Play();
            }

            yield return new WaitForSeconds(1.5f);

            nowPlayingAnimation.Stop();
            _nowPlayingText.GetComponent<TextMeshProUGUI>().SetText(string.Empty);
        }

        public override void EndGame() {
            _baseScoreHandler.SaveScore(UserStat.GameCategory.Language);

            ShowGraph(
                UserStat.GameCategory.Language,
                _baseScoreHandler.Score,
                _baseScoreHandler.ScoreLimit);

            base.EndGame();
        }
    }
}
