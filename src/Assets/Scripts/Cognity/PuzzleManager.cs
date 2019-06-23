using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GlobalScripts.Player;
using Assets.Scripts.GlobalScripts.UIComponents;
using Assets.Scripts.GlobalScripts.UITask;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;
using Type = Assets.Scripts.GlobalScripts.Game.Type;

#pragma warning disable 649

namespace Assets.Scripts.Cognity {
    /// <summary>
    ///     Main worker of the puzzle game.
    ///     Manages occuring actions within th e puzzle area.
    /// </summary>
    public class PuzzleManager : MonoBehaviour {
        [SerializeField] private RuntimeAnimatorController _shapeSelAnimatorController;

        // Whether the touched piece is in rotating or drag state
        [SerializeField] private GameObject _pieceStatus;

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
        private ScoreManager _scoreManager;

        [SerializeField] private GameObject _scorePrefab;

        // Holds spawn point IDs
        private List<int> _spawnKeys;

        // Spawn points of each puzzle piece
        [SerializeField] private GameObject[] _spawnPoints;

        // Handle when to start
        private Timer _timer;

        // Is game done ?
        public bool GameDone { get; private set; }

        // The piece user had touched
        public Transform TouchedPiece { get; set; }

        public GameObject PieceStatus => _pieceStatus;

        public bool Rotating { get; set; }

        private void Start() {
            // Score manager of cognity namespace
            _scoreManager = new ScoreManager();

            _timer = FindObjectOfType<Timer>();

            Populate();

            Instantiate(_puzzleImagesPerLevel[_currentLevel - 1], _puzzleImage.transform);

            // First level with timer set as 1:15
            _timer.StartTimerAt(1, 15f);

            Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.textName == "level")
                .textMesh
                .SetText("Level: " + _currentLevel);
        }

        private void Update() {
            // There is only 4 levels any more increment should result game completion
            if (_currentLevel > 4 && !GameDone) {
                GameDone = true;

                // Stop sound
                Array.Find(FindObjectOfType<AudioManager>().AudioCollection, i => i.Name == "background")
                    .AudioSource
                    .Stop();

                // Stop timer
                _timer.ChangeTimerState();

                // Show success panel
                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "success panel")
                    .Panel
                    .gameObject
                    .SetActive(true);

                // Add time as score
                _scoreManager.AddScore(_timer.Min, _timer.Sec);

                // Save the total score
                BaseScoreHandler baseScoreHandler = new BaseScoreHandler();
                baseScoreHandler.AddScore(_scoreManager.TotalScore, Type.GameType.Flexibility);
            }

            if (_proceedToNextLevel && !GameDone) {
                _proceedToNextLevel = false;

                // Clear current puzzle image shape
                Destroy(_puzzleImage.transform.GetChild(0).gameObject);

                // Then  update level image
                Instantiate(_puzzleImagesPerLevel[_currentLevel - 1], _puzzleImage.transform);

                // Add time as score
                _scoreManager.AddScore(_timer.Min, _timer.Sec);

                Array.Find(FindObjectOfType<UIManager>().TextCollection, i => i.textName == "level")
                    .textMesh.SetText("Level: " + _currentLevel);

                // For every level load, timer will reset and start at specified time
                switch (_currentLevel) {
                    case 2:
                        _timer.StartTimerAt(1, 30f);
                        break;
                    case 3:
                        _timer.StartTimerAt(1, 45f);
                        break;
                    case 4:
                        _timer.StartTimerAt(2, 00f);
                        break;
                }

                Populate();
            }

            if (FindObjectOfType<Timer>().TimerUp) {
                // Show failed panel
                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "failed panel")
                    .Panel
                    .gameObject
                    .SetActive(true);
            }
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
            scaleXKey[0] = new Keyframe(0.0f, transform.localScale.x);
            scaleXKey[1] = new Keyframe(0.5f, transform.localScale.x + 0.01f);
            scaleXKey[2] = new Keyframe(1.0f, transform.localScale.x);
            AnimationCurve curve = new AnimationCurve(scaleXKey);
            // and assign to the clip
            animationClip.SetCurve("", typeof(Transform), "localScale.x", curve);

            Keyframe[] scaleYKey = new Keyframe[3];
            scaleYKey[0] = new Keyframe(0.0f, transform.localScale.y);
            scaleYKey[1] = new Keyframe(0.5f, transform.localScale.y + 0.01f);
            scaleYKey[2] = new Keyframe(1.0f, transform.localScale.y);
            curve = new AnimationCurve(scaleYKey);
            // and assign to the clip
            animationClip.SetCurve("", typeof(Transform), "localScale.y", curve);

            // Play the animation
            anim.AddClip(animationClip, animationClip.name);
            anim.Play(animationClip.name);
            anim.wrapMode = WrapMode.Loop;
        }

        // Use these for starting the game and going to next level
        public void Populate() {
            // Remove the current outline
            Destroy(GameObject.FindGameObjectWithTag("PuzzleOutline"));

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

                GameObject t = Instantiate(
                    piece,
                    _spawnPoints[spawnPoint].transform.position,
                    // Generate random rotation at z axis
                    Quaternion.Euler(
                        0,
                        0,
                        _randomRotation[Random.Range(0, _randomRotation.Length)]));

                // Avoid using the same spawn point
                _spawnKeys.RemoveAt(randomKey);
            }

            // Display next level puzzle outline
            Instantiate(
                Array.Find(_puzzleLevels, i => i.LevelName == "Level_" + _currentLevel).PuzzleOutline,
                _outlineSpawnPoint.transform.position,
                Quaternion.identity
            );
        }

        public void SetPieceStatus(string state) {
            if (state == "drag") {
                TouchedPiece.GetChild(0).gameObject.SetActive(true);
                TouchedPiece.GetChild(1).gameObject.SetActive(false);
            }
            else {
                TouchedPiece.GetChild(0).gameObject.SetActive(false);
                TouchedPiece.GetChild(1).gameObject.SetActive(true);
            }
        }

        public void NextLevel() {
            // Increment from current level
            _currentLevel++;

            // Set flag to proceed
            _proceedToNextLevel = true;
        }

        public void Rotate() {
            Rotating = !Rotating;
        }

        public void FlipHorizontal() {
            TouchedPiece.Rotate(0, 180, 0);
        }

        public void FlipVertical() {
            TouchedPiece.Rotate(180, 0, 0);
        }
    }
}
