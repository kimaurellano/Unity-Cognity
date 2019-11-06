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

            float defaultWidth = 1;
            float defaultHeight = 1;

            Vector3[] vertices = new Vector3[4];

            float problemSolvingProgress = databaseManager.GetUserStat(loggedUser, UserStat.GameCategory.ProblemSolving).Score / 100f;
            TextMeshProUGUI problemSolvingPercentText = (TextMeshProUGUI) _uiManager.GetUI(UIManager.UIType.Text, "problem solving");
            problemSolvingPercentText.SetText((problemSolvingProgress * 100f).ToString("##.###") + "%");
            // Bottom left
            vertices[0] = new Vector3(-defaultWidth - problemSolvingProgress * MAX_VALUE, -defaultHeight - problemSolvingProgress * MAX_VALUE);

            float memoryProgress = databaseManager.GetUserStat(loggedUser, UserStat.GameCategory.Memory).Score / 100f;
            TextMeshProUGUI memoryPercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "memory");
            memoryPercentText.SetText((memoryProgress * 100f).ToString("##.###") + "%");
            // Top left
            vertices[1] = new Vector3(-defaultWidth - memoryProgress * MAX_VALUE, defaultHeight + memoryProgress * MAX_VALUE);

            float flexibilityProgress = databaseManager.GetUserStat(loggedUser, UserStat.GameCategory.Flexibility).Score / 100f;
            TextMeshProUGUI flexibilityPercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "flexibility");
            flexibilityPercentText.SetText((flexibilityProgress * 100f).ToString("##.###") + "%");
            // Top right
            vertices[2] = new Vector3(defaultWidth + flexibilityProgress * MAX_VALUE, defaultHeight + flexibilityProgress * MAX_VALUE);

            float languageProgress = databaseManager.GetUserStat(loggedUser, UserStat.GameCategory.Language).Score / 100f;
            TextMeshProUGUI languagePercentText = (TextMeshProUGUI)_uiManager.GetUI(UIManager.UIType.Text, "language");
            languagePercentText.SetText((languageProgress * 100f).ToString("##.###") + "%");
            // Bottom right
            vertices[3] = new Vector3(defaultWidth + languageProgress * MAX_VALUE, -defaultHeight - languageProgress * MAX_VALUE);

            int[] triangles = { 0, 1, 2, 0, 2, 3 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            _radarMesh.GetComponent<CanvasRenderer>().SetMesh(mesh);
            _radarMesh.GetComponent<CanvasRenderer>().SetMaterial(_radarMaterial, null);

            databaseManager.Close();
        }
    }
}
