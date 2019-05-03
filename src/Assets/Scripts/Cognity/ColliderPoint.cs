using UnityEngine;

namespace Assets.Scripts.Cognity {
    /// <summary>
    /// Checks collision of each corner of a puzzle piece
    /// </summary>
    public class ColliderPoint : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            // Colliding with parent game object
            if(other.gameObject.tag == "PuzzlePiece"){
                return;
            }

            FindObjectOfType<PuzzleOutlineCollider>().CurrentCollision++;
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(other.gameObject.tag == "PuzzlePiece"){
                return;
            }

            FindObjectOfType<PuzzleOutlineCollider>().CurrentCollision--;
        }
    }
}