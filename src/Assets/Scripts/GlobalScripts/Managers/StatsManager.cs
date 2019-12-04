using System.Linq;
using Assets.Scripts.DataComponent.Database;
using Assets.Scripts.DataComponent.Model;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Managers {
    public class StatsManager : MonoBehaviour {

        [SerializeField] private Material _radarMaterial;

        [SerializeField] private GameObject _radarMesh;

        private UIManager _uiManager;

        private const float MAX_VALUE = 35f;

        private void Awake() {
            _uiManager = FindObjectOfType<UIManager>();
        }

        public void UpdateRadarChart() {
            DatabaseManager databaseManager = new DatabaseManager();

            string loggedUser = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged)?.Username;

            Mesh mesh = new Mesh();

            const float defaultWidth = 1;
            const float defaultHeight = 1;

            // Pentagon has 5 vertices
            Vector3[] vertices = new Vector3[5];

            float problemSolvingProgress = databaseManager.GetUserStat(loggedUser, UserStat.GameCategory.ProblemSolving).Score / 100f;
            TextMeshProUGUI problemSolvingPercentText = (TextMeshProUGUI) _uiManager.GetUI(UIManager.UIType.Text, "problem solving");
            problemSolvingPercentText.SetText(Format(ClampPercent(problemSolvingProgress * 100f)));
            // Bottom left
            vertices[0] = new Vector3(-defaultWidth - problemSolvingProgress * MAX_VALUE, -defaultHeight - problemSolvingProgress * MAX_VALUE);

            float memoryProgress = databaseManager.GetUserStat(loggedUser, UserStat.GameCategory.Memory).Score / 100f;
            TextMeshProUGUI memoryPercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "memory");
            memoryPercentText.SetText(Format(ClampPercent(memoryProgress * 100f)));
            // Top left
            vertices[1] = new Vector3(-defaultWidth - memoryProgress * MAX_VALUE, defaultHeight + memoryProgress * MAX_VALUE);

            float flexibilityProgress = databaseManager.GetUserStat(loggedUser, UserStat.GameCategory.Flexibility).Score / 100f;
            TextMeshProUGUI flexibilityPercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "flexibility");
            flexibilityPercentText.SetText(Format(ClampPercent(flexibilityProgress * 100f)));
            // Top right
            vertices[2] = new Vector3(defaultWidth + flexibilityProgress * MAX_VALUE, defaultHeight + flexibilityProgress * MAX_VALUE);

            float languageProgress = databaseManager.GetUserStat(loggedUser, UserStat.GameCategory.Language).Score / 100f;
            TextMeshProUGUI languagePercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "language");
            languagePercentText.SetText(Format(ClampPercent(languageProgress * 100f)));
            // Bottom right
            vertices[3] = new Vector3(defaultWidth + languageProgress * MAX_VALUE, -defaultHeight - languageProgress * MAX_VALUE);

            float speedProgress = databaseManager.GetUserStat(loggedUser, UserStat.GameCategory.Speed).Score / 100f;
            TextMeshProUGUI speedPercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "speed");
            speedPercentText.SetText(Format(ClampPercent(speedProgress * 100f)));
            // Bottom center
            vertices[4] = new Vector3(0, -defaultHeight - speedProgress * MAX_VALUE);

            // Pentagon is composed of 3 triangles with 3 vertices each
            // 0 1 2, 0 2 3, 0 3 4
            int[] triangles = { 0, 1, 2, 0, 2, 3, 0, 3, 4 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            _radarMesh.GetComponent<CanvasRenderer>().SetMesh(mesh);
            _radarMesh.GetComponent<CanvasRenderer>().SetMaterial(_radarMaterial, null);

            databaseManager.Close();
        }

        private static float ClampPercent(float value) {
            if (value > 100f) {
                value = 100f;
            } else if (value < 0) {
                value = 0f;
            }

            return value;
        }

        private static string Format(float value) {
            return value <= 0 ? "0%" : value.ToString("##.###") + "%";
        }
    }
}
