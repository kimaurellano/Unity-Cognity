using UnityEngine;

namespace Assets.Scripts.Cognity {
    /// <summary>
    /// Set area where puzzle piece should be able to move
    /// </summary>
    [System.Serializable]
    public class PuzzleAreaBoundary : MonoBehaviour {
        public float XMin, XMax, YMin, YMax;
    }
}
