using System;
using UnityEngine;

namespace Assets.Scripts.RunningFigure {
    [Serializable]
    public class LevelScript {
        public enum Level {
            Easy,
            Medium,
            Hard
        }

        public Level GameLevel;

        public GameObject[] _prefabs;

        public float RangeFrom;

        public float RangeTo;
    }
}
