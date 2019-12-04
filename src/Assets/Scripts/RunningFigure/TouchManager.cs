using UnityEngine;

namespace Assets.Scripts.RunningFigure {
    public class TouchManager : MonoBehaviour {
        public delegate void OnImageCatch();

        public static event OnImageCatch OnImageCatchEvent;

        private void Update() {
            if (Input.touchCount > 0) {
                Touch _touch = Input.GetTouch(0);

                if (_touch.phase == TouchPhase.Began) {
                    Vector2 touchedPos = Camera.main.ScreenToWorldPoint(_touch.position);

                    if (GetComponent<Collider2D>().Equals(Physics2D.OverlapPoint(touchedPos))) {
                        GameManager manager = FindObjectOfType<GameManager>();
                        string spriteName = Physics2D.OverlapPoint(touchedPos).GetComponent<SpriteRenderer>().sprite
                            .name; 
                        float spriteNumber = Physics2D.OverlapPoint(touchedPos).GetComponent<FigureScript>().Number;
                        if (manager.RunningImage.sprite.name == spriteName &&
                            (int) manager.Number == (int) spriteNumber) {
                            OnImageCatchEvent?.Invoke();

                            Debug.Log("Matched");
                            Destroy(gameObject);
                        } else {
                            manager.DecreaseLife();

                            Debug.Log("Wrong match");
                        }
                    }
                }
            }
        }
    }
}
