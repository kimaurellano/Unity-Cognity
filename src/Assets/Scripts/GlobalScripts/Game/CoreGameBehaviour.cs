using System;
using System.Linq;
using Assets.Scripts.GlobalScripts.Managers;
using Assets.Scripts.GlobalScripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GlobalScripts.Game {
    [RequireComponent(typeof(ActionManager))]
    public class CoreGameBehaviour : MonoBehaviour {

        public delegate void OnQuitGame();
        public delegate void OnPauseGame();
        public delegate void OnMuteGame(string sfx);

        public static event OnQuitGame OnQuitGameEvent;
        public static event OnPauseGame OnPauseGameEvent;
        public static event OnMuteGame OnMuteGameEvent;

        public bool IsPaused { get; set; }

        public bool IsSfxMuted { get; set; }

        public bool IsBgMuted { get; set; }

        private void Awake() {
            IsSfxMuted = false;

            PlayerPrefs.SetInt("IsMuted", 0);

            IsBgMuted = false;

            PlayerPrefs.SetInt("IsBgMuted", 0);
        }

        public void Retry() {
            Scene active = SceneManager.GetActiveScene();
            SceneManager.LoadScene(active.name);
        }

        public virtual void EndGame() {
            FindObjectOfType<AudioManager>().SetVolume("bg_game", 0f);
        }

        public virtual void InitSounds() {
            throw new NotImplementedException();
        }

        public virtual void MuteBackgroundMusic() {
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

        public void SaveScore(float score, BaseScoreHandler.GameType gameType) {
            BaseScoreHandler baseScoreHandler = new BaseScoreHandler();
            baseScoreHandler.AddScore(score, gameType);
        }

        public void QuitGame() {
            OnQuitGameEvent?.Invoke();

            OnPauseGameEvent = null;
            OnMuteGameEvent = null;

            SceneManager.LoadScene("BaseMenu");
        }
    }
}
