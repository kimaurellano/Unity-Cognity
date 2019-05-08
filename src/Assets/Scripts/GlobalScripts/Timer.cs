using UnityEngine;
using TMPro;

namespace Assets.Scripts.GlobalScripts
{
    /// <summary>
    /// Provides Timer functionality.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        private float _t;

        private bool _startTimer;

        ///<summary>
        /// The seconds
        ///</summary>
        public float Sec { get; set; }

        ///<summary>
        /// The minutes
        ///</summary>
        public int Min { get; set; }

        ///<summary>
        /// Ups timer upon t minutes less than 0
        ///</summary>
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public bool TimerUp => Min < 0 && Sec == 0;

        ///<summary>
        /// Sets/Gets the textmeshpro text
        ///</summary>
        [field: SerializeField] public TextMeshProUGUI TimerText { get; set; }

        private void Update()
        {
            if (_startTimer)
            {
                Tick();
            }

            if (TimerUp)
            {
                TimerText.SetText("Timer: 00:00");
                _startTimer = false;
            }
        }

        ///<summary>
        /// Handles ticking of timer
        ///</summary>
        private void Tick()
        {
            _t -= Time.deltaTime;

            Sec = (int)(_t % 60);

            if (_t % 60 < 0)
            {
                _t = 60f;
                Min--;
            }

            TimerText.SetText($"Time: {Min:00}:{Sec:00}");
        }

        ///<summary>
        /// Starts immediately timer at certain point in time
        ///</summary>
        public void StartTimerAt(int min, float sec)
        {
            _startTimer = true;

            _t = sec;
            Min = min;
        }

        ///<summary>
        /// Pauses/Unpauses timer
        ///</summary>
        public void ChangeTimerState() 
        {
            _startTimer = !_startTimer;
        }
    }
}