using System;
using System.Linq;
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

        private void EvaluateStatScore(string category) {
            var total = _dataAccess.SelectAll().Where(i => i.Category.Equals(category)).Sum(catScore => catScore.Score);

            Array.Find(FindObjectOfType<UIManager>().StatsCollections, i => i.StatName.Equals(category))
                .Gauge
                .value = total;
        }
    }
}
