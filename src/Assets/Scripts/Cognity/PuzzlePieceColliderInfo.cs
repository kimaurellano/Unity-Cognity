using UnityEngine;

namespace Assets.Scripts.Cognity {
    /// <summary>
    /// This Holds information of how many unique colliders are present in the game object
    /// </summary>
    [System.Serializable]
    public class PuzzlePieceColliderInfo {

        public string ColliderName;

        [Header("Count of a colliders")]
        public int Count;
    }
}
