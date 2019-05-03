using System;
using Assets.Scripts.GlobalScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Cognity
{
    public class AudioManager : MonoBehaviour
    {

        private static AudioManager _audioManager;
        private GameObject[] _buttons;
        private bool _isMute = false;

        [SerializeField] private AudioCollection[] _audioCollection;

        public AudioCollection[] AudioCollection { get => _audioCollection; set => _audioCollection = value; }

        private void Start()
        {
            if (_audioManager != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _audioManager = this;
            }

            DontDestroyOnLoad(gameObject);

            foreach (GameObject button in GameObject.FindGameObjectsWithTag("Button"))
            {
                // Ready buttons to listen for clicks
                button.GetComponent<Button>().onClick.AddListener(TaskOnClick);
            }

            foreach (var audio in _audioCollection)
            {
                // We will setup each clip
                audio.AudioSource = gameObject.AddComponent<AudioSource>();
                audio.AudioSource.clip = audio.AudioClip;
                audio.AudioSource.clip.name = audio.AudioClip.name;
                audio.AudioSource.volume = audio.Volume;
            }

            SceneManager.activeSceneChanged += ChangedActiveScene;

            AudioSource background = Array.Find(_audioCollection, s => s.Name == "background").AudioSource;
            background.loop = true;
            background.Play();
        }

        private void TaskOnClick()
        {
            // This is where we assign what clip should be played on button click
            Array.Find(_audioCollection, s => s.Name == "button").AudioSource.Play();
        }

        private void ChangedActiveScene(Scene current, Scene next)
        {
            // We get the buttons for each scene loaded
            foreach (GameObject button in GameObject.FindGameObjectsWithTag("Button"))
            {
                // Ready buttons to listen for clicks
                button.GetComponent<Button>().onClick.AddListener(TaskOnClick);
            }
        }

        public void PlaySnapSfx()
        {
            // This is where we assign what clip should be played on button click
            Array.Find(_audioCollection, s => s.Name == "snap").AudioSource.Play();
        }

        public void StopBackground()
        {
            // This is where we assign what clip should be played on button click
            Array.Find(_audioCollection, s => s.Name == "background").AudioSource.Stop();

            Destroy(this.gameObject);
        }
    }
}
