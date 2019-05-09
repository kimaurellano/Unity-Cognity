using Assets.Scripts.Database;
using Assets.Scripts.Database.Model;
using UnityEngine;

namespace Assets.Scripts.GrammarQuiz.Mono
{
    public class ScoreManager
    {
        private readonly DataAccess _dataAccess;

        // The score as time
        private float _score;

        public ScoreManager() {
            this._dataAccess = new DataAccess();
        }

        public void SaveUserScore(float score) 
        {
            _dataAccess.Insert(new UserScore 
            {
                Username = PlayerPrefs.GetString("user_info"),
                Score = score / 1000, // Percentage
                Category = "Language"
            });
        }
    }
}
