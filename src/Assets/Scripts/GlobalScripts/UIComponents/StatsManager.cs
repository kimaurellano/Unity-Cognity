using System;
using Assets.Scripts.GlobalScripts.UITask;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.UIComponents {
    public class StatsManager : MonoBehaviour {

        private void Start() {
            Refresh();
        }

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

            Array.Find(FindObjectOfType<UIManager>().StatsCollections, i => i.StatName.Equals(category))
                .Gauge
                .value = summary;
        }
    }
}
