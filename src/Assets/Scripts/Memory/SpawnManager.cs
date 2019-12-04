using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Memory {
    public class SpawnManager : MonoBehaviour {
        [SerializeField] private GameObject[] _cardPrefabs;

        private bool _paused;

        private List<int> _spawnKeys;

        private TimerManager _timerManager;

        [SerializeField] private Transform[] _spawnPoints;

        [SerializeField] private TextMeshProUGUI _timerText;

        public float Sec { get; } = 5f;

        private void Start() {
            _spawnKeys = new List<int>();

            Spawn();

            // Prevent all card touches
            foreach (var card in GameObject.FindGameObjectsWithTag("Card")) {
                card.GetComponent<Card>().Locked = true;
            }

            TimerManager.OnPreGameTimerEndEvent += CardUnlock;
        }

        private void Spawn() {
            for (int i = 0; i < _spawnPoints.Length; i++) {
                _spawnKeys.Add(i);
            }

            GameObject[] cards = _cardPrefabs;

            foreach (var card in cards) {
                // Generate random spawn
                int randomKey = Random.Range(0, _spawnKeys.Count);

                // Set random spawn point
                int spawnPoint = _spawnKeys.ElementAt(randomKey);

                // Instantiate the prefab
                Instantiate(card, _spawnPoints[spawnPoint].transform.position, Quaternion.identity);

                // Avoid using the same spawn point
                _spawnKeys.RemoveAt(randomKey);
            }
        }

        private void CardUnlock() {
            TimerManager.OnGameTimerEndEvent -= CardUnlock;

            foreach (var item in GameObject.FindGameObjectsWithTag("Card")) {
                item.GetComponent<Animator>().SetBool("flip", true);
                // Enable all card touches
                item.GetComponent<Card>().Locked = false;
            }
        }
    }
}
