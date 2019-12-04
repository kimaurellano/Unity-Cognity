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

        private BaseScoreHandler _baseScoreHandler;
        private UIManager _uiManager;
        private List<string> _seqOfClips;
        private List<int> _soundIdx;
        private int _selectedIdx;
        private int _repetition;
        private bool _hasError;

        private void Start() {
            // Prevent animation event to proceed
            if (gameObject.name != "GameManager") {
                return;
            }

            _uiManager = FindObjectOfType<UIManager>();

            _baseScoreHandler = new BaseScoreHandler(0, 10);

            // Disable result mark image first
            _resultImage.enabled = false;

            ((Animator) _uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "playing"))
                .enabled = true;

            ((Animator) _uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "sequence result"))
                .enabled = false;

            // Add AudioSource components
            foreach (var sound in _sound) {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.clip = sound.AudioClip;
                src.playOnAwake = false;
                src.loop = false;
            }

            SetEnableInstrument(false);

            StartCoroutine(RandomizeSound());
        }

        private IEnumerator PleaseWait() {
            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "now playing"))
                .SetText("Please wait...");
            yield return new WaitForSeconds(2f);
        }

        private IEnumerator RandomizeSound() {
            SetEnableInstrument(false);

            yield return StartCoroutine(PleaseWait());

            ((Animator)_uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "playing"))
                .enabled = true;
            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "now playing"))
                .SetText("Playing sequence");

            _seqOfClips = new List<string>();
            _seqOfClips.Clear();

            _soundIdx = new List<int>();
            _soundIdx.Clear();

            // Disable button while sample playing sounds
            foreach (var item in _sound) {
                Button btn = item.Button.GetComponent<Button>();
                btn.transform.Find("Indicator").GetComponent<Animator>().enabled = false;
                btn.transform.Find("Indicator").gameObject.SetActive(false);
            }

            Debug.Log("<color=orange>Adding sound... Please wait</color>");
            for (int i = 0; i < _sound.Length; i++) {
                // Randomize sounds
                _soundIdx.Add(Random.Range(0, _sound.Length - 1));
                // Get the clip idx base on the random result
                string clipToPlay = _sound[_soundIdx[i]].Name;

                Debug.Log(clipToPlay);

                Sound sound = _sound[_soundIdx[i]];
                // Show which button is sounding
                Transform indicator = sound.Button.transform.Find("Indicator");
                indicator.GetComponent<Animator>().enabled = true;
                indicator.transform.gameObject.SetActive(true);

                PlayClip(sound.AudioClip.name.ToLower());

                yield return StartCoroutine(SamplePlay(sound.AudioClip.length));

                // Stop indicating
                indicator.GetComponent<Animator>().enabled = false;
                indicator.transform.gameObject.SetActive(false);

                _seqOfClips.Add(clipToPlay);
            }

            Debug.Log("<color=green>Done adding sound</color>");

            // Enable all the button
            SetEnableInstrument(true);

            // Stop playing icon animation
            ((Animator)_uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "playing"))
                .enabled = false;
            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "now playing"))
                .SetText("Choose correct order");

            yield return null;
        }

        private void SetEnableInstrument(bool value) {
            // Enable all the button
            foreach (var item in _sound) {
                item.Button.GetComponent<Button>().enabled = value;
            }
        }

        public void PlayButton(Transform button) {
            // Get the corresponding clip of a button
            string clipName = Array.Find(_sound, s => s.Button.transform.name == button.name).Name;

            PlayClip(clipName);

            if (clipName != _seqOfClips[_selectedIdx]) {
                _hasError = true;
            }

            Progress();
        }

        private void Progress() {
            // Add points
            _baseScoreHandler.AddScore(1);

            _selectedIdx++;

            if (_selectedIdx > _seqOfClips.Count - 1) {
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
                StartCoroutine(RandomizeSound());

                // Disable while waiting
                SetEnableInstrument(false);
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
            // TODO: implement paired button indication
            yield return new WaitForSeconds(seconds);
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
