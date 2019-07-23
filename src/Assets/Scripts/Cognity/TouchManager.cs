using System;
using Assets.Scripts.GlobalScripts.UIComponents;
using UnityEngine;

namespace Assets.Scripts.Cognity {
    public class TouchManager : MonoBehaviour {
        private PuzzleManager _puzzleManager;
        private Transform _target;
        private Vector2 _initialPosition;
        private Touch _touch;
        private float _deltaX;
        private float _deltaY;

        public bool Locked { get; set; }

        public bool IsTouching => Input.touchCount > 0;

        private void Update() {
            if (Input.touchCount == 1 && !Locked) {
                // Number of touches
                _touch = Input.GetTouch(0);

                // Screen point
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);

                _puzzleManager = FindObjectOfType<PuzzleManager>();

                switch (_touch.phase) {
                case TouchPhase.Began:
                    if (GetComponent<Collider2D>().Equals(Physics2D.OverlapPoint(touchPos))) {
                        // Current position of the touched object.
                        _deltaX = touchPos.x - transform.position.x;
                        _deltaY = touchPos.y - transform.position.y;

                        // Get the current touched piece
                        Transform curTouchedPiece = Physics2D.OverlapPoint(touchPos).transform;

                        // Stop the last touched piece
                        if (_puzzleManager.LastTouchedPiece != null && _puzzleManager.LastTouchedPiece.name != curTouchedPiece.name) {
                            _puzzleManager.Animate(_puzzleManager.LastTouchedPiece, false);
                        }

                        // Cache the current piece
                        _puzzleManager.LastTouchedPiece = curTouchedPiece;

                        _puzzleManager.Animate(_puzzleManager.LastTouchedPiece, true);

                        // Get the target position for this puzzle piece
                        _target = GetTarget(curTouchedPiece.name);
                    }

                    break;
                case TouchPhase.Moved:
                    Collider2D col2D = GetComponent<Collider2D>();
                    if (col2D.Equals(Physics2D.OverlapPoint(touchPos)) && Physics2D.OverlapPoint(touchPos).tag == "PuzzlePiece") {
                        // Move game object
                        transform.position = new Vector2(touchPos.x - _deltaX, touchPos.y - _deltaY);
                    }

                    break;
                case TouchPhase.Ended:
                    if (_target == null) {
                        return;
                    }

                    bool isHorizontalAligned = Mathf.Abs(transform.position.x - _target.position.x) <= 0.1f;
                    bool isVerticalAligned = Mathf.Abs(transform.position.x - _target.position.x) <= 0.1f;

                    if (isHorizontalAligned && isVerticalAligned) {
                        transform.position = new Vector2(_target.position.x, _target.position.y);

                        // Prevent movement
                        Locked = true;

                        // Count this piece as locked
                        FindObjectOfType<PuzzleOutlineCollider>().LockCount++;

                        // Play sfx
                        FindObjectOfType<AudioManager>().PlayPairedSfx();

                        // Collect the pieces placed within the outline
                        _puzzleManager.AddLockedPiece(transform);

                        _puzzleManager.Animate(_puzzleManager.LastTouchedPiece, false);
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