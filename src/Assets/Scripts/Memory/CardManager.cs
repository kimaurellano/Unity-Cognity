using System;
using System.Collections;
using Assets.Scripts.Database.Enum;
using Assets.Scripts.GlobalScripts.Player;
using Assets.Scripts.GlobalScripts.UITask;
using UnityEngine;

namespace Assets.Scripts.Memory {
    public class CardManager : MonoBehaviour {
        [SerializeField] private Transform _firstPick;

        [SerializeField] private Transform _secondPick;

        public Transform FirstPick {
            get => _firstPick;
            set => _firstPick = value;
        }

        public Transform SecondPick {
            get => _secondPick;
            set => _secondPick = value;
        }

        public int TouchCount { get; set; }

        public int LockCount { get; set; }

        public bool OnFlip { get; set; }

        private void Update() {
            // There are 7 pairs
            if (LockCount < 7) {
                return;
            }

            Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "success panel")
                .Panel
                .gameObject
                .SetActive(true);

            // Add score 
            BaseScoreHandler baseScoreHandler = new BaseScoreHandler();
            baseScoreHandler.SaveScore(GameObject.Find("CardSpawn").GetComponent<SpawnManager>().Sec, Game.GameType.Memory);
        }

        public IEnumerator WaitForFlip() {
            OnFlip = true;

            yield return new WaitForSeconds(1f);

            _firstPick.GetComponent<Animator>().SetBool("flip", true);
            _secondPick.GetComponent<Animator>().SetBool("flip", true);

            _firstPick = null;
            _secondPick = null;

            OnFlip = false;
        }
    }
}
