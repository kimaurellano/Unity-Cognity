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

        public Collection[] GameCollections { get => _gameCollection; set => _gameCollection = value; }

        private void Start() {
            if (_gameCollectionInstance != null) {
                Destroy(_gameCollectionInstance);
            } else {
                _gameCollectionInstance = this;
            }

            DontDestroyOnLoad(this);
        }
    }
}
