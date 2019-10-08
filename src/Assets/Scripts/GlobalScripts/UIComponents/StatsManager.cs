using System;
using System.Collections;
using Assets.Scripts.GlobalScripts.UITask;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.UIComponents {
    public class StatsManager : MonoBehaviour {

        [SerializeField] private Material _radarMaterial;

        [SerializeField] private GameObject _radarMesh;

        private UIManager _uiManager;

        private const float MAX_VALUE = 35;

        private void Awake() {
            _uiManager = FindObjectOfType<UIManager>();
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

            float problemSolvingProgress = PlayerPrefs.GetFloat("ProblemSolving");
            TextMeshProUGUI problemSolvingPercentText = (TextMeshProUGUI) _uiManager.GetUI(UIManager.UIType.Text, "problem solving");
            problemSolvingPercentText.SetText((problemSolvingProgress * 100) + "%");
            vertices[0] = new Vector3(-defaultWidth - problemSolvingProgress * MAX_VALUE, -defaultHeight - problemSolvingProgress * MAX_VALUE);

            float memoryProgress = PlayerPrefs.GetFloat("Memory");
            TextMeshProUGUI memoryPercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "memory");
            memoryPercentText.SetText((memoryProgress * 100) + "%");
            vertices[1] = new Vector3(-defaultWidth - memoryProgress * MAX_VALUE, defaultHeight + memoryProgress * MAX_VALUE);

            float flexibilityProgress = PlayerPrefs.GetFloat("Flexibility");
            TextMeshProUGUI flexibilityPercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "flexibility");
            flexibilityPercentText.SetText((flexibilityProgress * 100) + "%");
            vertices[2] = new Vector3(defaultWidth + flexibilityProgress * MAX_VALUE, defaultHeight + flexibilityProgress * MAX_VALUE);

            float languageProgress = PlayerPrefs.GetFloat("Language");
            TextMeshProUGUI languagePercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "language");
            languagePercentText.SetText((languageProgress * 100) + "%");
            vertices[3] = new Vector3(defaultWidth + languageProgress * MAX_VALUE, -defaultHeight - languageProgress * MAX_VALUE);

            int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            _radarMesh.GetComponent<CanvasRenderer>().SetMesh(mesh);
            _radarMesh.GetComponent<CanvasRenderer>().SetMaterial(_radarMaterial, null);
        }
    }
}
