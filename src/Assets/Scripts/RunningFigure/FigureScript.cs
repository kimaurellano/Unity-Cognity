using TMPro;
using UnityEngine;

namespace Assets.Scripts.RunningFigure {
    public class FigureScript : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI _text;

        public float MoveSpeed { get; set; }

        public float Number { get; set; }

        private void Start() {
            _text.SetText(Number.ToString());
        }

        private void Update() {
            transform.Translate(Vector3.down * Time.deltaTime * MoveSpeed);
        }

        private void OnBecameInvisible() {
            Destroy(gameObject);
        }
    }
}
