using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GlobalScripts.UIComponents {
    public class AudioManager : MonoBehaviour {
        private static AudioManager _audioManager;

        [SerializeField] private AudioCollection[] _audioCollection;

        public AudioCollection[] AudioCollection {
            get => _audioCollection;
            set => _audioCollection = value;
        }

        private void Start() {
            // Singleton
            if (_audioManager != null) {
                Destroy(gameObject);
            } else {
                _audioManager = this;
            }

            DontDestroyOnLoad(gameObject);

            foreach (var collection in AudioCollection) {
                // We will setup each clip
                collection.AudioSource = gameObject.AddComponent<AudioSource>();
                collection.AudioSource.clip = collection.AudioClip;
                collection.AudioSource.clip.name = collection.AudioClip.name;
                collection.AudioSource.volume = collection.Volume;
            }

            SetButtonEffect();

            var background = Array.Find(AudioCollection, s => s.Name == "background").AudioSource;
            background.loop = true;
            background.Play();
        }

        private void TaskOnClick() {
            // This is where we assign what clip should be played on button click
            Array.Find(AudioCollection, s => s.Name == "click").AudioSource.Play();
        }

        public void PlayPairedSfx() {
            // This is where we assign what clip should be played on button click
            Array.Find(AudioCollection, s => s.Name == "paired").AudioSource.Play();
        }

        public void StopBackground() {
            // This is where we assign what clip should be played on button click
            Array.Find(AudioCollection, s => s.Name == "background").AudioSource.Stop();

            Destroy(gameObject);
        }

        public bool MuteBackground() {
            // Get current audio state
            AudioSource backgroundAudio = Array.Find(AudioCollection, i => i.Name == "background").AudioSource;

            // Invert the current state
            return backgroundAudio.mute = !backgroundAudio.mute;
        }

        public void SetButtonEffect() {
            foreach (var button in GameObject.FindGameObjectsWithTag("Button")) {
                // Ready buttons to listen for clicks
                button.GetComponent<Button>().onClick.AddListener(TaskOnClick);
            }
        }
    }
}
