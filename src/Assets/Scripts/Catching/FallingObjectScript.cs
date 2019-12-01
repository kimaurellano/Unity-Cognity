using UnityEngine;

namespace Assets.Scripts.Catching {
    public class FallingObjectScript : MonoBehaviour {

        public delegate void OnMissed();

        public static event OnMissed OnMissedEvent;

        private bool _missed = true;

        public float MoveSpeed { get; set; }

        private void Update() {
            transform.Translate(Vector3.down * Time.deltaTime * MoveSpeed);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            _missed = false;

            FindObjectOfType<GameManager>().IncreasePoint();

            Destroy(gameObject);
        }

        private void OnBecameInvisible() {
            if (_missed) {
                Debug.Log("Missed.");
                OnMissedEvent?.Invoke();
            }

            Destroy(gameObject);
        }
    }
}
