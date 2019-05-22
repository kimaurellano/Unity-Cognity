using UnityEngine;

namespace Assets.Scripts.Cognity {
    public class ScoreManager {
        public float TotalScore { get; private set; }

        /// <summary>
        ///     Add score for every level iteration
        /// </summary>
        public void AddScore(float minute, float seconds) {
            // Convert to seconds
            float minToSec = minute * 60f;

            // Add score. We collect score as seconds
            TotalScore += minToSec + seconds;
        }

        /// <summary>
        ///     Display score as Min:Sec
        /// </summary>
        /// <returns>Converted seconds to a corresponding minute</returns>
        public string GetTotalScore() {
            float seconds = TotalScore / 60f;

            seconds = Mathf.Repeat(seconds, 1.0f) * 60f;
            int minutes = (int) TotalScore / 60;

            return $"{minutes:00}:{seconds:00}";
        }
    }
}
