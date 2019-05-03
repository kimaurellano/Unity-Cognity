using UnityEngine;

namespace Assets.Scripts.Cognity {
    /// <summary>
    /// Contents of a level
    /// </summary>
    [System.Serializable]
    public class PuzzlePieceContainer {
        public string LevelName;
        public GameObject[] PuzzlePiecePrefabs;
        public GameObject PuzzleOutline;
    }
}
