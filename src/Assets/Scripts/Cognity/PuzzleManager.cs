using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;
using Assets.Scripts.GlobalScripts;

namespace Assets.Scripts.Cognity
{
    /// <summary>
    /// Main worker of the puzzle game.
    /// Manages occuring actions within the puzzle area.
    /// </summary>
    public class PuzzleManager : MonoBehaviour
    {

        [SerializeField] private GameObject _scorePrefab;

        // Puzzle images on the top left
        [SerializeField] private GameObject[] _puzzleImagesPerLevel;

        // Holds current puzzle image
        [SerializeField] private GameObject _puzzleImage;

        // The contents of every level
        [SerializeField] private PuzzlePieceContainer[] _puzzleLevels;

        // Spawn points of each puzzle piece
        [SerializeField] private GameObject[] _spawnPoints;

        // Where the outline will be spawned
        [SerializeField] private Transform _outlineSpawnPoint;

        // Handle scoring
        private ScoreManager _scoreManager;

        // A random rotation after puzzle instantiation
        private readonly int[] _randomRotation = { 0, 90 };

        // Current level starting at 1 then start incrementing
        private int _currentLevel = 1;

        // Signals next level
        private bool _proceedToNextLevel;

        // Holds spawn point IDs
        private List<int> _spawnKeys;

        // Handle when to start
        private Timer _timer;

        // If the piece touched is rotating

        // Is game done ?
        public bool GameDone { get; private set; }

        // The piece user had touched
        public Transform TouchedPiece { get; set; }

        public bool Rotating { get; set; }

        private void Start()
        {
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

        private void Update()
        {
            // There is only 4 levels any more increment should result game completion
            if (_currentLevel > 4 && !GameDone)
            {
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

                // Save to user x
                _scoreManager.SaveUserScore(PlayerPrefs.GetString("user_info"));

                // List all saved scores
                ListScores();
            }

            if (_proceedToNextLevel && !GameDone)
            {
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
                switch (_currentLevel)
                {
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

            if (FindObjectOfType<Timer>().TimerUp)
            {
                // Show failed panel
                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "failed panel")
                    .Panel
                    .gameObject
                    .SetActive(true);
            }
        }

        // Use these for starting the game and going to next level
        public void Populate()
        {
            // Remove the current outline
            Destroy(GameObject.FindGameObjectWithTag("PuzzleOutline"));

            // Destroy puzzle pieces when spawning again
            if (!GameObject.FindGameObjectsWithTag("PuzzlePiece").Equals(null))
            {
                foreach (var existingPiece in GameObject.FindGameObjectsWithTag("PuzzlePiece"))
                {
                    Destroy(existingPiece);
                }
            }

            // To store spawn keys
            _spawnKeys = new List<int>();

            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                _spawnKeys.Add(i);
            }

            // Get puzzle pieces
            var pieces = Array.Find(_puzzleLevels, i => i.LevelName == "Level_" + _currentLevel).PuzzlePiecePrefabs;

            // Instantiate pieces at different spawn points
            foreach (var piece in pieces)
            {

                // Generate random spawn
                int randomKey = Random.Range(0, _spawnKeys.Count);

                // Set the random spawn point 
                int spawnPoint = _spawnKeys.ElementAt(randomKey);

                Instantiate(
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

        public void NextLevel()
        {
            // Increment from current level
            _currentLevel++;

            // Set flag to proceed
            _proceedToNextLevel = true;
        }

        public void Rotate()
        {
            if (Rotating)
            {
                Rotating = false;
            }
            else
            {
                Rotating = true;
            }
        }

        public void FlipHorizontal()
        {
            TouchedPiece.Rotate(0, 180, 0);
        }

        public void FlipVertical()
        {
            TouchedPiece.Rotate(180, 0, 0);
        }

        private void ListScores()
        {
            foreach (var score in _scoreManager.GetUserScoreList())
            {
                Transform parent = Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "scorelist panel").Panel;
                GameObject newPrefab = Instantiate(
                    _scorePrefab,
                    parent.position,
                    Quaternion.identity
                );

                newPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(score.Key + ":");
                newPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(score.Value.ToString());

                newPrefab.transform.SetParent(parent);
            }
        }
    }
}