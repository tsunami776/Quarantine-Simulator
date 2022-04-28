using System;
using UnityEngine;
using UnityEngine.Events;


namespace DayControl
{
    [Serializable]
    public struct EventTime
    {
        [SerializeField, Range(0, 24f), Tooltip("The time at which the event will be executed")] private float _timeEvent;
        [SerializeField, Tooltip("The time at which the event will be executed")] private UnityEvent _gameObjectsShutdown;

        private bool _completedPerDay;

        //Checking the event activation at a certain time
        public void CheckInvoke(float time, float nextTime)
        {
            Invoke(time >= _timeEvent && !_completedPerDay);

            if (nextTime >= 24)
            {
                _completedPerDay = false;
            }
        }

        //Enabling the event
        private void Invoke(bool isActive)
        {
            if (isActive)
            {
                _gameObjectsShutdown.Invoke();
                _completedPerDay = true;
            }
        }
    }
}
