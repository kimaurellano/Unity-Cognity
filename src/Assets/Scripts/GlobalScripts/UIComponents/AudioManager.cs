using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GlobalScripts.UIComponents {
    public class AudioManager : MonoBehaviour {
        private static AudioManager _audioManager;

        [field: SerializeField] public AudioCollection[] AudioCollection { get; set; }

        private void Start() {
            // Singleton
            if (_audioManager != null) {
                Destroy(gameObject);
            } else {
                _audioManager = this;
            }

            DontDestroyOnLoad(gameObject);

            foreach (var audio in AudioCollection) {
                // We will setup each clip
                audio.AudioSource = gameObject.AddComponent<AudioSource>();
                audio.AudioSource.clip = audio.AudioClip;
                audio.AudioSource.clip.name = audio.AudioClip.name;
                audio.AudioSource.volume = audio.Volume;
            }

            foreach (var button in GameObject.FindGameObjectsWithTag("Button")) {
                // Ready buttons to listen for clicks
                button.GetComponent<Button>().onClick.AddListener(TaskOnClick);
            }

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
    }
}
