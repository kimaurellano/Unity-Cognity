using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataComponent.Database;
using Assets.Scripts.DataComponent.Model;
using Assets.Scripts.GlobalScripts.Game;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GlobalScripts.Managers {
    /// <summary>
    ///     Handles specific button events. Initialized at the start of the Game
    /// </summary>
    public class ActionManager : MonoBehaviour {

        private static Utility _utility;
        private static Transform _targetPanel;
        private static Transform _currentPanel;
        private static List<Transform> _pageStack;
        private GameCollection _gameCollection;
        private List<Transform> _pages;
        private UIManager _uiManager;
        private bool _isBackPressed;
        private bool _onQuit;

        private void Start() {
            // For debugging. Uncomment to reset scores and user profile
            //PlayerPrefs.DeleteAll();

            // Auto log user if not logged out
            _utility = new Utility();

            _pageStack = new List<Transform>();

            _uiManager = FindObjectOfType<UIManager>();

            _gameCollection = FindObjectOfType<GameCollection>();

            if (SceneManager.GetActiveScene().name.Equals("BaseMenu")) {
                DatabaseManager databaseManager = new DatabaseManager();
                var lastLogged = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged);
                string result = string.Empty;

                StartCoroutine(_utility.LoadJson(data => { result = data.last_user; }));
                if (lastLogged?.Username != null) {
                    // If a user is left logged in but quitted the app
                    if (result == "login") {
                        if (lastLogged.Username != null) {
                            ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"))
                                .gameObject
                                .SetActive(false);
                            ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu"))
                                .gameObject
                                .SetActive(true);
                        }
                    } else {
                        // When exits from a game
                        ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"))
                            .gameObject
                            .SetActive(false);
                        ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "category selection"))
                            .gameObject
                            .SetActive(true);
                    }

                    _pageStack.Add((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu"));
                    _pageStack.Add((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "category selection"));

                    // Manually set visibility since SwitchPanel is not invoked which handles back button visibility
                    Transform btnBack = (Transform)_uiManager.GetUI(UIManager.UIType.Button, "button back");
                    btnBack.gameObject.SetActive(true);
                } else {
                    _pageStack.Add((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"));
                }

                databaseManager.Close();
            }

            // Revert time scale to 1 after quiting from a game which ended with 0f time scale
            Time.timeScale = 1f;
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

                TextMeshProUGUI notifText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "login notif");
                notifText.transform.gameObject.SetActive(true);
                notifText.SetText("User not found");
                notifText.color = new Color32(255, 0, 0, 189);
                return;
            }

            Debug.Log("<color=green>Exists!</color>");

            user.IsLogged = true;
            databaseManager.UpdateUser(user.Username, user);
            databaseManager.Close();

            StartCoroutine(_utility.LoadJson(isDone => {
                if (isDone) {
                    Utility.Data newData = _utility.GetData();
                    newData.last_user = user.Username;
                    _utility.ModifyJson(newData);

                    TransitionFrom((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"));
                    TransitionTo((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu"));

                    // Prevent stacking of the login page after successful login
                    _pageStack.Remove((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"));
                }
            }));
        }

        public void CreateUser(TMP_InputField newUser) {
            TextMeshProUGUI notifText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "create notif");
            DatabaseManager databaseManager = new DatabaseManager();
            if(databaseManager.GetUser(newUser.text)?.Username != null) {
                notifText.transform.gameObject.SetActive(true);
                notifText.SetText("User already exists!");
                notifText.color = new Color32(255, 0, 0, 189);

                databaseManager.Close();
            } else {
                databaseManager.CreateNewUser(new User { Username = newUser.text, IsLogged = false, FirstRun = false });
                notifText.transform.gameObject.SetActive(true);
                notifText.SetText("Created successfully... Please wait");
                notifText.color = new Color32(96, 164, 69, 189);

                databaseManager.Close();

                StartCoroutine(AfterAccCreate());
            }
        }

        private IEnumerator AfterAccCreate() {
            yield return new WaitForSeconds(2f);

            TransitionFrom((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "account create"));
            TransitionTo((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"));
        }

        public void SaveUserPref(TMP_InputField input) {
            // Cache user name
            PlayerPrefs.SetString("user_info", input.text);
        }

        public void GoTo(string sceneName) {
            if (sceneName.StartsWith("Game")) {
                StartCoroutine(_utility.LoadJson(isDone => {
                    if (isDone) {
                        Utility.Data newData = _utility.GetData();
                        newData.page = "category selection";
                        _utility.ModifyJson(newData);
                        Debug.Log($"<color=orange>Json file updated! page:{newData.page}</color>");
                    }
                }));
            }

            // Know where we started
            for (int i = 0; i < _gameCollection.GameCollections.Length; i++) {
                if (_gameCollection.GameCollections[i].Games.Any(sceneName.Equals)) {
                    Utility.Data newData = _utility.GetData();
                    newData.loaded = i;
                    _utility.ModifyJson(newData);
                    Debug.Log($"<color=orange>Json file updated! loaded:{newData.loaded}</color>");
                    break;
                }
            }

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
            DatabaseManager databaseManager = new DatabaseManager();
            User update = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged);
            if (update != null) {
                update.IsLogged = false;
                databaseManager.UpdateUser(update.Username, update);
            }

            databaseManager.Close();

            Utility.Data newData = _utility.GetData();
            newData.page = "login";
            _utility.ModifyJson(newData);

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

            Debug.Log("----");
            foreach (var page in _pageStack) {
                Debug.Log(page.name);
            }
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
            if (_pageStack[_pageStack.Count - 1].name.Equals("StartMenu")) {
                return;
            }

            _pageStack[_pageStack.Count - 1].gameObject.SetActive(false);
            _pageStack[_pageStack.Count - 2].gameObject.SetActive(true);

            _pageStack.RemoveAt(_pageStack.Count - 1);

            Transform btnBack = (Transform)_uiManager.GetUI(UIManager.UIType.Button, "button back");
            btnBack.gameObject.SetActive(_pageStack.Count > 1);
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

        private void OnApplicationQuit() {
            Debug.Log("application quit");
        }

        public void ClearNotif() {
            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "login notif"))
                .SetText(string.Empty);
            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "create notif"))
                .SetText(string.Empty);
        }
    }
}