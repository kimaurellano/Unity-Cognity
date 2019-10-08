using System;
using System.Collections;
using Assets.Scripts.GlobalScripts.UITask;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.UIComponents {
    public class StatsManager : MonoBehaviour {

        [SerializeField] private Material _radarMaterial;

        [SerializeField] private GameObject _radarMesh;

        private const float MAX_VALUE = 35;
        private float _radarRadius;

        private void Awake() {
            _radarRadius = _radarMesh.GetComponent<RectTransform>().sizeDelta.x;
        }

        /// <summary>
        ///     Loads/Refreshes stats for all category
        /// </summary>
        public void Refresh() {
            EvaluateStatScore("Flexibility");
            EvaluateStatScore("Memory");
            EvaluateStatScore("Language");
            EvaluateStatScore("ProblemSolving");
        }

        /// <summary>
        ///     Loads/Refreshes stat for a specific category
        /// </summary>
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

        public void UpdateRadarChart() {
            Mesh mesh = new Mesh();

            float defaultWidth = 1;
            float defaultHeight = 1;

            Vector3[] vertices = new Vector3[4];
            
            float problemSolvingPercent = PlayerPrefs.GetFloat("ProblemSolving") * MAX_VALUE;
            vertices[0] = new Vector3(-defaultWidth - problemSolvingPercent, -defaultHeight - problemSolvingPercent);

            float memoryPercent = PlayerPrefs.GetFloat("Memory") * MAX_VALUE;
            vertices[1] = new Vector3(-defaultWidth - memoryPercent, defaultHeight + memoryPercent);

            float flexibilityPercent = PlayerPrefs.GetFloat("Flexibility") * MAX_VALUE;
            vertices[2] = new Vector3(defaultWidth + flexibilityPercent, defaultHeight + flexibilityPercent);

            float languagePercent = PlayerPrefs.GetFloat("Language") * MAX_VALUE;
            vertices[3] = new Vector3(defaultWidth + languagePercent, -defaultHeight - languagePercent);

            int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            _radarMesh.GetComponent<CanvasRenderer>().SetMesh(mesh);
            _radarMesh.GetComponent<CanvasRenderer>().SetMaterial(_radarMaterial, null);
        }
    }
}
