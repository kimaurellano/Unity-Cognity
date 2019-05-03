using UnityEngine;

namespace Assets.Scripts.Cognity {
    public class AnimationEvent : MonoBehaviour {
        [SerializeField] private Transform _hiddenPanel;
        [SerializeField] private Transform _displayedPanel;

        public void Transition(){
            _hiddenPanel.gameObject.SetActive(false);
            _displayedPanel.gameObject.SetActive(true);
        }
    }
}