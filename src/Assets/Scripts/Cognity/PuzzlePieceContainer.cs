using System;
using UnityEngine;

namespace Assets.Scripts.Cognity {
    /// <summary>
    ///     Contents of a level
    /// </summary>
    [Serializable]
    public class PuzzlePieceContainer {
        public string LevelName;

        public GameObject PuzzleOutline;

        public GameObject[] PuzzlePiecePrefabs;
    }
}
