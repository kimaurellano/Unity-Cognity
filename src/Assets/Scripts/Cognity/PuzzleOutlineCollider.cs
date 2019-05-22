using UnityEngine;

namespace Assets.Scripts.Cognity {
    /// <summary>
    ///     Outline where the puzzle pieces should be placed correctly.
    /// </summary>
    public class PuzzleOutlineCollider : MonoBehaviour {
        // Collision limit
        private const int TOTALCOLLISION = 16;

        // Number of how many pieces should be locked. We only have 4 pieces per level so 4
        private const int TOTALLOCKCOUNT = 4;

        // Current locked pieces
        public int LockCount { get; set; }

        // Where the puzzle pieces should be within an Outline
        [field: SerializeField] public Transform[] PuzzlePiecesPlace { get; set; }

        // Current number of collision within the Outline
        public int CurrentCollision { get; set; }

        private void Update() {
            if (CurrentCollision >= TOTALCOLLISION &&
                !FindObjectOfType<PuzzleManager>().GameDone &&
                LockCount == TOTALLOCKCOUNT) {
                FindObjectOfType<PuzzleManager>().NextLevel();
            }

            GetComponent<Collider2D>().enabled = !FindObjectOfType<TouchManager>().IsTouching;

            //Debug.Log("Collision:" + CurrentCollision + "/" + TOTALCOLLISION);
        }
    }
}
