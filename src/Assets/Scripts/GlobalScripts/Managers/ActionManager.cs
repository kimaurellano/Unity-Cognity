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
using UnityEngine.UI;

namespace Assets.Scripts.GlobalScripts.Managers {
    /// <summary>
    ///     Handles specific button events. Initialized at the start of the Game
    /// </summary>
    public class ActionManager : MonoBehaviour {

        [SerializeField] private Transform _buttonMusic;
        [SerializeField] private Transform _buttonSfx;

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
            DatabaseManager db = new DatabaseManager();
            db.DeletePersistentData();

            _utility = new Utility();

            _pageStack = new List<Transform>();

            _uiManager = FindObjectOfType<UIManager>();

            _gameCollection = FindObjectOfType<GameCollection>();

            if (SceneManager.GetActiveScene().name.Equals("BaseMenu")) {
                DatabaseManager databaseManager = new DatabaseManager();
                var lastLogged = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged);

                if(lastLogged == null) {
                    Debug.Log($"<color=red>UserPrefs is null</color>");
                    return;
                }

                Debug.Log($"<color=yellow>User logged: {lastLogged.Username}</color>");

                string pageToLoad = string.Empty;

                StartCoroutine(_utility.LoadJson(data => { pageToLoad = data.page; }));
                // If a user is left logged in but quitted the app
                if (lastLogged.Username != null) {
                    FindObjectOfType<StatsManager>().UpdateRadarChart();

                    if (pageToLoad == "login") {
                        Debug.Log("<color=green>Json file page:login</color>");
                        if (lastLogged.Username != null) {
                            ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"))
                                .gameObject
                                .SetActive(false);
                            ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu"))
                                .gameObject
                                .SetActive(true);
                        }
                    } else {
                        Debug.Log($"<color=green>page to load:{pageToLoad}</color>");
                        // When exits from a game
                        ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"))
                            .gameObject
                            .SetActive(false);
                        ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, pageToLoad))
                            .gameObject
                            .SetActive(true);
                    }

                    _pageStack.Add((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "start menu"));
                    _pageStack.Add((Transform)_uiManager.GetUI(UIManager.UIType.Panel, pageToLoad));

                    // Manually set visibility since SwitchPanel is not invoked which handles back button visibility
                    Transform btnBack = (Transform)_uiManager.GetUI(UIManager.UIType.Button, "button back");
                    btnBack.gameObject.SetActive(true);
                } else {
                    _pageStack.Add((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"));
                }

                databaseManager.Close();
            }

            // Revert time scale to 1 after quiting from
            // a game which ended with 0f time scale
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

            FindObjectOfType<StatsManager>().UpdateRadarChart();

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

        public void AttachAction(Transform button) {
            Button buttonComponent = button.GetComponent<Button>();

            // Avoid stacking unused subscribe methods
            buttonComponent.onClick.RemoveAllListeners();

            string sceneToLoad = button.name.Split('_')[1];
            string gameName = button.name.Split('_')[2];

            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "game to load"))
                .SetText(gameName);

            ((Transform)_uiManager.GetUI(UIManager.UIType.Button, "button start game"))
                .GetComponent<Button>().onClick.AddListener(() => {
                    FindObjectOfType<ActionManager>().GoTo(sceneToLoad);
                });

            ((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "pre game menu"))
                .gameObject.SetActive(true);
        }

        public void StartSession() {
            SceneManager.LoadScene(FindObjectOfType<GameCollection>().GetNextScene());
        }

        public void MuteSfx() {
            AudioSource src = FindObjectOfType<AudioManager>().GetAttachedAudioComponent("Click");
            src.mute = !src.mute;
            if (src.mute) {
                _buttonSfx.GetChild(0).gameObject.SetActive(true);
                _buttonSfx.GetChild(1).gameObject.SetActive(false);
            } else {
                _buttonSfx.GetChild(0).gameObject.SetActive(false);
                _buttonSfx.GetChild(1).gameObject.SetActive(true);
            }
        }

        public void MuteBg() {
            AudioSource src = FindObjectOfType<AudioManager>().GetAttachedAudioComponent("JazzyFrench");
            src.mute = !src.mute;
            if (src.mute) {
                _buttonMusic.GetChild(0).gameObject.SetActive(true);
                _buttonMusic.GetChild(1).gameObject.SetActive(false);
            } else {
                _buttonMusic.GetChild(0).gameObject.SetActive(false);
                _buttonMusic.GetChild(1).gameObject.SetActive(true);
            }
        }

        public void CreateUser(TMP_InputField newUser) {
            TextMeshProUGUI notifText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "create notif");

            if (newUser.text == string.Empty) {
                notifText.transform.gameObject.SetActive(true);
                notifText.SetText("Field cannot be empty");
                notifText.color = new Color32(255, 0, 0, 189);

                return;
            }

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
            yield return new WaitForSeconds(1f);

            TransitionFrom((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "account create"));
            TransitionTo((Transform)_uiManager.GetUI(UIManager.UIType.Panel, "login"));
        }

        public void GoTo(string sceneName) {
            if (sceneName.StartsWith("Game")) {
                StartCoroutine(_utility.LoadJson(isDone => {
                    if (isDone) {
                        Utility.Data newData = _utility.GetData();
                        newData.page = "category selection";
                        _utility.ModifyJson(newData);
                        Debug.Log($"<color=green>Json file updated! page:{_utility.GetData().page}</color>");
                    }
                }));
            }

            for (int i = 0; i < _gameCollection.GameCollections.Length; i++) {
                if (_gameCollection.GameCollections[i].Games.Any(sceneName.Equals)) {
                    // Know where we started. At what scene index from GameCollection
                    FindObjectOfType<GameCollection>().Loaded = i;
                    Debug.Log($"<color=orange>Json file updated! loaded:{FindObjectOfType<GameCollection>().Loaded}</color>");
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
            UserPrefs update = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged);
            if (update != null) {
                update.IsLogged = false;
                databaseManager.UpdateUser(update.Username, update);
            }

            databaseManager.Close();

            Utility.Data newData = _utility.GetData();
            newData.page = "login";
            _utility.ModifyJson(newData);

            Debug.Log($"<color=green>Update to page:{_utility.GetData().page}</green>");

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
        /// Animation event for transition panel
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

        public void DestroyObject(string name) {
            Destroy(GameObject.Find(name));
        }

        private void OnApplicationQuit() {
            StartCoroutine(_utility.LoadJson(isDone => {
                if (isDone) {
                    Utility.Data newData = _utility.GetData();
                    newData.page = "start menu";
                    _utility.ModifyJson(newData);
                    Debug.Log($"<color=green>Json file updated! page:{_utility.GetData().page}</color>");
                }
            }));
        }

        public void ClearNotif() {
            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "login notif"))
                .SetText(string.Empty);
            ((TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "create notif"))
                .SetText(string.Empty);
        }
    }
}