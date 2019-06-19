using System;
using System.Collections;
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

        private void EvaluateStatScore(string category) {
            var score = PlayerPrefs.GetFloat(category);

            Debug.Log("Score for stats:" + category + " -> " + score);

            // Animates stats
            StartCoroutine(AnimateSlider(score, category));
        }

        private static IEnumerator AnimateSlider(float score, string category) {
            float curValue = 0f;
            const float increments = 0.01f;

            StatsCollection[] stats = FindObjectOfType<UIManager>().StatsCollections;

            while (curValue < score) {
                curValue += increments;

                Array.Find(stats, i => i.StatName.Equals(category))
                    .Gauge
                    .value = curValue;

                yield return null;
            }
        }
    }
}
