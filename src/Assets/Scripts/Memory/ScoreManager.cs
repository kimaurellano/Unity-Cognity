using Assets.Scripts.Database;
using Assets.Scripts.Database.Component;
using Assets.Scripts.Database.Model;
using UnityEngine;

namespace Assets.Scripts.Memory {
    public class ScoreManager {
        private readonly DataAccess _dataAccess;

        // The score as time
        private float _score;

        public ScoreManager() {
            _dataAccess = new DataAccess();
        }

        public void SaveUserScore(float seconds) {
            _dataAccess.Insert(new UserScore {
                Username = PlayerPrefs.GetString("user_info"),
                Score = _score / 1000, // Percentage
                Category = "Memory"
            });
        }
    }
}
