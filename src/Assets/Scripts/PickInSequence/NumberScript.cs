using TMPro;
using UnityEngine;

namespace Assets.Scripts.PickInSequence {
    public class NumberScript : MonoBehaviour {

        public delegate void OnNumberPop(int number, Transform transform);

        public static event OnNumberPop OnNumberPopEvent;

        [SerializeField] private TextMeshProUGUI _content;

        private GameManager _gameManager;
        private Collider2D _collider2D;
        private Touch _touch;
        private string _number;

        public string Content { get; set; }

        private void Start() {
            _collider2D = GetComponent<Collider2D>();

            _gameManager = FindObjectOfType<GameManager>();

            _content.SetText(Content);
        }

        private void Update() {
            if (Input.touchCount == 1) {
                _touch = Input.GetTouch(0);

                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);
                if (_touch.phase == TouchPhase.Began) {

                    Collider2D touchPoint = Physics2D.OverlapPoint(touchPos);

                    if (_collider2D.Equals(touchPoint)) {
                        int number = int.Parse(_content.text);

                        OnNumberPopEvent?.Invoke(number, transform);
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
