using System;
using System.Collections.Generic;
using UnityEngine;
using static AudioCollection.Audio;

public class AudioCollection : MonoBehaviour {
    public List<Audio> audioCollection = new List<Audio>();

    [Serializable]
    public class Audio {
        public AudioClip AudioClip;

        [HideInInspector] public AudioSource AudioSource;

        [Range(0f, 1f)] public float Volume;

        public enum GameType {
            None,
            GameCognity,
            GameMemory,
            GameQuizMath,
            GameQuizSolveMath,
            GameQuizGrammar,
            GamePicturePuzzle
        }

        public GameType gameType;

        public enum UseType {
            MainMenu,
            InGame,
            Interactable,
        }

        public UseType useType;
    }

    public AudioSource PlaySource(GameType gameType, UseType useType) {
        return audioCollection.Find(c => c.gameType == gameType && c.useType == useType).AudioSource;
    }
}
