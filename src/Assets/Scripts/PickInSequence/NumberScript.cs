using TMPro;
using UnityEngine;

namespace Assets.Scripts.PickInSequence {
    public class NumberScript : MonoBehaviour {

        public delegate void OnNumberPop(int number);

        public static event OnNumberPop OnNumberPopEvent;

        private GameManager _gameManager;
        private Collider2D _collider2D;
        private Touch _touch;
        private string _number;

        private void Start() {
            _collider2D = GetComponent<Collider2D>();

            _gameManager = FindObjectOfType<GameManager>();
        }

        private void Update() {
            if (Input.touchCount == 1) {
                _touch = Input.GetTouch(0);

                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);
                if (_touch.phase == TouchPhase.Began) {

                    Collider2D touchPoint = Physics2D.OverlapPoint(touchPos);

                    if (_collider2D.Equals(touchPoint)) {
                        int number = int.Parse(transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text);

                        OnNumberPopEvent?.Invoke(number);

                        Destroy(touchPoint.gameObject);
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            gameObject.transform.position = new Vector3(
                Random.Range(_gameManager.MinX, _gameManager.MaxX),
                Random.Range(_gameManager.MinY, _gameManager.MaxY),
                0f);
        }
    }
}
