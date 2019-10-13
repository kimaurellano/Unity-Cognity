using System;
using Assets.Scripts.GlobalScripts.Player;
using UnityEngine;

public class CoreGameBehaviour : MonoBehaviour, ICoreGameBehaviour {

    private bool _pause;

    public virtual void EndGame() {
        throw new NotImplementedException();
    }

    public virtual void InitSounds() {
        throw new NotImplementedException();
    }

    public virtual void MuteBackgroundMusic() {
        throw new NotImplementedException();
    }

    public virtual void MuteFX() {
        throw new NotImplementedException();
    }

    public virtual void Pause() {
        _pause = !_pause;

        Time.timeScale = _pause ? 0f : 1f;
    }

    public void SaveScore(float score, BaseScoreHandler.GameType gameType) {
        BaseScoreHandler baseScoreHandler = new BaseScoreHandler();
        baseScoreHandler.AddScore(score, gameType);
    }
}
