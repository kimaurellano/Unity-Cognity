using Assets.Scripts.GlobalScripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using static Assets.Scripts.GlobalScripts.Player.BaseScoreHandler;

namespace Assets.Scripts.Cognity {
    /// <summary>
    ///     Main worker of the puzzle game.
    ///     Manages occuring actions within th e puzzle area.
    /// </summary>
    public class PuzzleManager : CoreGameBehaviour {

        private List<KeyValuePair<int, int[]>> _levelRotList;

        private int[] _curRotList;

        private int _curRotIdx;

        private UIManager _uiManager;

        //
        private Dictionary<string, Vector3> _originalPieceLocation;

        private List<string> _lockedPiece;

        // A random rotation after puzzle instantiation
        private readonly int[] _randomRotation = {0, 90};

        // Current level starting at 1 then start incrementing
        private int _currentLevel = 1;

        // Where the outline will be spawned
        [SerializeField] private Transform _outlineSpawnPoint;

        // Signals next level
        private bool _proceedToNextLevel;

        // Holds current puzzle image
        [SerializeField] private GameObject _puzzleImage;

        // Puzzle images on the top left
        [SerializeField] private GameObject[] _puzzleImagesPerLevel;

        // The contents of every level
        [SerializeField] private PuzzlePieceContainer[] _puzzleLevels;

        // Handle scoring
        private PuzzleScoreManager _puzzleScoreManager;

        [SerializeField] private GameObject _scorePrefab;

        // Holds spawn point IDs
        private List<int> _spawnKeys;

        // Spawn points of each puzzle piece
        [SerializeField] private GameObject[] _spawnPoints;

        // Handle when to start
        private TimerManager _timerManager;

        // Is game done ?
        public bool GameDone { get; private set; }

        // The piece user had touched
        public Transform LastTouchedPiece { get; set; }

        private void Start() {
            // Score manager of cognity namespace
            _puzzleScoreManager = new PuzzleScoreManager();

            _originalPieceLocation = new Dictionary<string, Vector3>();

            _lockedPiece = new List<string>();

            _timerManager = GetComponent<TimerManager>();

            _uiManager = FindObjectOfType<UIManager>();

            _levelRotList = new List<KeyValuePair<int, int[]>>();

            // Limit of rotation specific to levels
            _levelRotList.Add(new KeyValuePair<int, int[]>(1, new []{ 0, 90, 180, 270, 360 }));
            _levelRotList.Add(new KeyValuePair<int, int[]>(2, new []{ 182 }));
            _levelRotList.Add(new KeyValuePair<int, int[]>(3, new []{ 0, 90, 180, 270, 360 }));
            _levelRotList.Add(new KeyValuePair<int, int[]>(4, new []{ 334, 18, 22, -874, 0 }));

            _curRotList = _levelRotList[_currentLevel - 1].Value;

            _curRotIdx = 0;

            TimerManager.OnPreGameTimerEndEvent += StartGame;
        }

        private void Update() {
            // Forcefully destroys the Animation component to prevent "select" animation to still occur when locked
            if (GameObject.FindGameObjectsWithTag("PuzzlePiece") != null) {
                foreach (GameObject piece in GameObject.FindGameObjectsWithTag("PuzzlePiece")) {
                    if (piece.GetComponent<TouchManager>().Locked && piece.GetComponent<Animation>() != null) {
                        Destroy(piece.GetComponent<Animation>());
                    }
                }
            }

            // There is only 4 levels any more increment should result game completion
            if (_currentLevel > 4 && !GameDone) {
                GameDone = true;

                // Stop timer
                _timerManager.ChangeTimerState();

                // Show success panel
                Transform successPanel = (Transform)_uiManager.GetUI(UIManager.UIType.Panel, "game finish panel");
                successPanel.gameObject.SetActive(true);

                TextMeshProUGUI gameResult = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "game result");
                gameResult.SetText("SUCCESS");

                // Add time as score
                _puzzleScoreManager.AddScore(_timerManager.Minutes, _timerManager.Seconds);

                // Save final score
                SaveScore(_puzzleScoreManager.TotalScore, GameType.Flexibility);
            }

