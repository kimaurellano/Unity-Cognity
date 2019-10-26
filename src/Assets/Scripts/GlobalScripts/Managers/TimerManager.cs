using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Managers {
    /// <summary>
    ///     Provides TimerManager functionality.
    /// </summary>
    public class TimerManager : MonoBehaviour {
        public delegate void OnTimerEnd();

        public static event OnTimerEnd OnPreGameTimerEndEvent;
        public static event OnTimerEnd OnGameTimerEndEvent;

        public TextMeshProUGUI TimerText;
        public Animation TimerAnimation;
        public float Seconds;
        public int Minutes;
        public bool IsPreGameTimer;

        public TextMeshProUGUI AttachedTextObject => GetComponent<TextMeshProUGUI>();

        // Ticking will be managed only by StartTimerAt and StartTimer
        public bool Ticking { get; private set; }

        private float _t;

        /// <summary>
        ///     Ups timer upon t minutes less than 0
        /// </summary>
        public bool TimerUp => Minutes < 0 && Seconds == 0;

        private void Start() {
            if (IsPreGameTimer) {
                // The script should be attached on a text object if used as a pre-game timer
                transform.parent.gameObject.SetActive(true);

                StartCoroutine(PreGameTimer(Seconds));

                return;
            }
        }

        private void Update() {
            if (Ticking) Tick();

            if (TimerUp) {
                OnGameTimerEndEvent?.Invoke();

                TimerText?.SetText("Time: 00:00");

                Ticking = false;
            }
        }

        /// <summary>
        ///     Handles ticking of timer
        /// </summary>
        private void Tick() {
            _t -= Time.deltaTime;

            Seconds = (int) (_t % 60);

            if (_t % 60 < 0) {
                _t = 60f;
                Minutes--;
            }

            TimerText?.SetText($"Time: {Minutes:00}:{Seconds:00}");

            if (Minutes == 0 && Seconds == 10) {
                if (TimerAnimation != null) {
                    TimerAnimation.Play();
                }
            }
        }

        /// <summary>
        ///     Starts timer
        /// </summary>
        public void StartTimerAt(int min, float sec) {
            StopTimer();

            Ticking = true;

            _t = sec;
            Minutes = min;
        }

        public void StartTimer() {
            Ticking = true;
        }

        public void StopTimer() {
            Ticking = false;
        }

        /// <summary>
        ///     Pauses/Unpauses timer
        /// </summary>
        public void ChangeTimerState() {
            Ticking = !Ticking;
        }

        public void ChangeTimerState(bool state) {
            Ticking = state;
        }

        private IEnumerator PreGameTimer(float seconds) {
            var countDown = seconds;

            while (countDown > -1) {
                AttachedTextObject.SetText(countDown.ToString());

                countDown--;

                // Avoid null reference exception
                if (TimerAnimation == null) {
                    throw new ArgumentNullException("There is no animation attached");
                }

                TimerAnimation.Play();

                yield return new WaitUntil(() => !TimerAnimation.isPlaying);
            }

            // The script should be attached on a text object if used as a pre-game timer
            transform.parent.gameObject.SetActive(false);

            // Invoke all the subcribed methods
            OnPreGameTimerEndEvent?.Invoke();
        }
    }
}