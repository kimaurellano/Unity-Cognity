using System;
using System.Linq;
using Assets.Scripts.GlobalScripts.Managers;
using Assets.Scripts.GlobalScripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GlobalScripts.Game {
    public class CoreGameBehaviour : MonoBehaviour, ICoreGameBehaviour {

        public delegate void OnPauseGame();
        public delegate void OnMuteGame(string sfx);

        public static event OnPauseGame OnPauseGameEvent;
        public static event OnMuteGame OnMuteGameEvent;

        public bool IsPaused { get; set; }

        public bool IsSFXMuted { get; set; }

        public bool IsBGMuted { get; set; }

        private void Awake() {
            IsSFXMuted = false;

            PlayerPrefs.SetInt("IsMuted", 0);

            IsBGMuted = false;

            PlayerPrefs.SetInt("IsBgMuted", 0);
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

            IsBGMuted = PlayerPrefs.GetInt("IsBgMuted") != 1;

            PlayerPrefs.SetInt("IsBgMuted", IsBGMuted ? 1 : 0);

            audioManager.SetVolume("bg_game", IsBGMuted ? 0f : 1f);
            audioManager.SetVolume("bg_menu", IsBGMuted ? 0f : 1f);

            OnMuteGameEvent?.Invoke("bg");
        }

        public virtual void MuteSFX() {
            AudioCollection audioCollection = FindObjectOfType<AudioCollection>();
            if(audioCollection == null) {
                return;
            }

            IsSFXMuted = PlayerPrefs.GetInt("IsMuted") != 1;

            PlayerPrefs.SetInt("IsMuted", IsSFXMuted ? 1 : 0);

            GameObject audioManager = audioCollection.gameObject;

            AudioSource[] components = audioManager.GetComponents<AudioSource>();
            foreach (var item in audioCollection.audioCollection.Where(audio => audio.Name.StartsWith("sfx"))) {
                foreach (var sfx in components.Where(i => i.clip.name.Equals(item.AudioClip.name))) {
                    sfx.mute = IsSFXMuted;
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
            OnPauseGameEvent = null;
            OnMuteGameEvent = null;

            SceneManager.LoadScene("BaseMenu");
        }
    }
}
