using System;
using System.Linq;
using Assets.Scripts.GlobalScripts.Managers;
using Assets.Scripts.GlobalScripts.Player;
using UnityEngine;

public class CoreGameBehaviour : MonoBehaviour, ICoreGameBehaviour {

    public delegate void OnMuteGame(string sfx);

    public static event OnMuteGame onMuteGameEvent;

    private bool _pause;

    public bool IsSFXMuted { get; set; }

    public bool IsBGMuted { get; set; }

    private void Awake() {
        IsSFXMuted = false;

        PlayerPrefs.SetInt("IsMuted", 0);

        IsBGMuted = false;

        PlayerPrefs.SetInt("IsBgMuted", 0);
    }

    public virtual void EndGame() {
        FindObjectOfType<AudioManager>().SetVolume("bg_game", 0f);
    }

    public virtual void InitSounds() {
        throw new NotImplementedException();
    }

    public virtual void MuteBackgroundMusic() {
        IsBGMuted = !(PlayerPrefs.GetInt("IsBgMuted") == 1);

        PlayerPrefs.SetInt("IsBgMuted", IsBGMuted ? 1 : 0);

        FindObjectOfType<AudioManager>().SetVolume("bg_game", IsBGMuted ? 0f : 1f);
        FindObjectOfType<AudioManager>().SetVolume("bg_menu", IsBGMuted ? 0f : 1f);

        onMuteGameEvent?.Invoke("bg");
    }

    public virtual void MuteSFX() {
        AudioCollection audioCollection = FindObjectOfType<AudioCollection>();
        if(audioCollection == null) {
            return;
        }

        IsSFXMuted = !(PlayerPrefs.GetInt("IsMuted") == 1);

        PlayerPrefs.SetInt("IsMuted", IsSFXMuted ? 1 : 0);

        GameObject audioManager = audioCollection.gameObject;

        AudioSource[] components = audioManager.GetComponents<AudioSource>() as AudioSource[];
        foreach (var item in audioCollection.audioCollection.Where(audio => audio.Name.StartsWith("sfx"))) {
            foreach (var sfx in components.Where(i => i.clip.name.Equals(item.AudioClip.name))) {
                sfx.mute = IsSFXMuted;
            }
        }

        onMuteGameEvent?.Invoke("sfx");
    }

    public virtual void Pause() {
        if(PlayerPrefs.GetInt("IsBgMuted") != 1) {
            FindObjectOfType<AudioManager>().SetVolume("bg_game", _pause ? 1f : 0.3f);
        }

        _pause = !_pause;

        Time.timeScale = _pause ? 0f : 1f;
    }

    public void SaveScore(float score, BaseScoreHandler.GameType gameType) {
        BaseScoreHandler baseScoreHandler = new BaseScoreHandler();
        baseScoreHandler.AddScore(score, gameType);
    }
}
