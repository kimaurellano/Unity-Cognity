using System;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Player {
    public class BaseScoreHandler {
        public enum GameType {
            Flexibility = 1,
            Memory = 2,
            ProblemSolving = 4,
            Language = 3
        }

        public void AddScore(float score, GameType gameType) {
            string category = Enum.GetName(typeof(GameType), gameType);

            // Up to 2 decimal places
            double newScore = Math.Truncate(100 * (score / 1000)) / 100;
            Debug.Log("new score:" + newScore);

            float oldScore = PlayerPrefs.GetFloat(category);
            Debug.Log("old score:" + oldScore);

            // Add new score to the current by category
            PlayerPrefs.SetFloat(category, (float)newScore + oldScore);
            Debug.Log("total score:" + ((float)newScore + oldScore));
        }
    }
}
