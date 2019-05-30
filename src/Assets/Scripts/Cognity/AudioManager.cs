using System;
using Assets.Scripts.GlobalScripts.UIComponents;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Cognity {
    public class AudioManager : MonoBehaviour {
        private static AudioManager _audioManager;

        [SerializeField] private AudioCollection[] _audioCollection;

        private GameObject[] _buttons;

        public AudioCollection[] AudioCollections { get => _audioCollection; set => _audioCollection = value; }

        private void Start() {
            if (_audioManager != null) {
                Destroy(gameObject);
            } else {
                _audioManager = this;
            }

            DontDestroyOnLoad(gameObject);

            foreach (var button in GameObject.FindGameObjectsWithTag("Button")) {
                // Ready buttons to listen for clicks
                button.GetComponent<Button>().onClick.AddListener(TaskOnClick);
            }

            foreach (AudioCollection audios in _audioCollection) {
                // We will setup each clip
                audios.AudioSource = gameObject.AddComponent<AudioSource>();
                audios.AudioSource.clip = audios.AudioClip;
                audios.AudioSource.clip.name = audios.AudioClip.name;
                audios.AudioSource.volume = audios.Volume;
            }

            SceneManager.activeSceneChanged += ChangedActiveScene;

            AudioSource background = Array.Find(_audioCollection, s => s.Name == "background").AudioSource;
            background.loop = true;
            background.Play();
        }

        private void TaskOnClick() {
            // This is where we assign what clip should be played on button click
            Array.Find(_audioCollection, s => s.Name == "button").AudioSource.Play();
        }

        private void ChangedActiveScene(Scene current, Scene next) {
            // We get the buttons for each scene loaded
            foreach (var button in GameObject.FindGameObjectsWithTag("Button")) {
                // Ready buttons to listen for clicks
                button.GetComponent<Button>().onClick.AddListener(TaskOnClick);
            }
        }

        public void PlaySnapSfx() {
            // This is where we assign what clip should be played on button click
            Array.Find(_audioCollection, s => s.Name == "snap").AudioSource.Play();
        }

        public void StopBackground() {
            // This is where we assign what clip should be played on button click
            Array.Find(_audioCollection, s => s.Name == "background").AudioSource.Stop();

            Destroy(gameObject);
        }
    }
}
