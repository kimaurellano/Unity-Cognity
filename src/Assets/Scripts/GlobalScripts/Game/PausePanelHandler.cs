using UnityEngine;

public class PausePanelHandler : CoreGameBehaviour {

    private void Awake() {
        onMuteGameEvent += UpdateUI;
    }

    private void UpdateUI(string sfx) {
        if (sfx == "bg") {
            IsBGMuted = !IsBGMuted;

            if (IsBGMuted) {
                transform.Find("ButtonMusic").GetChild(0).gameObject.SetActive(true);
                transform.Find("ButtonMusic").GetChild(1).gameObject.SetActive(false);
            } else {
                transform.Find("ButtonMusic").GetChild(0).gameObject.SetActive(false);
                transform.Find("ButtonMusic").GetChild(1).gameObject.SetActive(true);
            }
        } else {
            IsSFXMuted = !IsSFXMuted;

            if (IsSFXMuted) {
                transform.Find("ButtonVolume").GetChild(0).gameObject.SetActive(true);
                transform.Find("ButtonVolume").GetChild(1).gameObject.SetActive(false);
            } else {
                transform.Find("ButtonVolume").GetChild(0).gameObject.SetActive(false);
                transform.Find("ButtonVolume").GetChild(1).gameObject.SetActive(true);
            }
        }
    }
}
