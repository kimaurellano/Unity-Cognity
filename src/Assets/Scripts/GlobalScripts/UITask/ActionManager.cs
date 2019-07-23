using System;
using System.Collections;
using Assets.Scripts.GlobalScripts.UIComponents;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GlobalScripts.UITask {
    /// <summary>
    ///     Handles specific button events.
    /// </summary>
    public class ActionManager : MonoBehaviour {

        private static Transform _targetPanel;
        private static Transform _currentPanel;

        private void Start() {
            // Make sure games not paused after quitting any game modes
            Time.timeScale = 1f;

            // Avoid null exception
            if (SceneManager.GetActiveScene().buildIndex != 0) {
                return;
            }

            if (SceneManager.GetActiveScene().buildIndex == 0) {
                
            }

            if (PlayerPrefs.GetString("user_info") != string.Empty) {
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
            // Avoid per game category audio duplication(not stopping)
            if (sceneName == "BaseMenu") {
                Destroy(GameObject.Find("AudioManager").gameObject);
            }

            SceneManager.LoadScene(sceneName);
        }

        [Obsolete("Use GoTo(string) function instead")]
        public void GoToBaseMenu() {
            // Avoid per game category audio duplication(not stopping)
            Destroy(GameObject.Find("AudioManager").gameObject);

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

        public void TransitionFrom(Transform currentPanel) {
            _currentPanel = currentPanel;

            // Start panel transition to transition from
            StartCoroutine(BeginTransition(currentPanel));
        }

        public void TransitionTo(Transform targetPanel) {
            if (targetPanel.name == "User_Panel") {
                Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.textName == "label username")
                    .textMesh
                    .SetText(PlayerPrefs.GetString("user_info"));
            }

            // The panel to transition to
            _targetPanel = targetPanel;
        }

        public void SwitchPanel() {
            _currentPanel.gameObject.SetActive(false);
            _targetPanel.gameObject.SetActive(true);
        }

        private static IEnumerator BeginTransition(Transform transform) {
            Animator animator = Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "panel transition")
                .Panel
                .GetComponent<Animator>();

            // Trigger transition
            animator.SetTrigger("transition");

            // Animation has ended
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("PanelTransitionIdle"));

            if (transform.name == "User_Panel") {
                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "panel user")
                    .Panel
                    .gameObject
                    .SetActive(true);

                Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.textName == "label username")
                    .textMesh
                    .SetText(PlayerPrefs.GetString("user_info"));
            }
        }

        public void MuteBackground() {
            Transform button = Array.Find(FindObjectOfType<UIManager>().ButtonCollection, i => i.Name == "volume").Button;

            // Since GameQuizGrammar has different AudioManager namespace
            if (SceneManager.GetActiveScene().name == "GameQuizGrammar" || SceneManager.GetActiveScene().name == "GameQuizMath") {
                AudioSource audioSource = Array
                    .Find(FindObjectOfType<Quiz.Mono.AudioManager>().Sounds, i => i.Name == "GameMusic")
                    .Source;

                audioSource.mute = !audioSource.mute;

                button.GetChild(0).gameObject.SetActive(!button.GetChild(0).gameObject.activeSelf);
                button.GetChild(1).gameObject.SetActive(!button.GetChild(1).gameObject.activeSelf);

                return;
            }

            if (FindObjectOfType<AudioManager>().MuteBackground()) {
                button.GetChild(0).gameObject.SetActive(!button.GetChild(0).gameObject.activeSelf);
                button.GetChild(1).gameObject.SetActive(!button.GetChild(1).gameObject.activeSelf);
            } else {
                button.GetChild(0).gameObject.SetActive(!button.GetChild(0).gameObject.activeSelf);
                button.GetChild(1).gameObject.SetActive(!button.GetChild(0).gameObject.activeSelf);
            }
        }

        public void DestroyObject(string name) {
            Destroy(GameObject.Find(name));
        }
    }
}
