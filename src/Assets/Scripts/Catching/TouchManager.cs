using UnityEngine;

namespace Assets.Scripts.Catching {
    public class TouchManager : MonoBehaviour {

        private void Update() {
            if (Input.touchCount == 1) {
                Touch _touch = Input.GetTouch(0);

                if (_touch.phase == TouchPhase.Stationary || _touch.phase == TouchPhase.Moved) {
                    // get the touch position from the screen touch to world point
                    Vector2 touchedPos =
                        Camera.main.ScreenToWorldPoint(new Vector3(_touch.position.x, _touch.position.y, 0f));
                    // Set the position of the current object to that of the touch
                    transform.position = new Vector2(touchedPos.x, transform.position.y);
                }
            }
        }
    }
}
