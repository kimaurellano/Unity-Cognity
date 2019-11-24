using System.Linq;
using Assets.Scripts.GlobalScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GlobalScripts.Game {
    public class PausePanelHandler : CoreGameBehaviour {

        private CoreGameBehaviour[] _coreGameBehaviour;
        private Animator _showDialogAnim;

        private bool _isPaused;
        private bool _overrided;

        private void Start() {
            Debug.Log("Pause panel init");

            _coreGameBehaviour = Resources.FindObjectsOfTypeAll<CoreGameBehaviour>();
            if (_coreGameBehaviour == null) {
                return;
            }

            // Auto attaches resume capability for all game types
            foreach (var item in _coreGameBehaviour.Where(i => !i.name.Equals(transform.name))) {
                transform.Find("ButtonResume").GetComponent<Button>().onClick.AddListener(item.Pause);
                transform.Find("ButtonTryAgain").GetComponent<Button>().onClick.AddListener(item.Retry);
            }

            OnMuteGameEvent += UpdateUI;
            OnPauseGameEvent += ShowDialog;
            OnQuitGameEvent += RemoveAttached;
            OnEndGameEvent += RemoveAttached;

            AudioManager.OnAllAudioOverrideEvent += AudioOverride;
        }

        private void RemoveAttached() {
            foreach (var item in _coreGameBehaviour.Where(i => !i.name.Equals(transform.name))) {
                transform.Find("ButtonResume").GetComponent<Button>().onClick.RemoveListener(item.Pause);
                transform.Find("ButtonTryAgain").GetComponent<Button>().onClick.RemoveListener(item.Retry);
            }
        }

        private void UpdateUI(string sfx) {
            if (_overrided) {
                transform.Find("ButtonMusic").GetChild(0).gameObject.SetActive(true);
                transform.Find("ButtonMusic").GetChild(1).gameObject.SetActive(false);

                transform.Find("ButtonVolume").GetChild(0).gameObject.SetActive(true);
                transform.Find("ButtonVolume").GetChild(1).gameObject.SetActive(false);

                return;
            }

            if (sfx == "bg") {
                if (IsBgMuted) {
                    transform.Find("ButtonMusic").GetChild(0).gameObject.SetActive(true);
                    transform.Find("ButtonMusic").GetChild(1).gameObject.SetActive(false);
                } else {
                    transform.Find("ButtonMusic").GetChild(0).gameObject.SetActive(false);
                    transform.Find("ButtonMusic").GetChild(1).gameObject.SetActive(true);
                }
                IsBgMuted = !IsBgMuted;
            } else {
                if (IsSfxMuted) {
                    transform.Find("ButtonVolume").GetChild(0).gameObject.SetActive(true);
                    transform.Find("ButtonVolume").GetChild(1).gameObject.SetActive(false);
                } else {
                    transform.Find("ButtonVolume").GetChild(0).gameObject.SetActive(false);
                    transform.Find("ButtonVolume").GetChild(1).gameObject.SetActive(true);
                }
                IsSfxMuted = !IsSfxMuted;
            }
        }

        private void ShowDialog() {
            foreach (var item in Resources.FindObjectsOfTypeAll<PausePanelHandler>()) {
                _showDialogAnim = item.GetComponent<Animator>();
            }

            _isPaused = !_isPaused;

            _showDialogAnim.Play(_isPaused ? "ShowPause" : "HidePause");
        }

        private void AudioOverride() {
            _overrided = !_overrided;
        }
    }
}