            if (_proceedToNextLevel && !GameDone) {
                _proceedToNextLevel = false;

                // Clear current puzzle image shape
                Destroy(_puzzleImage.transform.GetChild(0).gameObject);

                // Then  update level image
                Instantiate(_puzzleImagesPerLevel[_currentLevel - 1], _puzzleImage.transform);

                // Add time as score
                _puzzleScoreManager.AddScore(_timerManager.Minutes, _timerManager.Seconds);

                TextMeshProUGUI levelText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "level");
                levelText.SetText(string.Format("Level: {0}", _currentLevel));

                // For every level load, timer will reset and start at specified time
                switch (_currentLevel) {
                    case 2:
                        _timerManager.StartTimerAt(1, 30f);
                        break;
                    case 3:
                        _timerManager.StartTimerAt(1, 45f);
                        break;
                    case 4:
                        _timerManager.StartTimerAt(2, 00f);
                        break;
                }

                Populate();
            }

            if (_timerManager.TimerUp) {
                Transform gameFinishPanel = (Transform)_uiManager.GetUI(UIManager.UIType.Panel, "game finish panel");
                gameFinishPanel.gameObject.SetActive(true);

                TextMeshProUGUI gameResult = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "game result");
                gameResult.SetText("FAILED!");

                SaveScore(_puzzleScoreManager.TotalScore, GameType.Flexibility);
            }
        }

        public override void Pause() {
            base.Pause();
        }

        private void StartGame() {
            TimerManager.OnPreGameTimerEndEvent -= StartGame;

            Populate();

            Instantiate(_puzzleImagesPerLevel[_currentLevel - 1], _puzzleImage.transform);

            // First level with timer set as 1:15
            _timerManager.StartTimerAt(1, 15f);

            TextMeshProUGUI levelText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "level");
            levelText.SetText(string.Format("Level: {0}", _currentLevel));
        }

        /// <summary>
        ///     Animates the selected puzzle piece.
        /// </summary>
        /// <param name="transform">Transform to animate</param>
        /// <param name="animate">Start or Stop animation</param>
        public void Animate(Transform transform, bool animate) {
            if (transform.gameObject.GetComponent<Animation>() != null && !animate) {
                Vector3 origScale = new Vector3(0.15f, 0.15f);

                // Reset to original scale upon animation stop
                transform.localScale = origScale;

                Destroy(transform.gameObject.GetComponent<Animation>());

                return;
            }

            // Avoid duplicate instance of Animator component of puzzle piece
            if (transform.GetComponent<Animation>() == null) {
                transform.gameObject.AddComponent<Animation>();
            }

            Animation anim = transform.gameObject.GetComponent<Animation>();

            // Create new animation clip
            AnimationClip animationClip = new AnimationClip {legacy = true};

            // Create a curve to scale the x axis of the GameObject
            Keyframe[] scaleXKey = new Keyframe[3];
            scaleXKey[0] = new Keyframe(0.0f, 0.15f);
            scaleXKey[1] = new Keyframe(0.5f, 0.15f + 0.01f);
            scaleXKey[2] = new Keyframe(1.0f, 0.15f);
            AnimationCurve curve = new AnimationCurve(scaleXKey);
            // and assign to the clip
            animationClip.SetCurve("", typeof(Transform), "localScale.x", curve);

            Keyframe[] scaleYKey = new Keyframe[3];
            scaleYKey[0] = new Keyframe(0.0f, 0.15f);
            scaleYKey[1] = new Keyframe(0.5f, 0.15f + 0.01f);
            scaleYKey[2] = new Keyframe(1.0f, 0.15f);
            curve = new AnimationCurve(scaleYKey);
            // and assign to the clip
            animationClip.SetCurve("", typeof(Transform), "localScale.y", curve);

            // Play the animation
            anim.AddClip(animationClip, animationClip.name);
            anim.Play(animationClip.name);
            anim.wrapMode = WrapMode.Loop;
        }

        /// <summary>
        ///     Use these for starting the game and going to next level
        /// </summary>
        public void Populate() {
            // Remove the current outline
            Destroy(GameObject.FindGameObjectWithTag("PuzzleOutline"));

            // Avoid duplication
            _originalPieceLocation.Clear();

            // Destroy puzzle pieces when spawning again
            if (!GameObject.FindGameObjectsWithTag("PuzzlePiece").Equals(null)) {
                foreach (GameObject existingPiece in GameObject.FindGameObjectsWithTag("PuzzlePiece")) {
                    Destroy(existingPiece);
                }
            }

            // To store spawn keys
            _spawnKeys = new List<int>();

            for (int i = 0; i < _spawnPoints.Length; i++) {
                _spawnKeys.Add(i);
            }

            // Get puzzle pieces
            GameObject[] pieces = Array.Find(_puzzleLevels, i => i.LevelName == "Level_" + _currentLevel).PuzzlePiecePrefabs;

            // Instantiate pieces at different spawn points
            foreach (GameObject piece in pieces) {
                // Generate random spawn
                int randomKey = Random.Range(0, _spawnKeys.Count);

                // Set the random spawn point 
                int spawnPoint = _spawnKeys.ElementAt(randomKey);

                GameObject pieceInstance = Instantiate(
                    piece,
                    _spawnPoints[spawnPoint].transform.position,
                    // Generate random rotation at z axis
                    Quaternion.identity);

                // Avoid using the same spawn point
                _spawnKeys.RemoveAt(randomKey);

                // Cache the original location of the piece
                _originalPieceLocation.Add(pieceInstance.transform.name, pieceInstance.transform.position);
            }

            // Display next level puzzle outline
            Instantiate(
                Array.Find(_puzzleLevels, i => i.LevelName == "Level_" + _currentLevel).PuzzleOutline,
                _outlineSpawnPoint.transform.position,
                Quaternion.identity
            );
        }

        /// <summary>
        ///     Add piece to be listed as locked in chronological order
        /// </summary>
        /// <param name="toLock">piece to be listed as locked</param>
        public void AddLockedPiece(Transform toLock) {
            _lockedPiece.Add(toLock.name);
        }

        public void UndoPiece() {
            if (_lockedPiece.Count == 0) {
                return;
            }

            string lastLockedPiece = _lockedPiece.ToArray()[_lockedPiece.ToArray().Length - 1];
            // Remove that piece from list
            _lockedPiece.Remove(lastLockedPiece);
            // Get the current state of last locked piece
            GameObject updatedPieceState = GameObject.Find(lastLockedPiece);
            // Then reset to its original position
            updatedPieceState.transform.position = _originalPieceLocation[lastLockedPiece];
            // Set as movable
            updatedPieceState.GetComponent<TouchManager>().Locked = false;
        }

        public void NextLevel() {
            // Increment from current level
            _currentLevel += 1;

            // Set flag to proceed
            _proceedToNextLevel = true;

            // Available rotation for the level
            _curRotList = _levelRotList[_currentLevel - 1].Value;

            _curRotIdx = 0;
        }

        public void Rotate() {
            if (_curRotIdx > _curRotList.Length - 1) {
                _curRotIdx = 0;
            }

            int rotationAt = _curRotList.ElementAt(_curRotIdx++);

            Debug.Log("rotation:" + rotationAt);

            LastTouchedPiece.Rotate(0, 0, rotationAt);
        }

        public void FlipHorizontal() {
            LastTouchedPiece.Rotate(0, 180, 0);
        }

        public void FlipVertical() {
            LastTouchedPiece.Rotate(180, 0, 0);
        }
    }
}
