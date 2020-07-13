using System;

namespace Assets.Scripts.GlobalScripts.Game {
    public static class Difficulty {
        public enum DifficultyLevel {
            Easy,
            Medium,
            Hard
        }

        /// <summary>
        /// Parses int to level.
        /// </summary>
        /// <param name="level">Must be levels 0(Easy), 1(Medium), 2(Hard)</param>
        /// <returns></returns>
        public static DifficultyLevel ParseLevel(int level) {
            return (DifficultyLevel)Enum.ToObject(typeof(DifficultyLevel), level);
        }
    }
}
