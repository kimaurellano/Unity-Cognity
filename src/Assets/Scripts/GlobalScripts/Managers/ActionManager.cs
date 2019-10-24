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

            if (SceneManager.GetActiveScene().name.Equals("BaseMenu")) {
                if (PlayerPrefs.GetString("DisplayPage").Equals("CategorySelection")) {
                    Transform panelStartMenu = (Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu");
                    panelStartMenu.gameObject.SetActive(false);

                    Transform panelHome = (Transform)_uiManager.GetUI(UIManager.UIType.Panel, "category selection");
                    panelHome.gameObject.SetActive(true);
                } else {
                    Transform panelStartMenu = (Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu");
                    panelStartMenu.gameObject.SetActive(true);

                    Transform panelHome = (Transform)_uiManager.GetUI(UIManager.UIType.Panel, "category selection");
                    panelHome.gameObject.SetActive(false);
                }
            }

            // Make sure games not paused after quitting any game modes
            Time.timeScale = 1f;

            // Avoid null exception
            if (SceneManager.GetActiveScene().buildIndex != 0) {
                return;
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
            // We do not have to go back to start menu every after game quit
            PlayerPrefs.SetString("DisplayPage", "CategorySelection");

            // Avoid per game category audio duplication(not stopping)
            SceneManager.LoadScene(sceneName);
        }

        [Obsolete("Use GoTo(string) function instead")]
        public void GoToBaseMenu() { 
            SceneManager.LoadScene("BaseMenu");
        }

        public void Back() {
            if(((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu")).gameObject.activeSelf) {
                ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "panel quit")).gameObject.SetActive(true);
            } else if (((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "category selection")).gameObject.activeSelf) {
                ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "category selection")).gameObject.SetActive(false);
                ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu")).gameObject.SetActive(true);
                Transform btnBack = (Transform)_uiManager.GetUI(UIManager.UIType.Button, "button back");
                btnBack.gameObject.SetActive(false);
            } else {
                foreach (GameObject item in GameObject.FindGameObjectsWithTag("CategoryPanel")) {
                    item.gameObject.SetActive(false);
                }

                ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "category selection")).gameObject.SetActive(true);
            }
        }

        public void Quit() {
            ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "panel quit")).gameObject.SetActive(true);
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
            if(targetPanel.name.Equals("start menu")) {
                Transform btnBack = (Transform)_uiManager.GetUI(UIManager.UIType.Button, "button back");
                btnBack.gameObject.SetActive(false);
            } else {
                Transform btnBack = (Transform)_uiManager.GetUI(UIManager.UIType.Button, "button back");
                btnBack.gameObject.SetActive(true);
            }

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
        }

        public void DestroyObject(string name) {
            Destroy(GameObject.Find(name));
        }
    }
}