using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelHandler : CoreGameBehaviour {

    private Animator showDialogAnim;

    private bool _isPaused;

    private void Start() {
        if (!(Resources.FindObjectsOfTypeAll<CoreGameBehaviour>() is CoreGameBehaviour[] gameManager)) {
            return;
        }

        // Auto attaches resume capability for all game types
        foreach (var item in gameManager.Where(i => !i.name.Equals(transform.name))) {
            transform.Find("ButtonResume").GetComponent<Button>().onClick.AddListener(item.Pause);
        }

        showDialogAnim = GetComponent<Animator>();
        OnMuteGameEvent += UpdateUI;
        OnPauseGameEvent += ShowDialog;
    }

    public override void MuteBackgroundMusic() {
        base.MuteBackgroundMusic();
    }

    public override void MuteSFX() {
        base.MuteSFX();
    }

    private void UpdateUI(string sfx) {
        if (sfx == "bg") {
            if (IsBGMuted) {
                transform.Find("ButtonMusic").GetChild(0).gameObject.SetActive(true);
                transform.Find("ButtonMusic").GetChild(1).gameObject.SetActive(false);
            } else {
                transform.Find("ButtonMusic").GetChild(0).gameObject.SetActive(false);
                transform.Find("ButtonMusic").GetChild(1).gameObject.SetActive(true);
            }
            IsBGMuted = !IsBGMuted;
        } else {
            if (IsSFXMuted) {
                transform.Find("ButtonVolume").GetChild(0).gameObject.SetActive(true);
                transform.Find("ButtonVolume").GetChild(1).gameObject.SetActive(false);
            } else {
                transform.Find("ButtonVolume").GetChild(0).gameObject.SetActive(false);
                transform.Find("ButtonVolume").GetChild(1).gameObject.SetActive(true);
            }
            IsSFXMuted = !IsSFXMuted;
        }
    }

    private void ShowDialog() {
        _isPaused = !_isPaused;

        showDialogAnim.Play(_isPaused ? "ShowPause" : "HidePause");
    }
}
