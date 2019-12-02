using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataComponent.Database;
using Assets.Scripts.DataComponent.Model;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;

namespace Assets.Scripts.GlobalScripts.Game {
    public class RemarkManager : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI _waitText;
        [SerializeField] private Button _btnContinue;
        [SerializeField] private Sprite _circleSprite;

        private RectTransform _graphContainer;
        private List<float> _values;
        private int _seconds = 5;

        private void Awake() {
            _values = new List<float>();

            // Programmatically add LoadNextScene as OnClick of button continue
            _btnContinue.onClick.AddListener(FindObjectOfType<CoreGameBehaviour>().LoadNextScene);

            _graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();

            StartCoroutine(WindowClose());
        }

        private IEnumerator WindowClose() {
            while (_seconds > 0) {
                _seconds--;
                _waitText.SetText(_seconds.ToString());
                yield return new WaitForSecondsRealtime(1f);
            }

            FindObjectOfType<CoreGameBehaviour>().LoadNextScene();
        }

        private GameObject CreateCircle(Vector2 anchoredPosition) {
            GameObject dotInstance = new GameObject("circle", typeof(Image));
            dotInstance.transform.SetParent(_graphContainer, false);
            dotInstance.GetComponent<Image>().sprite = _circleSprite;
            RectTransform rectTransform = dotInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(20, 20);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);

            return dotInstance;
        }

        public void ShowGraph(UserStat.GameCategory category) {
            transform.gameObject.SetActive(true);

            DatabaseManager databaseManager = new DatabaseManager();
            string loggedUser = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged)?.Username;

            _values.Clear();

            foreach (var userScoreHistory in databaseManager.GetScoreHistory(loggedUser).Where(i => i.Category == (int)category).ToList()) {
                _values.Add(userScoreHistory.SessionScore);
            }

            float graphHeight = _graphContainer.sizeDelta.y;
            const float yMaximum = 100f;
            const float xSize = 100f;

            GameObject lastCircleGameObject = null;

            // Show only 5 session scores on remarks
            for (int i = 0; i < 5; i++) {
                float xPosition = xSize + i * xSize;
                float yPosition = (_values[i] / yMaximum) * graphHeight;
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
                if(lastCircleGameObject != null) {
                    CreateDotConnection(
                        lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                        circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                }

                lastCircleGameObject = circleGameObject;
            }
        }

        private void CreateDotConnection(Vector2 dotPosA, Vector2 dotPosB) {
            GameObject dotConnection = new GameObject("dotConnection", typeof(Image));
            dotConnection.transform.SetParent(_graphContainer, false);
            dotConnection.GetComponent<Image>().color = Color.blue;
            RectTransform rectTransform = dotConnection.GetComponent<RectTransform>();
            Vector2 dir = (dotPosB - dotPosA).normalized;
            float dist = Vector2.Distance(dotPosA, dotPosB);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(dist, 3f);
            rectTransform.anchoredPosition = dotPosA + dir * dist * 0.5f;
            rectTransform.localEulerAngles = new Vector3(0,0, UtilsClass.GetAngleFromVectorFloat(dir));
        }
    }
}
