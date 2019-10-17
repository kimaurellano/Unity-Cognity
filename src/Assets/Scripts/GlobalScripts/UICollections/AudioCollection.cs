using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioCollection : MonoBehaviour {
    public List<Audio> audioCollection = new List<Audio>();

    [Serializable]
    public class Audio {
        public string Name;

        public string[] Games;

        public AudioClip AudioClip;

        [HideInInspector] public AudioSource AudioSource;

        [Range(0f, 1f)] public float Volume;

        public bool playOnAwake;

        public bool loop;
    }
}
