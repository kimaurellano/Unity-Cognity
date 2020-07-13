using System.Security.Cryptography;
using Assets.Scripts.DataComponent.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GlobalScripts.Game {
    public class ScoreDataHolder : MonoBehaviour {

        public GameObject GameObjectHolder { get; set; }

        public UserStat.GameCategory category { get; set; }

        public string ParentScene { get; set; }

        public int MinScore { get; set; }
        
        public int MaxScore { get; set; }

        private void Start() {
            // This gameobject is created on a game scene
            // that can be destroyed upon loading remark
            // scene.
            //
            // To avoid losing this gameobject that
            // holds score data of the current game
            DontDestroyOnLoad(GameObjectHolder);

            SceneManager.activeSceneChanged += DestroyThis;
        }

        // Avoid retaining instance to scenes other than Remark scene
        private void DestroyThis(Scene current, Scene next) {
            SceneManager.activeSceneChanged -= DestroyThis;

            if (SceneManager.GetActiveScene().name != ParentScene) {
                Destroy(GameObjectHolder);
            }
        }
    }
}
