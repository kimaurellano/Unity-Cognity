using System;
using Assets.Scripts.Database.Enum;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Player {
    public class BaseScoreHandler {
        public void SaveScore(float score, Game.GameType gameType) {
            string category = Enum.GetName(typeof(Game.GameType), gameType);

            // Up to 2 decimal places
            double value = Math.Truncate(100 * (score / 1000)) / 100;

            // Record only scores higher than the current
            if (!IsHighScore(category, (float)value)) {
                return;
            }

            // Save score by category
            PlayerPrefs.SetFloat(category, (float)value);
        }

        private static bool IsHighScore(string category, float score) {
            return PlayerPrefs.GetFloat(category) < score;
        }
    }
}
