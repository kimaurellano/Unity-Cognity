using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.Database.Model;

namespace Assets.Scripts.Cognity
{
    public class ScoreManager
    {
        private readonly DataAccess _dataAccess;

        // The score as time
        private float _score;

        public ScoreManager()
        {
            this._dataAccess = new DataAccess();
        }

        public void AddScore(float minute, float seconds)
        {
            // Convert to seconds
            float minToSec = minute * 60f;

            // Add score. We collect score as seconds
            _score += minToSec + seconds;
        }

        /// <summary>
        /// Display score as Min:Sec
        /// </summary>
        /// <returns></returns>
        public string GetTotalScore()
        {
            float seconds = _score / 60f;

            seconds = Mathf.Repeat(seconds, 1.0f) * 60f;
            int minutes = (int)_score / 60;

            // After converting seconds to a corresponding minute
            return $"{minutes:00}:{seconds:00}";
        }

        public List<KeyValuePair<string, float>> GetUserScoreList()
        {
            List<KeyValuePair<string, float>> scoreList = new List<KeyValuePair<string, float>>();
            int listCount = 1;
            foreach (var score in _dataAccess.SelectAll())
            {
                // Get only 5 scores
                if (listCount++ > 5)
                {
                    break;
                }

                scoreList.Add(new KeyValuePair<string, float>(score.Username, score.Score));
            }

            return scoreList;
        }

        /// <summary>
        /// Save score as Seconds
        /// </summary>
        /// <param name="username"></param>
        public void SaveUserScore(string username)
        {
            _dataAccess.Insert(new UserScore
            {
                Username = username,
                Score = (_score / 1000), // Percentage
                Category = "Flexibility"
            });
        }
    }
}