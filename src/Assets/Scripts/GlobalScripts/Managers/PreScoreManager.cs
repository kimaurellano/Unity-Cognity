namespace Assets.Scripts.GlobalScripts.Managers {
    public class PreScoreManager {
        public float TotalTimeScore { get; private set; }

        public float TotalScore { get; private set; }

        /// <summary>
        ///  Add score for every level iteration
        /// </summary>
        public void AddScore(float minute, float seconds) {
            // Convert to seconds
            float minToSec = minute * 60f;

            // Add score. We collect score as seconds
            TotalTimeScore += minToSec + seconds;
        }

        public void AddScore(float score) {
            TotalScore += score;
        }
    }
}
