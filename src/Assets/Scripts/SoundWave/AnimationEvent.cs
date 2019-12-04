using Assets.Scripts.GlobalScripts.Managers;
using UnityEngine;

namespace Assets.Scripts.SoundWave {
    public class AnimationEvent : MonoBehaviour {

        private bool _hasError;

        public void PlaySoundInEvent() {
            FindObjectOfType<AudioManager>().PlayClip(_hasError ? "sfx_incorrect" : "sfx_correct");
        }

        public void SetError(bool hasError) {
            _hasError = hasError;
        }
    }
}
