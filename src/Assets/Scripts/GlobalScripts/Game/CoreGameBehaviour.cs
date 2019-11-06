using System.Linq;
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

        public bool IsPaused { get; set; }

        public bool IsSfxMuted { get; set; }

        public bool IsBgMuted { get; set; }

        private void Awake() {
            IsSfxMuted = false;

            PlayerPrefs.SetInt("IsMuted", 0);

            IsBgMuted = false;

            PlayerPrefs.SetInt("IsBgMuted", 0);

            _gameCollection = FindObjectOfType<GameCollection>();
        }

        public void Retry() {
            Scene active = SceneManager.GetActiveScene();
            SceneManager.LoadScene(active.name);
        }

        public virtual void EndGame() {
            FindObjectOfType<AudioManager>().SetVolume("bg_game", 0f);

            OnEndGameEvent?.Invoke();
        }

        /// <summary>
        /// Returns next scene of a random game in the next category
        /// </summary>
        public string GetNextScene() {
            Utility utility = new Utility();
            // Load persistent data
            StartCoroutine(utility.LoadJson());
            // Fetch the current known loaded scene
            Utility.Data newData = utility.GetData();
            // Proceed to next category
            int currentCatIdx = newData.loaded;
            currentCatIdx++;
            // We only have 4 categories
            if (currentCatIdx > 3) {
                currentCatIdx = 0;
            }
            // Write new values to json file to be read again after game end
            newData.loaded = currentCatIdx;
            utility.ModifyJson(newData);

            // Non zero count
            int gamePerCatCount = _gameCollection.GameCollections[currentCatIdx].Games.Length - 1;
            // Random scene to load per games of a category
            string sceneToLoad = _gameCollection.GameCollections[currentCatIdx].Games[Random.Range(0, gamePerCatCount)];
            // Load
            Debug.Log("Load in sequence -> Loading next category: " + currentCatIdx + " with game:" + sceneToLoad);

            return sceneToLoad;
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

        public void QuitGame() {
            OnQuitGameEvent?.Invoke();

            OnPauseGameEvent = null;
            OnMuteGameEvent = null;

            SceneManager.LoadScene("BaseMenu");
        }
    }
}
