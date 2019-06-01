using UnityEngine;

namespace Assets.Scripts.Memory {
    public class Card : MonoBehaviour {
        [SerializeField] private bool _locked;

        public bool Locked {
            get => _locked;
            set => _locked = value;
        }
    }
}
