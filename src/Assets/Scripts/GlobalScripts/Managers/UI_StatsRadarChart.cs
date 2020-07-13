using Assets.Scripts.DataComponent.Model;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Managers {
    public class UI_StatsRadarChart : MonoBehaviour {

        [SerializeField] private Material radarMaterial;
        [SerializeField] private Texture2D radarTexture2D;
        [SerializeField] private CanvasRenderer radarMesh;

        private Stats stats;

        public void SetStats(Stats stats) {
            this.stats = stats;
            stats.OnStatsChanged += Stats_OnStatsChanged;
            UpdateStatsVisual();
        }

        private void Stats_OnStatsChanged(object sender, System.EventArgs e) {
            UpdateStatsVisual();
        }

        private void UpdateStatsVisual() {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[6];
            Vector2[] uv = new Vector2[6];
            int[] triangles = new int[3 * 5];

            float angleIncrement = 360f / 5;
            float radarChartSize = 145f;

            Vector3 languageVertext = Quaternion.Euler(0, 0, -angleIncrement * 0) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(UserStat.GameCategory.Language);
            int languageVertexIndex = 1;
            Vector3 flexibilityVertex = Quaternion.Euler(0, 0, -angleIncrement * 1) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(UserStat.GameCategory.Flexibility);
            int flexibilityVertexIndex = 2;
            Vector3 memoryVertex = Quaternion.Euler(0, 0, -angleIncrement * 2) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(UserStat.GameCategory.Memory);
            int memoryVertexIndex = 3;
            Vector3 problemSolvingVertex = Quaternion.Euler(0, 0, -angleIncrement * 3) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(UserStat.GameCategory.ProblemSolving);
            int problemSolvingVertexIndex = 4;
            Vector3 speedVertex = Quaternion.Euler(0, 0, -angleIncrement * 4) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(UserStat.GameCategory.Speed);
            int speedVertexIndex = 5;

            vertices[0] = Vector3.zero;
            vertices[languageVertexIndex] = languageVertext;
            vertices[flexibilityVertexIndex] = flexibilityVertex;
            vertices[memoryVertexIndex] = memoryVertex;
            vertices[problemSolvingVertexIndex] = problemSolvingVertex;
            vertices[speedVertexIndex] = speedVertex;

            uv[0] = Vector2.zero;
            uv[languageVertexIndex] = Vector2.one;
            uv[flexibilityVertexIndex] = Vector2.one;
            uv[memoryVertexIndex] = Vector2.one;
            uv[problemSolvingVertexIndex] = Vector2.one;
            uv[speedVertexIndex] = Vector2.one;

            triangles[0] = 0;
            triangles[1] = languageVertexIndex;
            triangles[2] = flexibilityVertexIndex;

            triangles[3] = 0;
            triangles[4] = flexibilityVertexIndex;
            triangles[5] = memoryVertexIndex;

            triangles[6] = 0;
            triangles[7] = memoryVertexIndex;
            triangles[8] = problemSolvingVertexIndex;

            triangles[9] = 0;
            triangles[10] = problemSolvingVertexIndex;
            triangles[11] = speedVertexIndex;

            triangles[12] = 0;
            triangles[13] = speedVertexIndex;
            triangles[14] = languageVertexIndex;


            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            radarMesh.SetMesh(mesh);
            radarMesh.SetMaterial(radarMaterial, radarTexture2D);
        }

    }
}
