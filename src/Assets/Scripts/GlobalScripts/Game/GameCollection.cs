using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Game {
    public class GameCollection : MonoBehaviour {
        [System.Serializable]
        public class Collection {
            public string CategoryName;

            public string[] Games;
        }

        private static GameCollection _gameCollectionInstance;

        [SerializeField] private Collection[] _gameCollection;

        public int PlayedGames { get; private set; }
        public int Loaded { get; set; }
        public Collection[] GameCollections { get => _gameCollection; set => _gameCollection = value; }

        private void Awake() {
            if (_gameCollectionInstance != null) {
                Destroy(gameObject);
            } else {
                _gameCollectionInstance = this;
            }

            DontDestroyOnLoad(this);
        }

        public string GetNextScene() {
            PlayedGames++;

            Loaded++;
            // We only have 4 game categories
            if (Loaded > 3) {
                Loaded = 0;
            }

            int gameCount = _gameCollection[Loaded].Games.Length;
            
            // Get the game scene name
            string scene = _gameCollection[Loaded].Games[Random.Range(0, gameCount)];
            Debug.Log($"<color=green>Next scene:{scene}</color>");

            return scene;
        }
    }
}
