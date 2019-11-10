using System;
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
        public class Data {
            public Games category { get; set; }

            public string last_user { get; set; }

            public string page { get; set; }

            public int loaded { get; set; }
        }

        [System.Serializable]
        public class Games {
            public string[] flexibility { get; set; }

            public string[] memory { get; set; }

            public string[] language { get; set; }

            public string[] problemsolving { get; set; }
        }

        public IEnumerator LoadJson(Action<Data> callback) {
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

            Data value = JsonConvert.DeserializeObject<Data>(persistentData);

            callback(value);

            yield return 0;
        }

        public IEnumerator LoadJson(Action<bool> IsDone) {
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

            yield return 0;
        }

        public void LoadJson() {
            if (Application.platform == RuntimePlatform.Android) {
                // The path to write to
                _persistentPath = $"{Application.persistentDataPath}/UtilityData.json";

                // The path to get the .json file
                _streamingAssetsPath = Path.Combine(Application.streamingAssetsPath + "/", "UtilityData.json");

                if (_streamingAssetsPath.Contains("://") || _streamingAssetsPath.Contains(":///")) {
                    // Get the compressed jar file containing streaming assets
                    var www = UnityWebRequest.Get(_streamingAssetsPath);

                    // Cache all the loaded data from the UnityWebRequest to the persistent file
                    File.WriteAllBytes(_persistentPath, www.downloadHandler.data);
                } else {
                    // Cache all the loaded data from the UnityWebRequest to the persistent file
                    File.WriteAllBytes(_persistentPath, File.ReadAllBytes(_streamingAssetsPath));
                }
            } else {
                _persistentPath = @"Assets/StreamingAssets/UtilityData.json";
            }
        }

        public void ModifyJson(Data newData) {
            _persistentPath = 
                Application.platform == RuntimePlatform.Android ? 
                    $"{Application.persistentDataPath}/UtilityData.json" : @"Assets/StreamingAssets/UtilityData.json";

            string data = File.ReadAllText(_persistentPath);

            Data oldValue = JsonConvert.DeserializeObject<Data>(data);

            JsonConvert.PopulateObject(JsonConvert.SerializeObject(newData), oldValue);

            string d = JsonConvert.SerializeObject(oldValue);
            
            byte[] bytes = Encoding.ASCII.GetBytes(d);
            
            File.WriteAllBytes(_persistentPath, bytes);
        }

        public Data GetData() {
            return JsonConvert.DeserializeObject<Data>(File.ReadAllText(_persistentPath));
        }
    }
}