using System;
using System.Linq;
using Assets.Scripts.Database;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts
{
    public class StatsManager : MonoBehaviour
    {
        private DataAccess _dataAccess;

        private void Start() 
        {
            _dataAccess = new DataAccess();

            Refresh();
        }

        public void Refresh() 
        {
            EvaluateStatScore("Flexibility");
            EvaluateStatScore("Memory");
            EvaluateStatScore("Language");
            EvaluateStatScore("ProblemSolving");
        }

        private void EvaluateStatScore(string category) 
        {
            float total = 0f;
            foreach (var catScore in _dataAccess.SelectAll().Where(i => i.Category.Equals(category))) 
            {
                total += catScore.Score;
            }

            Array.Find(FindObjectOfType<UIManager>().StatsCollections, i => i.StatName.Equals(category))
                .Gauge
                .value = total;
        }
    }
}
