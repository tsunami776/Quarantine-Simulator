using System;
using UnityEngine;

namespace DayControl
{
    [Serializable]
    public class AnalogClock
    {
        [Header("Analog Clock")]
        [SerializeField] private GameObject _hourNeedle;
        [SerializeField] private GameObject _minuteNeedle;
        [SerializeField] private GameObject _secondNeedle;

        //Turning the clock hands
        public void TurnClockHands(float hours, float minutes, float seconds)
        {
            if (_hourNeedle != null)
            {
                TurnClockNeedle((hours % 12) / 12, _hourNeedle);
            }

            if (_minuteNeedle != null)
            {
                TurnClockNeedle(minutes / 60, _minuteNeedle);
            }

            if (_secondNeedle != null)
            {
                TurnClockNeedle(seconds / 60, _secondNeedle);
            }
        }

        //Rotation of a certain arrow along the z axis depending on time as a percentage
        private void TurnClockNeedle(float timePercent, GameObject needle)
        {
            needle.transform.localRotation = Quaternion.Euler(
                new Vector3(0f, 0f, timePercent * -360));
        }
    }
}
