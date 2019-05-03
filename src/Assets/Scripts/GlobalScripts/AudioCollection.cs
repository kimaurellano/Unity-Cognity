using UnityEngine;

namespace Assets.Scripts.GlobalScripts
{
    [System.Serializable]
    public class AudioCollection
    {
        public string Name;

        public AudioClip AudioClip;

        [Range(0f, 1f)] public float Volume;

        [HideInInspector] public AudioSource AudioSource;
    }
}
