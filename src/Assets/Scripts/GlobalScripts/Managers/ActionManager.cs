using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GlobalScripts.Managers {
    /// <summary>
    ///     Handles specific button events. Initialized at the start of the Game
    /// </summary>
    public class ActionManager : MonoBehaviour {

        private static Transform _targetPanel;
        private static Transform _currentPanel;

        private UIManager _uiManager;

        private void Start() {
            // For debugging. Uncomment to reset scores and user profile
            //PlayerPrefs.DeleteAll();

            _uiManager = FindObjectOfType<UIManager>();

            // Make sure games not paused after quitting any game modes
            Time.timeScale = 1f;

            // Avoid null exception
            if (SceneManager.GetActiveScene().buildIndex != 0) {
                return;
            }

            if (PlayerPrefs.GetString("user_info") != string.Empty) {
                Transform panelUserInfo = (Transform) _uiManager.GetUI(UIManager.UIType.Panel, "panel userinfo");
                panelUserInfo.gameObject.SetActive(false);

                Transform panelHome = (Transform)_uiManager.GetUI(UIManager.UIType.Panel, "panel home");
                panelHome.gameObject.SetActive(true);
            }
        }

        public void CheckInput(TMP_InputField input) {
            Transform buttonSave = (Transform) _uiManager.GetUI(UIManager.UIType.Button, "button save");
            buttonSave.gameObject.SetActive(input.text != string.Empty);
        }

        public void SaveUserPref(TMP_InputField input) {
            // Cache user name
            PlayerPrefs.SetString("user_info", input.text);
        }

        public void GoTo(string sceneName) {
            // Avoid per game category audio duplication(not stopping)
            SceneManager.LoadScene(sceneName);
        }

        [Obsolete("Use GoTo(string) function instead")]
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

        public void TransitionFrom(Transform currentPanel) {
            _currentPanel = currentPanel;

            // Start panel transition to transition from
            StartCoroutine(BeginTransition(currentPanel));
        }

        public void TransitionTo(Transform targetPanel) {
            // The panel to transition to
            _targetPanel = targetPanel;
        }

        /// <summary>
        ///     Animation event for transition panel
        /// </summary>
        public void SwitchPanel() {
            _currentPanel.gameObject.SetActive(false);
            _targetPanel.gameObject.SetActive(true);
        }

        private IEnumerator BeginTransition(Transform transform) {
            Animation transition = (Animation)_uiManager.GetUI(UIManager.UIType.AnimatedSingleState, "transition");
            transition.Play();

            // Sync to the seconds when animation event is invoked
            yield return new WaitForSeconds(0.5f);

            if (transform.name == "User_Panel") {
                Transform panelUser = (Transform) _uiManager.GetUI(UIManager.UIType.Panel, "panel user");
                panelUser.gameObject.SetActive(true);
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

            //if (FindObjectOfType<AudioManager>().LowerVolume("bg_game", 0f)) {
            //    button.GetChild(0).gameObject.SetActive(!button.GetChild(0).gameObject.activeSelf);
            //    button.GetChild(1).gameObject.SetActive(!button.GetChild(1).gameObject.activeSelf);
            //} else {
            //    button.GetChild(0).gameObject.SetActive(!button.GetChild(0).gameObject.activeSelf);
            //    button.GetChild(1).gameObject.SetActive(!button.GetChild(0).gameObject.activeSelf);
            //}
        }

        public void DestroyObject(string name) {
            Destroy(GameObject.Find(name));
        }
    }
}
