using System;
using UnityEngine;

namespace DayControl
{
    [Serializable]
    public class Timer
    {
        [SerializeField] private DigitalClock[] _digitalClocks;
        [SerializeField] private AnalogClock[] _analogClocks;

        //Updating analog and digital clocks
        public void SetValue(float timeHours)
        {
            var minuts = (timeHours - (int)timeHours) * 60;
            var seconds = (minuts - (int)minuts) * 60;

            for (int i = 0; i < _digitalClocks.Length; i++)
            {
                _digitalClocks[i].UpdateText(timeHours);
            }

            for (int i = 0; i < _analogClocks.Length; i++)
            {
                _analogClocks[i].TurnClockHands(timeHours, minuts, seconds);
            }
        }
    }
}
