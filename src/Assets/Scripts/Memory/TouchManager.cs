using Assets.Scripts.GlobalScripts.UIComponents;
using UnityEngine;

namespace Assets.Scripts.Memory {
    public class TouchManager : MonoBehaviour {
        [SerializeField] private CardManager _cardManager;

        private Touch _touch;

        private void Start() {
            _cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
        }

        private void Update() {
            if (Input.touchCount == 1 && !_cardManager.OnFlip) {
                _touch = Input.GetTouch(0);

                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);

                switch (_touch.phase) {
                    case TouchPhase.Began:
                        if (GetComponent<Collider2D>().Equals(Physics2D.OverlapPoint(touchPos))) {

                            _cardManager.TouchCount++;

                            // Get the transform of the touched card
                            Transform touchCard = Physics2D.OverlapPoint(touchPos).transform;

                            if (touchCard.GetComponent<Card>().Locked) {
                                return;
                            }

                            // Animate the touched card to flip up
                            touchCard.GetComponent<Animator>().SetBool("flip", false);

                            // Play sfx on card pick
                            FindObjectOfType<AudioManager>().PlayPairedSfx();

                            if (_cardManager.FirstPick == null) {
                                _cardManager.FirstPick = touchCard;
                            } else {
                                _cardManager.SecondPick = touchCard;
                            }

                            if (_cardManager.FirstPick != null &&
                                _cardManager.SecondPick != null &&
                                _cardManager.FirstPick.name ==
                                _cardManager.SecondPick.name) {
                                _cardManager.LockCount++;
                                Debug.Log(_cardManager.LockCount);

                                // Prevent from picking the already paired cards
                                _cardManager.FirstPick.GetComponent<Card>().Locked = true;
                                _cardManager.SecondPick.GetComponent<Card>().Locked = true;

                                _cardManager.FirstPick = null;
                                _cardManager.SecondPick = null;
                            } else if (_cardManager.FirstPick != null &&
                                       _cardManager.SecondPick != null &&
                                       _cardManager.FirstPick.name !=
                                       _cardManager.SecondPick.name) {
                                StartCoroutine(_cardManager.WaitForFlip());
                            }
                        }

                        break;
                }
            }
        }
    }
}
