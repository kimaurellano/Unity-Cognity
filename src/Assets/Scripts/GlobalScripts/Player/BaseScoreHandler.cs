using System;
using UnityEngine;
using Type = Assets.Scripts.GlobalScripts.Game.Type;

namespace Assets.Scripts.GlobalScripts.Player {
    public class BaseScoreHandler : MonoBehaviour {
        public void AddScore(float score, Type.GameType gameType) {
            string category = Enum.GetName(typeof(Type.GameType), gameType);

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
