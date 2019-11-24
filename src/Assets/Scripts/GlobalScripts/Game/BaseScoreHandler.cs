using System.Linq;
using Assets.Scripts.DataComponent.Database;
using Assets.Scripts.DataComponent.Model;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Game {
    public class BaseScoreHandler : DatabaseManager {

        private delegate void OnSetMinMax();

        private static event OnSetMinMax OnSetMinMaxEvent;

        private int _maxValue;
        private int _minValue;

        public int Score { get; private set; }

        public enum GameType {
            Flexibility = 0,
            Memory = 1,
            Language = 2,
            ProblemSolving = 3
        }

        /// <summary>
        /// Set the minimum and maximum possible score of the game
        /// </summary>
        public BaseScoreHandler(int min, int max) {
            _minValue = min;
            _maxValue = max;
        }

        // Conversion of game 0-score x to percentage equivalent
        private static float Normalize(float x, float inMin, float inMax, float outMin, float outMax) {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        public void SaveScore(UserStat.GameCategory category) {
            User user = GetUsers().FirstOrDefault(u => u.IsLogged);

            if (user == null) {
                return;
            }

            UserStat stat = GetUserStat(user.Username, category);
            if(stat == null) {
                return;
            }

            float result = Normalize(Score, _minValue, _maxValue, 0f, 1f);
            result *= 100;
            stat.Score = float.Parse(((stat.Score + result) / 2).ToString("0.0"));
            Debug.Log($"<color=green>Normalized SCORE:{stat.Score}</color>");
            UpdateUserStat(user.Username, stat, category);
        }

        /// <summary>
        /// Time-based game score
        /// </summary>
        /// <param name="minute"></param>
        /// <param name="seconds"></param>
        public void AddScore(float minute, float seconds) {
            // Convert to seconds
            float minToSec = minute * 60f;

            // Add score. We collect score as seconds
            Score += (int)minToSec + (int)seconds;
        }

        /// <summary>
        /// Non Time-based game score
        /// </summary>
        /// <param name="score"></param>
        public void AddScore(float score) {
            Score += (int)score;
        }

        public void DeductScore(float value) {
            Score -= (int)value;
        }
    }
}
