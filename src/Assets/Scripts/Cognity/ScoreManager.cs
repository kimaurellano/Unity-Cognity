using UnityEngine;
using System;
using Assets.Scripts.Cognity.Database;
using Assets.Scripts.Cognity.Database.Model;
using System.Collections.Generic;

namespace Assets.Scripts.Cognity
{
    public class ScoreManager
    {
        private DataAccess _dataAccess;

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

        public string GetTotalScore()
        {
            float seconds = _score / 60f;

            seconds = Mathf.Repeat(seconds, 1.0f) * 60f;
            int minutes = (int)_score / 60;

            // After converting seconds to a corresponding minute
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public List<KeyValuePair<string, float>> GetUserScoreList()
        {
            List<KeyValuePair<string, float>> ScoreList = new List<KeyValuePair<string, float>>();
            int listCount = 1;
            foreach (var score in _dataAccess.SelectAll())
            {
                // Get only 5 scores
                if (listCount++ > 5)
                {
                    break;
                }

                ScoreList.Add(new KeyValuePair<string, float>(score.Username, score.Score));
            }

            return ScoreList;
        }

        public void SaveUserScore(string username)
        {
            _dataAccess.Insert(new UserScore
            {
                Username = username,
                Score = _score
            });
        }
    }
}