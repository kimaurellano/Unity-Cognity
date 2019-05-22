using System;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.UIComponents {
    [Serializable]
    public class AudioCollection {
        public AudioClip AudioClip;

        [HideInInspector] public AudioSource AudioSource;

        public string Name;

        [Range(0f, 1f)] public float Volume;
    }
}
