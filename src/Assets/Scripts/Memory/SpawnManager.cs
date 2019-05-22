using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using Assets.Scripts.GlobalScripts;
using Assets.Scripts.GlobalScripts.UITask;

namespace Assets.Scripts.Memory
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private GameObject[] _cardPrefabs;
        private List<int> _spawnKeys;
        private bool _gameStart;

        public float Sec { get; private set; } = 5f;

        private void Start()
        {
            _spawnKeys = new List<int>();

            Spawn();

            // Prevent all card touches
            foreach (var card in GameObject.FindGameObjectsWithTag("Card"))
            {
                card.GetComponent<Card>().Locked = true;
            }

            Array.Find(FindObjectOfType<UIManager>().ButtonCollection, i => i.Name == "button pause")
                .Button
                .transform
                .gameObject
                .SetActive(false);
        }

        private void Update()
        {
            if (!_gameStart)
            {
                Sec -= Time.deltaTime;

                _timerText.SetText(Sec.ToString("F0"));
            }
            else
            {
                Sec -= Time.deltaTime;

                _timerText.SetText(Sec.ToString("F0"));
            }

            if (Sec < 0.01f && !_gameStart)
            {
                _gameStart = true;

                foreach (var item in GameObject.FindGameObjectsWithTag("Card"))
                {
                    item.GetComponent<Animator>().SetBool("flip", true);
                    // Enable all card touches
                    item.GetComponent<Card>().Locked = false;
                }

                Sec = 45f;

                Array.Find(FindObjectOfType<UIManager>().ButtonCollection, i => i.Name == "button pause")
                    .Button
                    .transform
                    .gameObject
                    .SetActive(true);

                _timerText.SetText("");
            }
            else if (Sec < 0.01f && _gameStart)
            {
                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "failed panel")
                    .Panel
                    .transform
                    .gameObject
                    .SetActive(true);

                _timerText.SetText("");
            }
        }

        private void Spawn()
        {
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                _spawnKeys.Add(i);
            }

            var cards = _cardPrefabs;

            foreach (var card in cards)
            {
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
    }


}
