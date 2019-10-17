using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.GlobalScripts.Managers {
    public class AudioManager : MonoBehaviour {

        public delegate void OnAudioPlay();

        public static OnAudioPlay onAudioEndPlayEvent;

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

            AttachButtonSFX();

            // Reattach button sfx every scene change
            SceneManager.activeSceneChanged += onSceneChanged;
        }

        private void ButtonClick() {
            PlayClip("sfx_button");
        }

        public void onSceneChanged(Scene current, Scene next) {
            Debug.Log("Scene changed");
            AttachButtonSFX();

            InitSounds();

            if(current.name == "GameQuizMath" || current.name == "GameQuizGrammar") {
                transform.gameObject.SetActive(false);
            } else if (current.name == "BaseMenu") {
                transform.gameObject.SetActive(true);
            }
        }

        private void AttachButtonSFX() {
            // Get all active objects
            foreach (var button in Resources.FindObjectsOfTypeAll(typeof(Button)) as Button[]) {
                Debug.Log("Attaching:" + button.name);
                button.GetComponent<Button>().onClick.AddListener(ButtonClick);
            }

            // Get all inactive objects
            foreach (var button in FindObjectsOfType(typeof(Button)) as Button[]) {
                Debug.Log("Attaching:" + button.name);
                button.GetComponent<Button>().onClick.AddListener(ButtonClick);
            }
        }

        private void InitSounds() {
            // Remove attached audio to the current scene
            Component[] attachedComponents = GetAttachedAudioComponents();
            if (attachedComponents != null) {
                foreach (var item in attachedComponents) {
                    Destroy(item);
                }
            } else {
                return;
            }

            // Re-assign proper audio to a scene
            AttachSFXToScene(SceneManager.GetActiveScene().name);
        }

        private void AttachSFXToScene(string scene) {
            foreach (var item in _audioCollection.audioCollection) {
                foreach (var name in item.Games) {
                    if (scene.Contains(name) || scene.Equals(name) || name.Equals("All")) {
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
                if (((AudioSource)item).clip.name.Equals(clipName)) {
                    ((AudioSource)item).Play();
                }
            }
        }

        public void LowerVolume(string name, float value) {
            string clipName = GetAudioClipName(name);
            foreach (var item in GetAttachedAudioComponents()) {
                if (((AudioSource)item).clip.name.Equals(clipName)) {
                    ((AudioSource)item).volume = value;
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

        public Component[] GetAttachedAudioComponents() {
            return GetComponents<AudioSource>() as Component[];
        }
    }
}
