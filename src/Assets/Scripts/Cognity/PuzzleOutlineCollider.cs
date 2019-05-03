using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Cognity {
    /// <summary>
    /// Outline where the puzzle pieces should be placed correctly.
    /// </summary>
    public class PuzzleOutlineCollider : MonoBehaviour {

        [SerializeField] private Transform[] _puzzlePiecesPlace;

        // Existing collider points within the Outline
        private int _currentCollision;

        // Collision limit
        private const int TOTALCOLLISION = 16; 

        // Number of how many pieces should be locked. We only have 4 pieces per level so 4
        private const int TOTALLOCKCOUNT = 4;

        // Current locked pieces
        public int LockCount { get; set; }

        // Where the puzzle pieces should be within an Outline
        public Transform[] PuzzlePiecesPlace { get => _puzzlePiecesPlace; set => _puzzlePiecesPlace = value; }

        // Current number of collision within the Outline
        public int CurrentCollision { get => _currentCollision; set => _currentCollision = value; }

        private void Update() {
            if(CurrentCollision >= TOTALCOLLISION && 
               !FindObjectOfType<PuzzleManager>().GameDone && 
                LockCount == TOTALLOCKCOUNT){
                FindObjectOfType<PuzzleManager>().NextLevel();
            }

            GetComponent<Collider2D>().enabled = !FindObjectOfType<TouchManager>().IsTouching;

            Debug.Log("Collision:" + CurrentCollision + "/" + TOTALCOLLISION);
        }
    }
}
