using System;
using Assets.Scripts.Cognity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace Assets.Scripts.GlobalScripts.UITask {
    /// <summary>
    ///     Handles specific button events.
    /// </summary>
    public class ActionManager : MonoBehaviour {
        private void Start() {

            //PlayerPrefs.DeleteAll();

            // Not first time use and avoid null exception
            if (PlayerPrefs.GetString("user_info") == string.Empty || SceneManager.GetActiveScene().buildIndex != 0) {
                return;
            }

            Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name.Equals("panel home"))
                .Panel
                .transform
                .gameObject
                .SetActive(true);

            Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name.Equals("panel userinfo"))
                .Panel
                .transform
                .gameObject
                .SetActive(false);
        }

        public void SaveUserPref(TMP_InputField input) {
            string userInfo = input.text;
            if (input.text == string.Empty) {
                Random rand = new Random();
                userInfo = "user" + rand.NextDouble();
            }

            // Cache user name
            PlayerPrefs.SetString("user_info", userInfo);
        }

        public void GoTo(string sceneName) {
            // Avoid per game category audio duplication(not stopping)
            if (sceneName == "BaseMenu") {
                Destroy(GameObject.Find("AudioManager").gameObject);
            }

            SceneManager.LoadScene(sceneName);
        }

        public void GoToBaseMenu() {
            SceneManager.LoadScene("BaseMenu");
        }

        public void Quit() {
            Application.Quit();
        }

        public void Show(Transform transform) {
            transform.gameObject.SetActive(true);
        }

        public void Hide(Transform transform) {
            transform.gameObject.SetActive(false);
        }

        /// <summary>
        ///     The transition after username input. Shall never be used with other animators unless it has the
        ///     same behaviour
        /// </summary>
        public void InvokeAnimation(Animator animator) {
            animator.SetTrigger("show");
        }

        public void MuteBackground() {
            bool mute = Array.Find(FindObjectOfType<AudioManager>().AudioCollections, s => s.Name == "background")
                .AudioSource.mute;
            Array.Find(FindObjectOfType<AudioManager>().AudioCollections, s => s.Name == "background").AudioSource.mute =
                !mute;

            if (mute) {
                Transform button = Array.Find(FindObjectOfType<UIManager>().ButtonCollection, i => i.Name == "music").Button;
                button.GetChild(0).gameObject.SetActive(false);
                button.GetChild(1).gameObject.SetActive(true);
            } else {
                Transform button = Array.Find(FindObjectOfType<UIManager>().ButtonCollection, i => i.Name == "music").Button;
                button.GetChild(0).gameObject.SetActive(true);
                button.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}
