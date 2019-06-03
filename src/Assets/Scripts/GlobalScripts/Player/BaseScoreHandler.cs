using System;
using Assets.Scripts.Database.Enum;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Player {
    public class BaseScoreHandler : MonoBehaviour {
        public void AddScore(float score, Game.GameType gameType) {

            string category = Enum.GetName(typeof(Game.GameType), gameType);

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
