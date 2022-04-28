using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace DayControl
{
    [Serializable]
    public class DigitalClock
    {
        [SerializeField] private TextMeshProUGUI _digitalClock;
        [SerializeField] private bool _isDisplayedHours = true;
        [SerializeField] private bool _isDisplayedMinuts = true;
        [SerializeField] private bool _isDisplayedSeconds = true;

        //Inserting the time, depending on the format, into the text field
        public void UpdateText(float timeHours)
        {
            if (_digitalClock == null)
                return;

            var timeFormat = GetTimeFormat();

            if (timeFormat.Length == 0)
            {
                if (_digitalClock.text.Length > 0)
                    _digitalClock.text = string.Empty;
                return;
            }

            _digitalClock.text = new DateTime()
                .AddHours(timeHours)
                .ToString(timeFormat);
        }

        //Checking the time format
        private string GetTimeFormat()
        {
            var timeFormat = new StringBuilder();

            if (_isDisplayedHours)
            {
                timeFormat.Append("HH");
            }

            if (_isDisplayedMinuts)
            {
                timeFormat.Append(":mm");
            }

            if (_isDisplayedSeconds)
            {
                timeFormat.Append(":ss");
            }

            return timeFormat.ToString();
        }

    }
}
