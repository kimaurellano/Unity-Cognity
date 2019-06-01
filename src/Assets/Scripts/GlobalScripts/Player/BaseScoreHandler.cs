using System;
using Assets.Scripts.Database.Component;
using Assets.Scripts.Database.Enum;
using Assets.Scripts.Database.Model;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Player {
    public class BaseScoreHandler {
        public void SaveScore(float score, Game.GameType gameType) {

            // Up to 2 decimal places
            double value = Math.Truncate(100 * (score / 1000)) / 100;

            DataAccess dataAccess = new DataAccess();
            dataAccess.Insert(new UserScore {
                Username = PlayerPrefs.GetString("user_info"),
                Score = (float)value, // Percentage
                Category = Enum.GetName(typeof(Game.GameType), gameType)
            });
        }
    }
}
