using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Assets.Scripts.GlobalScripts.Managers {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class Utility {

        private string _persistentPath;
        private string _streamingAssetsPath;

        [System.Serializable]
        public class Category {
            public Games category { get; set; }

            public string last_user { get; set; }
        }

        [System.Serializable]
        public class Games {
            public string[] flexibility { get; set; }

            public string[] memory { get; set; }

            public string[] language { get; set; }

            public string[] problemsolving { get; set; }
        }
        
        public IEnumerator LoadJson(System.Action<Category> callback) {
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

            Category value = JsonConvert.DeserializeObject<Category>(persistentData);

            callback(value);

            yield return null;
        }

        public IEnumerator LoadJson(System.Action<bool> IsDone) {
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

            IsDone(true);

            yield return null;
        }

        public void WriteValue(string lastUser) {
            _persistentPath = 
                Application.platform == RuntimePlatform.Android ? 
                    $"{Application.persistentDataPath}/UtilityData.json" : @"Assets/StreamingAssets/UtilityData.json";

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
        }
    }
}