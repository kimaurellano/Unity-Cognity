using System;
using Assets.Scripts.Cognity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GlobalScripts.UITask {
    /// <summary>
    ///     Handles specific button events.
    /// </summary>
    public class ActionManager : MonoBehaviour {

        private void Start() {

            // avoid null exception
            if (SceneManager.GetActiveScene().buildIndex != 0) {
                return;
            }

            // Not first time use
            if (PlayerPrefs.GetString("user_info") != string.Empty) {
                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "panel userinfo")
                    .Panel
                    .gameObject
                    .SetActive(false);

                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "panel welcome")
                    .Panel
                    .gameObject
                    .SetActive(true);

                Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.textName == "label welcome user")
                    .textMesh
                    .SetText(PlayerPrefs.GetString("user_info"));
            }

            if (PlayerPrefs.GetString("game_state") == "set_to_running") {
                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "panel userinfo")
                    .Panel
                    .gameObject
                    .SetActive(false);

                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "panel home")
                    .Panel
                    .gameObject
                    .SetActive(true);
            }
        }

        public void CheckInput(TMP_InputField input) {
            Array.Find(FindObjectOfType<UIManager>().ButtonCollection, i => i.Name == "button save")
                .Button
                .gameObject
                .SetActive(input.text != string.Empty);
        }

        public void SaveUserPref(TMP_InputField input) {
            // Cache user name
            PlayerPrefs.SetString("user_info", input.text);
        }

        public void GoTo(string sceneName) {
            // Signifies that every GoToBaseMenu is preceeded by a game mode
            PlayerPrefs.SetString("game_state", "set_to_running");

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
            PlayerPrefs.SetString("game_state", "set_to_first_run");

            Application.Quit();
        }

        public void Show(Transform transform) {
            if (transform.name == "User_Panel") {
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Hi! " + PlayerPrefs.GetString("user_info"));
            }

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
