using System;
using UnityEngine;

namespace Assets.Scripts.Cognity {
    public class TouchManager : MonoBehaviour {
        private Transform _target;
        private Vector2 _initialPosition;
        private float _deltaX;
        private float _deltaY;
        private bool _locked;
        private Touch _touch;

        public bool IsTouching => Input.touchCount > 0;

        private void Update() {
            if (Input.touchCount == 1 && !_locked) {
                // Number of touches
                _touch = Input.GetTouch(0);

                // Screen point
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);

                switch (_touch.phase) {
                case TouchPhase.Began:
                    if (GetComponent<Collider2D>().Equals(Physics2D.OverlapPoint(touchPos))) {
                        // Current position of the touched object.
                        _deltaX = touchPos.x - transform.position.x;
                        _deltaY = touchPos.y - transform.position.y;

                        // Tell puzzle manager which piece has been touched
                        var touchedPiece = Physics2D.OverlapPoint(touchPos).transform;

                        if (touchedPiece.tag != "PuzzlePiece") {
                            return;
                        }

                        FindObjectOfType<PuzzleManager>().TouchedPiece = touchedPiece;

                        // Get the target position for this puzzle piece
                        _target = GetTarget(touchedPiece.name);
                    }

                    break;
                case TouchPhase.Moved:
                    Collider2D col2D = GetComponent<Collider2D>();
                    if (col2D.Equals(Physics2D.OverlapPoint(touchPos)) && Physics2D.OverlapPoint(touchPos).tag == "PuzzlePiece") {
                        if (FindObjectOfType<PuzzleManager>().Rotating) {
                            // Will work only after build
                            float axisRotY = Input.GetAxis("Mouse Y") * 10f;
                            transform.Rotate(Vector3.forward, axisRotY);
                        } else {
                            // Move game object
                            transform.position = new Vector2(touchPos.x - _deltaX, touchPos.y - _deltaY);
                        }
                    }

                    break;
                case TouchPhase.Ended:
                    bool isHorizontalAligned = Mathf.Abs(transform.position.x - _target.position.x) <= 0.1f;
                    bool isverticalAligned = Mathf.Abs(transform.position.x - _target.position.x) <= 0.1f;

                    if (isHorizontalAligned && isverticalAligned) {
                        transform.position = new Vector2(_target.position.x, _target.position.y);

                        // Prevent movement
                        _locked = true;

                        // Count this piece as locked
                        FindObjectOfType<PuzzleOutlineCollider>().LockCount++;

                        // Play sfx
                        FindObjectOfType<AudioManager>().PlaySnapSfx();
                    }
                    break;
                }
            }
        }

        private static Transform GetTarget(string puzzlePieceName) {
            Transform[] places = FindObjectOfType<PuzzleOutlineCollider>().PuzzlePiecesPlace;
            Transform targetPlace = Array.Find(places, i => i.name.StartsWith(puzzlePieceName.Split('_')[2]));

            return targetPlace;
        }
    }
}
