using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Cognity {
    /// <summary>
    /// Monitors if all colliders of this game object has collided or not
    /// </summary>
    [RequireComponent(typeof(PuzzleAreaBoundary))]
    public class PuzzlePiece : MonoBehaviour {

        private PuzzleAreaBoundary _puzzleAreaBoundary;

        private void Start() {
            _puzzleAreaBoundary = GetComponent<PuzzleAreaBoundary>();
        }

        private void Update() {
            // Limit area where puzzle pieces can move
            transform.position = new Vector3(
                Mathf.Clamp(transform.transform.position.x, _puzzleAreaBoundary.XMin, _puzzleAreaBoundary.XMax),
                Mathf.Clamp(transform.transform.position.y, _puzzleAreaBoundary.YMin, _puzzleAreaBoundary.YMax));
        }
    }
}
