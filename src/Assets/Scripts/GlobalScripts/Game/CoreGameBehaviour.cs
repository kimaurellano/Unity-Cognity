using System;
using System.Linq;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GlobalScripts.Game {
    [RequireComponent(typeof(ActionManager))]
    public class CoreGameBehaviour : MonoBehaviour {

        public delegate void OnEndGame();
        public delegate void OnQuitGame();
        public delegate void OnPauseGame();
        public delegate void OnMuteGame(string sfx);

        public static event OnEndGame OnEndGameEvent;
        public static event OnQuitGame OnQuitGameEvent;
        public static event OnPauseGame OnPauseGameEvent;
        public static event OnMuteGame OnMuteGameEvent;

        private static GameCollection _gameCollection;
        private Transform _remark;
        private bool _disposed;
        private bool _overrided;

        public bool IsPaused { get; set; }

        public bool IsSfxMuted { get; set; }

        public bool IsBgMuted { get; set; }

        private void Awake() {
            // Remove halted state after every game end
            Time.timeScale = 1f;

            IsSfxMuted = false;

            PlayerPrefs.SetInt("IsMuted", 0);

            IsBgMuted = false;

            PlayerPrefs.SetInt("IsBgMuted", 0);

            _gameCollection = FindObjectOfType<GameCollection>();

            AudioManager.OnAllAudioOverrideEvent += OverrideAudio;
        }

        private void OverrideAudio() {
            _overrided = !_overrided;
        }

        private void ShowGraph() {
            foreach (var remark in (Transform[])Resources.FindObjectsOfTypeAll(typeof(Transform))) {
                if (remark.name.Equals("Remarks")) {
                    // We can now fetch the WindowGraph by enabling it first
                    _remark = remark;
                    _remark.gameObject.SetActive(true);
                    
                    FindObjectOfType<WindowGraph>().ShowGraph();

                    break;
                }
            }
        }

        public void Retry() {
            ClearEvents();

            Scene active = SceneManager.GetActiveScene();
            SceneManager.LoadScene(active.name);
        }

        /// <summary>
        /// Shows graph, clears events, sets time scale to 0
        /// </summary>
        public virtual void EndGame() {
            FindObjectOfType<AudioManager>().SetVolume("bg_game", 0f);

            OnEndGameEvent?.Invoke();

            ClearEvents();

            // Halt all activities
            Time.timeScale = 0f;

            ShowGraph();
        }

        public string GetNextScene() {
            return FindObjectOfType<GameCollection>().GetNextScene();
        }

        public void LoadNextScene() {
            SceneManager.LoadScene(GetNextScene());
        }

        public virtual void MuteBackgroundMusic() {
            if (_overrided) {
                return;
            }

            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if(audioManager == null) {
                return;
            }

            IsBgMuted = PlayerPrefs.GetInt("IsBgMuted") != 1;

            PlayerPrefs.SetInt("IsBgMuted", IsBgMuted ? 1 : 0);

            audioManager.SetVolume("bg_game", IsBgMuted ? 0f : 0.3f);
            audioManager.SetVolume("bg_menu", IsBgMuted ? 0f : 0.3f);

            OnMuteGameEvent?.Invoke("bg");
        }

        public virtual void MuteSfx() {
            if (_overrided) {
                return;
            }

            AudioCollection audioCollection = FindObjectOfType<AudioCollection>();
            if(audioCollection == null) {
                return;
            }

            IsSfxMuted = PlayerPrefs.GetInt("IsMuted") != 1;

            PlayerPrefs.SetInt("IsMuted", IsSfxMuted ? 1 : 0);

            GameObject audioManager = audioCollection.gameObject;

            AudioSource[] components = audioManager.GetComponents<AudioSource>();
            foreach (var item in audioCollection.audioCollection.Where(audio => audio.Name.StartsWith("sfx"))) {
                foreach (var sfx in components.Where(i => i.clip.name.Equals(item.AudioClip.name))) {
                    sfx.mute = IsSfxMuted;
                }
            }

            OnMuteGameEvent?.Invoke("sfx");
        }

        public virtual void Pause() {
            if (PlayerPrefs.GetInt("IsBgMuted") != 1) {
                FindObjectOfType<AudioManager>().SetVolume("bg_game", IsPaused ? 1f : 0.3f);
            }

            IsPaused = !IsPaused;

            Time.timeScale = IsPaused ? 0f : 1f;

            OnPauseGameEvent?.Invoke();
        }

        public void QuitGame() {
            OnQuitGameEvent?.Invoke();

            ClearEvents();

            SceneManager.LoadScene("BaseMenu");
        }

        public void ClearEvents() {
            OnEndGameEvent = null;
            OnQuitGameEvent = null;
            OnPauseGameEvent = null;
            OnMuteGameEvent = null;
        }
    }
}
