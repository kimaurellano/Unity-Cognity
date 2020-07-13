using TMPro;
using UnityEngine;

namespace Assets.Scripts.PopNumber {
    public class NumberScriptPop : MonoBehaviour {

        public delegate void OnBottomHit(int number);
        public delegate void OnNumberPop(int number);

        public static event OnBottomHit OnBottomHitEvent;
        public static event OnNumberPop OnNumberPopEvent;

        private TMP_Text _content;
        private Collider2D _collider2D;
        private Touch _touch;

        public string Content { get; set; }
        public float MoveSpeed { get; set; }

        private void Start() {
            _collider2D = GetComponent<Collider2D>();

            _content = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();

            // Display number
            _content.SetText(Content);

            Debug.Log($"<color=green>Text: {Content}</color>");
        }

        private void Update() {
            transform.Translate(Vector3.down * Time.deltaTime * MoveSpeed);

            if(Input.touchCount == 1) {
                _touch = Input.GetTouch(0);

                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);
                if (_touch.phase == TouchPhase.Began) {

                    Collider2D touchPoint = Physics2D.OverlapPoint(touchPos);

                    if (_collider2D.Equals(touchPoint)) {
                        OnNumberPopEvent?.Invoke(int.Parse(Content));

                        Destroy(touchPoint.gameObject);
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.name.Equals("Bottom")) {
               // Tell what number has hit the bottom
               OnBottomHitEvent?.Invoke(int.Parse(Content));
            }

            Destroy(gameObject);
        }
    }
}
