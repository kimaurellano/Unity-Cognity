using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AudioCollection.Audio;

namespace Assets.Scripts.GlobalScripts.Managers {
    public class AudioManager : MonoBehaviour {

        private static AudioManager _audioManager;

        private AudioCollection _audioCollection;

        private void Start() {
            // Singleton
            if (_audioManager != null) {
                Destroy(gameObject);
            } else {
                _audioManager = this;
            }

            DontDestroyOnLoad(gameObject);

            _audioCollection = GetComponent<AudioCollection>();

            InitSounds();

            AttachButtonSFX();

            SceneManager.activeSceneChanged += onSceneChanged;
        }

        private void ButtonClick() {
            InitSound(UseType.Interactable);
        }

        public void PlayPairedSfx() {
        }

        public void StopBackground() {
            Destroy(gameObject);
        }

        public bool MuteBackground() {
            return false;
        }

        public void onSceneChanged(Scene current, Scene next) {
            AttachButtonSFX();
        }

        private void AttachButtonSFX() {
            foreach (var button in GameObject.FindGameObjectsWithTag("Button")) {
                // Ready buttons to listen for clicks
                button.GetComponent<Button>().onClick.AddListener(ButtonClick);
            }
        }

        private void InitSounds() {
            foreach (var item in _audioCollection.audioCollection) {
                item.AudioSource = gameObject.AddComponent<AudioSource>();
                item.AudioSource.playOnAwake = false;
                item.AudioSource.clip = item.AudioClip;
                if (FindObjectOfType<GameSceneManager>().CurrentSceneType == item.useType) {
                    item.AudioSource.Play();
                }
            }
        }

        private void InitSound(UseType useType) {
            foreach (var item in _audioCollection.audioCollection.Where(i => i.useType == useType)) {
                item.AudioSource.Play();
            }
        }
    }
}
