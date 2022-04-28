using System;
using UnityEngine;

namespace DayControl
{
    [Serializable]
    public struct LightingField
    {
        [SerializeField, Tooltip("Minimum value")] private float _min;
        [SerializeField, Tooltip("Maximum value")] private float _max;

        //Getting the value depending on the time, from the minimum at sunrise to the maximum and back to the minimum at sunset
        public float GetValueInTime(float singleTime, float sunriseTime, float sunsetTime)
        {
            var time = singleTime * 24;

            if (time < sunriseTime || time > sunsetTime)
                return _min;

            var percentageInterval = (time - sunriseTime) * 2 / (sunsetTime - sunriseTime);

            if (percentageInterval > 1)
            {
                percentageInterval = 2 - percentageInterval;
            }

            return _min + ((_max - _min) * percentageInterval);
        }
    }
}
