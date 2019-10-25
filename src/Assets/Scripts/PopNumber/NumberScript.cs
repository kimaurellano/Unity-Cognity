using TMPro;
using UnityEngine;

namespace Assets.Scripts.PopNumber {
    public class NumberScript : MonoBehaviour {

        public delegate void OnNumberPop();

        public static event OnNumberPop OnNumberPopEvent;

        [SerializeField] private TextMeshProUGUI content;

        private GameManager _gameManager;
        private Collider2D _collider2D;
        private Touch _touch;

        public string Content { get; set; }
        public float MoveSpeed { get; set; }

        private void Start() {
            _collider2D = GetComponent<Collider2D>();

            _gameManager = FindObjectOfType<GameManager>();

            content.SetText(Content);
        }

        private void Update() {
            transform.Translate(Vector3.down * Time.deltaTime * MoveSpeed);

            if(Input.touchCount == 1) {
                _touch = Input.GetTouch(0);

                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);
                if (_touch.phase == TouchPhase.Began) {

                    Collider2D touchPoint = Physics2D.OverlapPoint(touchPos);

                    if (_collider2D.Equals(touchPoint)) {
                        int number = int.Parse(transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text);
                        Debug.Log("Number to touch:" + _gameManager.GetCurrentAnswer());
                        if (_gameManager.GetCurrentAnswer() != number) {
                            return;
                        }

                        OnNumberPopEvent?.Invoke();

                        Destroy(touchPoint.gameObject);
                    }
                }
            }
        }

        private void OnBecameInvisible() {
            Destroy(gameObject);
        }
    }
}
