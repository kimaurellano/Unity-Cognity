using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.GlobalScripts.Managers {
    public class AudioManager : MonoBehaviour {

        public delegate void OnAllAudioOverride();

        public delegate void OnAudioPlay();

        public static OnAudioPlay onAudioEndPlayEvent;

        public static OnAllAudioOverride OnAllAudioOverrideEvent; 
        private static AudioManager _audioManager;

        private AudioCollection _audioCollection;

        private void Awake() {
            // Must only be placed on Awake to be invoke once only

            Debug.Log("AudioManager: started");

            // Singleton
            if (_audioManager != null) {
                Destroy(gameObject);
            } else {
                _audioManager = this;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            _audioCollection = GetComponent<AudioCollection>();

            InitSounds();

            AttachButtonSfx();

            // Reattach button sfx every scene change
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void ButtonClick() {
            PlayClip("sfx_button");
        }

        public void OnSceneChanged(Scene current, Scene next) {
            // Prevent audio manipulation during Sound games.
            // Informs PausePanel and CoreGameBehaviour that the
            // audio cannot be unmuted/change volume unless it's a scene other
            // than sound games
            if (SceneManager.GetActiveScene().name.Equals("GameSoundWave")) {
                SetVolume("bg_game", 0.1f);
                SetVolume("bg_menu", 0.1f);

                OnAllAudioOverrideEvent?.Invoke();

                return;
            }

            OnAllAudioOverrideEvent = null;

            Debug.Log("Scene changed");
            AttachButtonSfx();

            InitSounds();
        }

        private void AttachButtonSfx() {
            // Get all inactive objects
            foreach (var button in (Button[]) Resources.FindObjectsOfTypeAll(typeof(Button))) {
                button.GetComponent<Button>().onClick.AddListener(ButtonClick);
            }

            // Get all active objects
            foreach (var button in (Button[]) FindObjectsOfType(typeof(Button))) {
                button.GetComponent<Button>().onClick.AddListener(ButtonClick);
            }
        }

        private void InitSounds() {
            // Remove attached audio to the current scene
            AudioSource[] attachedComponents = GetAttachedAudioComponents();
            if (attachedComponents != null) {
                foreach (var item in attachedComponents) {
                    Destroy(item);
                }
            } else {
                return;
            }

            // Re-assign proper audio to a scene
            AttachSfxToScene(SceneManager.GetActiveScene().name);
        }

        private void AttachSfxToScene(string scene) {
            foreach (var item in _audioCollection.audioCollection) {
                foreach (var i in item.Games) {
                    if (scene.Contains(i) || scene.Equals(i) || i.Equals("All")) {
                        AudioSource src = gameObject.AddComponent<AudioSource>();
                        src.clip = item.AudioClip;
                        src.volume = item.Volume;
                        src.playOnAwake = item.playOnAwake;
                        src.loop = item.loop;

                        if (item.playOnAwake) {
                            src.Play();
                        }
                    }
                }
            }
        }

        public void PlayClip(string name) {
            string clipName = GetAudioClipName(name);
            foreach (var item in GetAttachedAudioComponents()) {
                if (item.clip.name.Equals(clipName)) {
                    item.Play();
                }
            }
        }

        public void SetVolume(string name, float value) {
            string clipName = GetAudioClipName(name);
            foreach (var item in GetAttachedAudioComponents()) {
                if (item.clip.name.Equals(clipName)) {
                    item.volume = value;
                }
            }
        }

        public string GetAudioClipName(string name) {
            string clipName = string.Empty;
            foreach (var item in _audioCollection.audioCollection.Where(i => i.Name.Equals(name))) {
                clipName = item.AudioClip.name;
            }

            return clipName;
        }

        public AudioSource[] GetAttachedAudioComponents() {
            return GetComponents<AudioSource>();
        }
    }
}
