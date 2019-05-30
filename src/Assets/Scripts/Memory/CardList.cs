using System;
using Assets.Scripts.GlobalScripts.UITask;
using UnityEngine;

namespace Assets.Scripts.Memory {
    public class CardList : MonoBehaviour {
        [SerializeField] private Transform _firstPick;

        private ScoreManager _scoreManager;

        public Transform FirstPick {
            get => _firstPick;
            set => _firstPick = value;
        }

        public int LockCount { get; set; }

        private void Start() {
            _scoreManager = new ScoreManager();
        }

        private void Update() {
            if (LockCount != 14) {
                return;
            }

            Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "success panel")
                .Panel
                .gameObject
                .SetActive(true);

            // Add score 
            _scoreManager.SaveUserScore(GameObject.Find("CardSpawn").GetComponent<SpawnManager>().Sec);
        }

        public void AddLockedCard() {
            LockCount += 2;
        }
    }
}
