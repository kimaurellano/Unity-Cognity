using Assets.Scripts.GlobalScripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Memory {
    public class TouchManager : MonoBehaviour {
        [SerializeField] private GameManager _gameManager;

        private Touch _touch;

        private void Start() {
            _gameManager = GameObject.Find("PicturePuzzleGameManager").GetComponent<GameManager>();
        }

        private void Update() {
            if (Input.touchCount == 1 && !_gameManager.OnFlip) {
                _touch = Input.GetTouch(0);

                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);

                switch (_touch.phase) {
                    case TouchPhase.Began:
                        if (GetComponent<Collider2D>().Equals(Physics2D.OverlapPoint(touchPos))) {

                            _gameManager.TouchCount++;

                            // Get the transform of the touched card
                            Transform touchCard = Physics2D.OverlapPoint(touchPos).transform;

                            if (touchCard.GetComponent<Card>().Locked) {
                                return;
                            }

                            // Animate the touched card to flip up
                            touchCard.GetComponent<Animator>().SetBool("flip", false);

                            // Play sfx on card pick
                            FindObjectOfType<AudioManager>().PlayPairedSfx();

                            if (_gameManager.FirstPick == null) {
                                _gameManager.FirstPick = touchCard;
                            } else {
                                _gameManager.SecondPick = touchCard;
                            }

                            if (_gameManager.FirstPick != null &&
                                _gameManager.SecondPick != null &&
                                _gameManager.FirstPick.name ==
                                _gameManager.SecondPick.name) {
                                _gameManager.LockCount++;
                                Debug.Log(_gameManager.LockCount);

                                // Prevent from picking the already paired cards
                                _gameManager.FirstPick.GetComponent<Card>().Locked = true;
                                _gameManager.SecondPick.GetComponent<Card>().Locked = true;

                                _gameManager.FirstPick = null;
                                _gameManager.SecondPick = null;
                            } else if (_gameManager.FirstPick != null &&
                                       _gameManager.SecondPick != null &&
                                       _gameManager.FirstPick.name !=
                                       _gameManager.SecondPick.name) {
                                StartCoroutine(_gameManager.WaitForFlip());
                            }
                        }

                        break;
                }
            }
        }
    }
}
