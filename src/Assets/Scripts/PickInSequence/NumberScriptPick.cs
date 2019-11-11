using TMPro;
using UnityEngine;

namespace Assets.Scripts.PickInSequence {
    public class NumberScriptPick : MonoBehaviour {

        public delegate void OnNumberPopPick(int number, Transform transform);

        public static event OnNumberPopPick OnNumberPopPickEvent;

        [SerializeField] private TextMeshProUGUI _content;

        private Collider2D _collider2D;
        private Touch _touch;
        private string _number;

        public string Content { get; set; }

        private void Start() {
            _collider2D = GetComponent<Collider2D>();

            _content.SetText(Content);
        }

        private void Update() {
            if (Input.touchCount == 1) {
                _touch = Input.GetTouch(0);

                Vector2 touchPos = Camera.main.ScreenToWorldPoint(_touch.position);
                if (_touch.phase == TouchPhase.Began) {

                    Collider2D touchPoint = Physics2D.OverlapPoint(touchPos);

                    if (_collider2D.Equals(touchPoint)) {
                        int number = int.Parse(transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text);
                        Debug.Log($"at NumberScriptPick.cs:{number}, {transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text}");
                        OnNumberPopPickEvent?.Invoke(number, transform);
                    }
                }
            }
        }
    }
}
