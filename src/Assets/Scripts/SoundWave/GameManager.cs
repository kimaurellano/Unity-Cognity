using System;
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

namespace Assets.Scripts.SoundWave {
    [RequireComponent(typeof(ActionManager))]
    public class GameManager : CoreGameBehaviour {
        [Serializable]
        private class Sound {
            public string Name;

            public Transform Button;

            public AudioClip AudioClip;
        }

        [SerializeField] private Sound[] _sound;
        [SerializeField] private Image _resultImage;
        [SerializeField] private Sprite _correct;
        [SerializeField] private Sprite _wrong;
        [SerializeField] private Button _buttonHint;

        private BaseScoreHandler _baseScoreHandler;
        private UIManager _uiManager;
        private List<Sound> _seqOfClips;
        private List<int> _soundIdx;
        private int _selectedIdx;
        private int _repetition;
        private int _hintCount = 1;
        private bool _hasError;

        private void Start() {
            // Prevent animation event to proceed
            if (gameObject.name != "GameManager") {
                return;
            }

            _soundIdx = new List<int>();
            _seqOfClips = new List<Sound>();

            _uiManager = FindObjectOfType<UIManager>();

            _baseScoreHandler = new BaseScoreHandler(0, 10);

            // Disable result mark image first
            _resultImage.enabled = false;

            ((Animator) _uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "playing"))
                .enabled = true;

            ((Animator) _uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "sequence result"))
                .enabled = false;

            TimerManager.OnPreGameTimerEndEvent += StartGame;
        }

        private void StartGame() {
            TimerManager.OnPreGameTimerEndEvent -= StartGame;

            // Add AudioSource components
            foreach (var sound in _sound) {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.clip = sound.AudioClip;
                src.playOnAwake = false;
                src.loop = false;
            }

            SetEnableInstrument(false);

            RandomPopulate();
        }

        private IEnumerator PleaseWait() {
            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "now playing"))
                .SetText("Please wait...");
            yield return new WaitForSeconds(2f);
        }

        private IEnumerator PlaySequence() {
            _buttonHint.interactable = false;

            SetEnableInstrument(false);

            yield return StartCoroutine(PleaseWait());

            foreach (var clip in _seqOfClips) {
                ((Animator)_uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "playing"))
                    .enabled = true;
                ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "now playing"))
                    .SetText("Playing sequence");

                Transform indicator = clip.Button.transform.Find("Indicator");
                indicator.GetComponent<Animator>().enabled = true;
                indicator.transform.gameObject.SetActive(true);

                PlayClip(clip.AudioClip.name.ToLower());

                yield return StartCoroutine(SamplePlay(clip.AudioClip.length));

                // Stop indicating
                indicator.GetComponent<Animator>().enabled = false;
                indicator.transform.gameObject.SetActive(false);

                // Disable button while sample playing sounds
                foreach (var item in _sound) {
                    Button btn = item.Button.GetComponent<Button>();
                    btn.transform.Find("Indicator").GetComponent<Animator>().enabled = false;
                    btn.transform.Find("Indicator").gameObject.SetActive(false);
                }
            }

            _buttonHint.interactable = _hintCount > 0;

            // Enable all the button
            SetEnableInstrument(true);

            // Stop playing icon animation
            ((Animator)_uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "playing"))
                .enabled = false;
            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "now playing"))
                .SetText("Choose correct order");

            yield return null;
        }

        private void RandomPopulate() {
            _soundIdx.Clear();
            _seqOfClips.Clear();

            for (int i = 0; i < _sound.Length; i++) {
                _soundIdx.Add(Random.Range(0, _sound.Length));

                Sound sound = _sound[_soundIdx[i]];

                _seqOfClips.Add(sound);
            }
        }

        private void SetEnableInstrument(bool value) {
            // Enable all the button
            foreach (var item in _sound) {
                item.Button.GetComponent<Button>().enabled = value;
            }
        }

        public void PlaySeq() {
            _hintCount--;

            _buttonHint.interactable = _hintCount > 0;

            StartCoroutine(PlaySequence());
        }

        public void PlayButton(Transform button) {
            // Get the corresponding clip of a button
            string clipName = Array.Find(_sound, s => s.Button.transform.name == button.name).Name;

            PlayClip(clipName);

            if (clipName != _seqOfClips[_selectedIdx].Name) {
                _hasError = true;
            }

            Progress();
        }

        private void Progress() {
            _selectedIdx++;

            if (_selectedIdx > _seqOfClips.Count - 1) {
                // Just add point every complete sequence
                if (!_hasError) {
                    // Add points
                    _baseScoreHandler.AddScore(1);
                }

                _selectedIdx = 0;
                _repetition++;

                if (_repetition > 10) {
                    _baseScoreHandler.SaveScore(UserStat.GameCategory.Memory);

                    ShowGraph(
                        UserStat.GameCategory.Memory,
                        _baseScoreHandler.Score,
                        _baseScoreHandler.ScoreLimit);

                    base.EndGame();

                    return;
                }

                // Show result
                StartCoroutine(SequenceNotif(_hasError));

                // Ready new set of sequence
                RandomPopulate();

                // Disable while waiting
                SetEnableInstrument(false);

                StartCoroutine(PlaySequence());
            }
        }

        private void DisableInstrument() {
            // Disable button while sample playing sounds
            foreach (var item in _sound) {
                Button btn = item.Button.GetComponent<Button>();
                btn.enabled = false;
                btn.transform.Find("Indicator").GetComponent<Animator>().enabled = false;
                btn.transform.Find("Indicator").gameObject.SetActive(false);
            }
        }

        private IEnumerator SequenceNotif(bool result) {
            FindObjectOfType<AnimationEvent>().SetError(_hasError);

            Animation anim = (Animation) _uiManager.GetUI(UIManager.UIType.AnimatedSingleState, "sequence result");
            // Set check or x image
            _resultImage.enabled = true;
            _resultImage.sprite = result ? _wrong : _correct;
            anim.Play();

            yield return new WaitForSeconds(anim.clip.length);

            // Stop animation and clear image
            anim.Stop();
            _resultImage.enabled = false;
            _resultImage.enabled = false;
            _hasError = !_hasError;
        }

        private IEnumerator SamplePlay(float seconds) {
            yield return new WaitForSeconds(seconds + 3f);
        }

        private AudioSource[] GetSoundAudioSources() {
            return GetComponents<AudioSource>();
        }

        private string GetAudioClipName(string name) {
            string clipName = string.Empty;
            foreach (var item in _sound.Where(i => i.Name.Equals(name))) {
                clipName = item.AudioClip.name;
            }

            return clipName;
        }

        private void PlayClip(string name) {
            string clipName = GetAudioClipName(name);
            foreach (var item in GetSoundAudioSources()) {
                if (item.clip.name.Equals(clipName)) {
                    item.Play();
                }
            }
        }
    }
}
