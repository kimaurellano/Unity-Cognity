using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataComponent.Database;
using Assets.Scripts.DataComponent.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Assets.Scripts.GlobalScripts.Managers {
    /// <summary>
    ///     Handles specific button events. Initialized at the start of the Game
    /// </summary>
    public class ActionManager : MonoBehaviour {

        private static Transform _targetPanel;
        private static Transform _currentPanel;
        private static List<Transform> _pageStack;
        private List<Transform> _pages;
        private UIManager _uiManager;
        private bool _isBackPressed;
        private bool _onQuit;

        private void Start() {
            // For debugging. Uncomment to reset scores and user profile
            //PlayerPrefs.DeleteAll();

            _uiManager = FindObjectOfType<UIManager>();

            _pageStack = new List<Transform>();

            // Auto log user if not logged out
            Utility utility = new Utility();
            StartCoroutine(utility.LoadJson(category => {
                if (category.last_user != string.Empty) {
                    TransitionFrom((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"));
                    TransitionTo((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu"));

                    // The user is logged in
                    DatabaseManager databaseManager = new DatabaseManager();
                    User update = databaseManager.GetUser(category.last_user);
                    update.IsLogged = true;

                    databaseManager.UpdateUser(category.last_user, update);
                    databaseManager.Close();
                }
            }));
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                // Ignore other ActionManager script instance
                if (transform.name.Equals("ActionManager")) {
                    Back();
                }
            }
        }

        public void CheckInput(TMP_InputField input) {
            DatabaseManager databaseManager = new DatabaseManager();
            var user = databaseManager.GetUser(input.text);
            if(user == null) {
                Debug.Log("<color=red>Not found!</color>");
                return;
            }

            Debug.Log("<color=green>Exists!</color>");

            user.IsLogged = true;
            databaseManager.UpdateUser(user.Username, user);
            databaseManager.Close();

            Utility utility = new Utility();
            StartCoroutine(utility.LoadJson(isDone => {
                if (isDone) {
                    // The logged user
                    utility.WriteValue(user.Username);

                    TransitionFrom((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"));
                    TransitionTo((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu"));
                }
            }));
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

        public void QuitDialog() {
            _onQuit = !_onQuit;

            ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "panel quit")).gameObject.SetActive(true);

            Animator anim = (Animator) _uiManager.GetUI(UIManager.UIType.AnimatedMultipleState, "quit dialog");
            anim.Play(_onQuit ? "Quit" : "Hide");

            if (!_onQuit) {
                StartCoroutine(QuitDialogExit());
            }
        }

        private IEnumerator QuitDialogExit() {
            yield return new WaitForSeconds(1f);
            ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "panel quit")).gameObject.SetActive(false);
        }

        public void QuitApp() {
            // Auto log-out user
            Utility utility = new Utility();
            StartCoroutine(utility.LoadJson(category => {
                if (category.last_user != string.Empty) {
                    string user = category.last_user;

                    // Empty json last_user
                    utility.WriteValue(string.Empty);

                    DatabaseManager databaseManager = new DatabaseManager();
                    User update = databaseManager.GetUser(user);
                    update.IsLogged = false;

                    databaseManager.UpdateUser(user, update);
                    databaseManager.Close();
                }
            }));

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

        private IEnumerator BeginTransition(Transform transform) {
            Animation transition = (Animation)_uiManager.GetUI(UIManager.UIType.AnimatedSingleState, "transition");
            transition.Play();

            // Sync to the seconds when animation event is invoked
            yield return new WaitForSeconds(0.3f);

            Transform btnBack = (Transform)_uiManager.GetUI(UIManager.UIType.Button, "button back");
            btnBack.gameObject.SetActive(_pageStack.Count > 1);
        }

        /// <summary>
        ///     Animation event for transition panel
        /// </summary>
        public void SwitchPanel() {
            _currentPanel.gameObject.SetActive(false);
            _targetPanel.gameObject.SetActive(true);

            if (_pageStack.Contains(_targetPanel)) {
                return;
            }

            _pageStack.Add(_targetPanel);
        }

        public void Back() {
            TransitionFrom(_pageStack[_pageStack.Count - 1]);
            TransitionTo(_pageStack[_pageStack.Count - 2]);

            _pageStack.RemoveAt(_pageStack.Count - 1);
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
            }
        }

        public void DestroyObject(string name) {
            Destroy(GameObject.Find(name));
        }

        public void TestDbWrite() {
            DatabaseManager databaseManager = new DatabaseManager();
            databaseManager.CreateNewUser(new User{ Username = "test", IsLogged = false, FirstRun = true });
        }

        private void OnApplicationQuit() {
            PlayerPrefs.SetString("DisplayPage", "StartMenu");
            Debug.Log("application quit");
        }
    }
}