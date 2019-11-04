using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using AOT;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Networking;

namespace Assets.Scripts.GlobalScripts.Managers {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Utility : MonoBehaviour {

        private string _persistentPath;
        private string _streamingAssetsPath;

        [SerializeField] private TextMeshProUGUI _textFilePath;
        [SerializeField] private TextMeshProUGUI _textContent;

        [System.Serializable]
        public class Category {
            public Games category;

            public string last_user;
        }

        [System.Serializable]
        public class Games {
            public string[] flexibility;

            public string[] memory;

            public string[] language;

            public string[] problemsolving;
        }

        private void Start() {
            Category c = new Category();

            StartCoroutine(LoadOnAndroid(category => {
                if(category != null) {
                    c = category;
                }
            }));

            _textContent.SetText(c.last_user);
        }

        private void LoadAsset() {
            string streamingAssetsPath = "Assets/StreamingAssets/UtilityData.json";

            string data = File.ReadAllText(streamingAssetsPath);

            Category c = JsonConvert.DeserializeObject<Category>(data);

            _textContent.SetText(c.last_user);

            // Update
//            var cat = JsonConvert.DeserializeObject<Category>(data);

//            var newCat = new Category {
//                last_user = "test change",
//                category = new Games {
//                    flexibility = cat.category.flexibility,
//                    memory = cat.category.memory,
//                    language = cat.category.language,
//                    problemsolving = cat.category.problemsolving
//                }
//            };
//
//            JsonConvert.PopulateObject(JsonConvert.SerializeObject(newCat), cat);

//            string d = JsonConvert.SerializeObject(cat);
//
//            byte[] bytes = Encoding.ASCII.GetBytes(d);
//
//            File.WriteAllBytes(streamingAssetsPath, bytes);

//            _textContent.SetText("\n\n" + cat.last_user);
        }

        private IEnumerator LoadOnAndroid(System.Action<Category> callback) {
            if (Application.platform == RuntimePlatform.Android) {
                // The path to write to
                _persistentPath = $"{Application.persistentDataPath}/UtilityData.json";

                // The path to get the .json file
                _streamingAssetsPath = Path.Combine(Application.streamingAssetsPath + "/", "UtilityData.json");

                if (_streamingAssetsPath.Contains("://") || _streamingAssetsPath.Contains(":///")) {
                    // Get the compressed jar file containing streaming assets
                    var www = UnityWebRequest.Get(_streamingAssetsPath);
                    yield return www.SendWebRequest();

                    // Cache all the loaded data from the UnityWebRequest to the persistent file
                    File.WriteAllBytes(_persistentPath, www.downloadHandler.data);
                } else {
                    // Cache all the loaded data from the UnityWebRequest to the persistent file
                    File.WriteAllBytes(_persistentPath, File.ReadAllBytes(_streamingAssetsPath));
                }
            } else {
                _persistentPath = @"Assets/StreamingAssets/UtilityData.json";
            }

            // Read the data contents of the persistent file
            string persistentData = File.ReadAllText(_persistentPath);

            Category c = JsonConvert.DeserializeObject<Category>(persistentData);

            callback(c);

            WriteValue("This is the updated. IT WORKS!");

            yield return null;
        }

        private void WriteValue(string lastUser) {
            _persistentPath = Application.platform == RuntimePlatform.Android ? $"{Application.persistentDataPath}/UtilityData.json" : @"Assets/StreamingAssets/UtilityData.json";

            string data = File.ReadAllText(_persistentPath);

            Category oldValue = JsonConvert.DeserializeObject<Category>(data);

            var updatedCategory = new Category {
                last_user = lastUser,
                category = new Games {
                    flexibility = oldValue.category.flexibility,
                    memory = oldValue.category.memory,
                    language = oldValue.category.language,
                    problemsolving = oldValue.category.problemsolving
                }
            };

            JsonConvert.PopulateObject(JsonConvert.SerializeObject(updatedCategory), oldValue);

            string d = JsonConvert.SerializeObject(oldValue);
            
            byte[] bytes = Encoding.ASCII.GetBytes(d);
            
            File.WriteAllBytes(_persistentPath, bytes);

            _textContent.SetText("\n\n" + File.ReadAllText(_persistentPath));
        }
    }
}