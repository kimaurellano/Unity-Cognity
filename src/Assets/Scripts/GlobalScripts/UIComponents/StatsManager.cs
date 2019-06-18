using System;
using Assets.Scripts.GlobalScripts.UITask;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.UIComponents {
    public class StatsManager : MonoBehaviour {

        public void Refresh() {
            EvaluateStatScore("Flexibility");
            EvaluateStatScore("Memory");
            EvaluateStatScore("Language");
            EvaluateStatScore("ProblemSolving");
        }

        public void Refresh(string category) {
            EvaluateStatScore(category);
        }

        private static void EvaluateStatScore(string category) {
            var summary = PlayerPrefs.GetFloat(category);

            Debug.Log("Score for stats:" + category + " -> " + summary);

            StatsCollection[] stats = FindObjectOfType<UIManager>().StatsCollections;

            Array.Find(stats, i => i.StatName.Equals(category))
                .Gauge
                .value = summary;
        }
    }
}
