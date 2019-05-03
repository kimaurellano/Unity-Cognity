using UnityEngine;
using TMPro;

namespace Assets.Scripts.GlobalScripts
{
    /// <summary>
    /// Provides Timer functionality.
    /// </summary>
    public class Timer : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _timerText;

        // Delta time
        private float _t;

        // Should start the timer ?
        private bool _startTimer;

        // Well it is self explanatory
        public float Sec { get; set; }

        // Well it is self explanatory
        public int Min { get; set; }

        // Well it is self explanatory
        public bool TimerUp { get => Min < 0; }

        // Text where the timer is parsed into a string format
        public TextMeshProUGUI TimerText { get => _timerText; set => _timerText = value; }
        public bool StartTimer { get => _startTimer; set => _startTimer = value; }

        private void Update()
        {
            if (StartTimer)
            {
                Tick();
            }

            if (TimerUp || !StartTimer)
            {
                TimerText.SetText("Timer: 00:00");
                StartTimer = false;
            }
        }

        private void Tick()
        {
            _t -= Time.deltaTime;

            Sec = (int)(_t % 60);

            if (_t % 60 < 0)
            {
                _t = 60f;
                Min--;
            }

            TimerText.SetText(string.Format("Time: {0:00}:{1:00}", Min, Sec));
        }

        public void StartTimerAt(int min, float sec)
        {
            StartTimer = true;

            _t = sec;
            Min = min;
        }

        public void ChangeTimerState()
        {
            if (StartTimer)
            {
                StartTimer = false;
            }
            else
            {
                StartTimer = true;
            }
        }
    }
}