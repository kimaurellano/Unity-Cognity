using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Managers {
    /// <summary>
    /// Provides TimerManager functionality.
    /// </summary>
    public class TimerManager : MonoBehaviour {
        public delegate void OnTimerEnd();

        public static event OnTimerEnd OnPreGameTimerEndEvent;
        public static event OnTimerEnd OnGameTimerEndEvent;

        private Coroutine _coroutineTick;
        private Coroutine _coroutingPreGameTimer;
        private float _tempSec;
        private int _tempMin;
        private float _t;

        public TextMeshProUGUI TimerText;
        public Animation TimerAnimation;
        public float Seconds;
        public int Minutes;
        public bool IsPreGameTimer;

        public TextMeshProUGUI AttachedTextObject => GetComponent<TextMeshProUGUI>();

        public bool Active { get; private set; }

        private void Start() {
            if (IsPreGameTimer) {
                // The script should be attached on a text object if used as a pre-game timer
                transform.parent.gameObject.SetActive(true);

                _coroutingPreGameTimer = StartCoroutine(IE_PreGameTimer(Seconds));
            }
        }

        private void Update() {
            // When paused
            if (!Active) return;

            // Timer up
            if (Minutes < 0 && Mathf.RoundToInt(Seconds) == 0) {
                Debug.Log("Timer up");

                Active = false;

                OnGameTimerEndEvent?.Invoke();

                TimerText?.SetText("Time: 00:00");
            }

            // Continue ticking
            Tick();
        }

        /// <summary>
        /// Handles ticking of timer
        /// </summary>
        private void Tick() {
            _t -= Time.deltaTime;

            Seconds = (int) (_t % 60);

            if (_t % 60 < 0) {
                _t = 60f;
                Minutes--;
            }

            TimerText?.SetText($"Time: {Minutes:00}:{Seconds:00}");

            if (Minutes == 0 && Mathf.RoundToInt(Seconds) == 10) {
                if (TimerAnimation == null) return;
                
                TimerAnimation.Play();
            }
        }

        /// <summary>
        /// Starts timer
        /// </summary>
        public void StartTimerAt(int min, float sec) {
            _t = sec;
            Minutes = min;

            Active = true;

            Debug.Log($"<color=green>Started at {min}:{sec}, Active:{Active}</color>");

            _tempMin = min;
            _tempSec = sec;
        }

        public void StartTimer() {
            Active = true;
        }

        /// <summary>
        /// Will reset to the original minute and second used from StartTimerAt(min, sec)
        /// </summary>
        public void ResetTimer() {
            StartTimerAt(_tempMin, _tempSec);

            Debug.Log("Timer reset");
        }

        /// <summary>
        /// Pauses/Unpauses timer
        /// </summary>
        public void ChangeTimerState() {
            Active = !Active;
        }

        public void ChangeTimerState(bool state) {
            Active = state;
        }

        private IEnumerator IE_PreGameTimer(float seconds) {
            var countDown = seconds;

            while (countDown > -1) {
                AttachedTextObject?.SetText(countDown.ToString());

                countDown--;

                // Avoid null reference exception
                if (TimerAnimation == null) {
                    Debug.LogWarning("There's no timer animation", TimerAnimation);
                    yield break;
                }

                TimerAnimation.Play();

                yield return new WaitUntil(() => !TimerAnimation.isPlaying);
            }

            // The script should be attached on a text object if used as a pre-game timer
            transform.parent.gameObject.SetActive(false);

            // Invoke all the subcribed methods
            OnPreGameTimerEndEvent?.Invoke();

            StopCoroutine(_coroutingPreGameTimer);
        }
    }
}