using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Memory
{
    public class TouchManager : MonoBehaviour
    {
        private Touch _touch;
        [SerializeField] private CardList _cardList;

        private void Start()
        {
            _cardList = GameObject.Find("CardManager").GetComponent<CardList>();
        }

        private void Update()
        {
            if (Input.touchCount == 1)
            {
                _touch = Input.GetTouch(0);

                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);

                switch (_touch.phase)
                {
                    case TouchPhase.Began:
                        if (GetComponent<Collider2D>().Equals(Physics2D.OverlapPoint(touchPos)))
                        {
                            // Get the transform of the touched card
                            var touchCard = Physics2D.OverlapPoint(touchPos).transform;

                            if (touchCard.GetComponent<Card>().Locked)
                            {
                                return;
                            }

                            // Animate the touched card to flip up
                            touchCard.GetComponent<Animator>().SetBool("flip", false);

                            // Two same card picked
                            if (_cardList.FirstPick != null &&
                                touchCard.name == _cardList.FirstPick.name)
                            {
                                // Tell when to display success panel
                                FindObjectOfType<CardList>().AddLockedCard();

                                FindObjectOfType<Assets.Scripts.GlobalScripts.AudioManager>().PlayPairedSfx();

                                // Avoid picking the paired cards
                                touchCard.GetComponent<Card>().Locked = true;
                                _cardList.FirstPick.GetComponent<Card>().Locked = true;

                                // Reset picking
                                _cardList.FirstPick = null;
                                touchCard = null;
                            }
                            else if (_cardList.FirstPick != null && touchCard.name != _cardList.FirstPick.name)
                            {
                                touchCard.GetComponent<Animator>().SetBool("flip", true);
                                _cardList.FirstPick.GetComponent<Animator>().SetBool("flip", true);

                                // Remove the first picked
                                _cardList.FirstPick = null;
                            }
                            else if (_cardList.FirstPick == null)
                            {
                                // Remember last picked
                                _cardList.FirstPick = touchCard;
                            }
                        }
                        break;
                }
            }
        }

        private string CompareName(string first, string second)
        {
            return null;
        }
    }
}
