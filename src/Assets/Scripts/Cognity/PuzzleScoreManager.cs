namespace Assets.Scripts.Cognity {
    public class PuzzleScoreManager {
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
    }
}
