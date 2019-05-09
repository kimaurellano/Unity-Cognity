using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GlobalScripts;

namespace Assets.Scripts.Memory
{
    public class CardList : MonoBehaviour
    {
        [SerializeField] private Transform _firstPick;
        private int _lockCount;
        private ScoreManager _scoreManager;

        public Transform FirstPick { get => _firstPick; set => _firstPick = value; }
        public int LockCount { get => _lockCount; set => _lockCount = value; }

        private void Start() 
        {
            _scoreManager = new ScoreManager();
        }

        private void Update()
        {
            Debug.Log(LockCount);

            if (LockCount == 14)
            {
                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "success panel")
                    .Panel
                    .gameObject
                    .SetActive(true);

                // Add score 
                _scoreManager.SaveUserScore(GameObject.Find("CardSpawn").GetComponent<SpawnManager>().Sec);
            }
        }

        public void AddLockedCard()
        {
            _lockCount += 2;
        }
    }
}
