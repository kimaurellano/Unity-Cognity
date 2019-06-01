using System;
using Assets.Scripts.Database.Component;
using Assets.Scripts.GlobalScripts.UITask;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.UIComponents {
    public class StatsManager : MonoBehaviour {
        private DataAccess _dataAccess;

        private void Start() {
            _dataAccess = new DataAccess();

            Refresh();
        }

        public void Refresh() {
            EvaluateStatScore("Flexibility");
            EvaluateStatScore("Memory");
            EvaluateStatScore("Language");
            EvaluateStatScore("ProblemSolving");
        }

        private static void EvaluateStatScore(string category) {
            var summary = PlayerPrefs.GetFloat(category);

            Array.Find(FindObjectOfType<UIManager>().StatsCollections, i => i.StatName.Equals(category))
                .Gauge
                .value = summary;
        }
    }
}
