using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PopNumber {
    public class NumberScript : MonoBehaviour {

        public float MoveSpeed { get; set; }

        private void Update() {
            transform.Translate(Vector3.down * Time.deltaTime * MoveSpeed);
        }

        private void OnBecameInvisible() {
            Destroy(gameObject);
        }
    }
}
