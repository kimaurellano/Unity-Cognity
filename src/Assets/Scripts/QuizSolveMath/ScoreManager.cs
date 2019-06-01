using System;
using Assets.Scripts.Database.Component;
using Assets.Scripts.Database.Enum;
using Assets.Scripts.Database.Interface;
using Assets.Scripts.Database.Model;
using UnityEngine;

namespace Assets.Scripts.QuizSolveMath {
    public class ScoreManager : IScoreHandler {
        private readonly DataAccess _dataAccess;

        // The score as time
        private float _score;

        public ScoreManager() {
            _dataAccess = new DataAccess();
        }

        public void SaveScore(float score, Game.GameType gameType) {
            var value = Math.Truncate(100 * (score / 1000)) / 100;
            _dataAccess.Insert(new UserScore {
                Username = PlayerPrefs.GetString("user_info"),
                Score = (float)value, // Percentage
                Category = "ProblemSolving"
            });
        }
    }
}
