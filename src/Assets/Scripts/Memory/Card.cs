using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Memory
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private bool _locked;

        public bool Locked { get => _locked; set => _locked = value; }

        private void Start()
        {
            //StartCoroutine(FaceUp());
        }

        private IEnumerator FaceUp()
        {
            yield return new WaitForSeconds(5f);

            GetComponent<Animator>().SetBool("flip", true);
        }
    }
}
