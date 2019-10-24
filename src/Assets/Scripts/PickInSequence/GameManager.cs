using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.PickInSequence {
    public class GameManager : CoreGameBehaviour {

        [SerializeField] private GameObject _numberPrefab;

        [SerializeField] private float _radius;
        [SerializeField] private float _minY, _minX;
        [SerializeField] private float _maxY, _maxX;

        private List<int> _rndSequence;
        private int curElement = 0;

        public float MinY { get => _minY; }
        public float MinX { get => _minX; }
        public float MaxY { get => _maxY; }
        public float MaxX { get => _maxX; }

        private void Start() {
            NumberScript.OnNumberPopEvent += ProceedToNextElement;

            InstantiateNumber(RandomSequence(5));
        }

        private void InstantiateNumber(int[] sequence) {
            for (int i = 0; i < sequence.Length; i++) {
                GameObject numPrefab = Instantiate(
                    _numberPrefab,
                    new Vector3(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY), 0f),
                    Quaternion.identity);

                numPrefab
                    .transform
                    .GetChild(0)
                    .GetChild(0)
                    .GetComponent<TextMeshProUGUI>()
                    .SetText(sequence[i].ToString());
            }
        }

        private int[] RandomSequence(int length) {
            _rndSequence = new List<int>();

            System.Random rnd = new System.Random();
            for (int i = 0; i < length; i++) {
                int curRnd = rnd.Next(100);
                if (_rndSequence.Exists(e => e.Equals(curRnd))) {
                    continue;
                }

                _rndSequence.Add(rnd.Next(100));
            }
            _rndSequence.Sort();

            Debug.Log("Sequence");
            foreach (var item in _rndSequence) {
                Debug.Log(item);
            }

            return _rndSequence.ToArray();
        }

        private void Respawn() {
            Debug.Log("Respawning...");

            foreach (var item in Resources.FindObjectsOfTypeAll(typeof(NumberScript)) as NumberScript[]) {
                Destroy(item);
            }

            InstantiateNumber(RandomSequence(5));
        }

        public int GetCurrentElement() {
            return _rndSequence.ToArray()[curElement];
        }

        private void ProceedToNextElement() {
            curElement++;
        }
    }
}