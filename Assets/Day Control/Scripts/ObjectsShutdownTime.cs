using System;
using System.Collections.Generic;
using UnityEngine;


namespace DayControl
{
    [Serializable]
    public struct ObjectsShutdownTime
    {
        [SerializeField, Range(0, 24f), Tooltip("Start of the object shutdown/on time")] private float _startTime;
        [SerializeField, Range(0, 24f), Tooltip("End of the object shutdown/on time")] private float _endTime;
        [SerializeField, Tooltip("Objects to turn off/on")] private List<GameObject> _gameObjectsShutdown;
        [SerializeField, Tooltip("Names of objects to turn off/on")] private List<string> _namesGOShutdown;
        [SerializeField, Tooltip("turning off/on during the interval")] private bool _shutdownObject;

        public void Init()
        {
            FindObjects();
        }

        //Search for objects by name to disable/enable
        private void FindObjects()
        {
            var objects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

            for (int i = 0; i < objects.Length; i++)
            {
                for (int j = 0; j < _namesGOShutdown.Count; j++)
                {
                    if (SearchString(objects[i].name, _namesGOShutdown[j]) && !_gameObjectsShutdown.Contains(objects[i]))
                    {
                        _gameObjectsShutdown.Add(objects[i]);
                        break;
                    }
                }
            }
        }

        //Search for substrings in names
        public static bool SearchString(string str, string pat)
        {
            var m = pat.Length;
            var n = str.Length;

            int[] badChar = new int[256];

            BadCharHeuristic(pat, m, ref badChar);

            int s = 0;
            while (s <= (n - m))
            {
                int j = m - 1;

                while (j >= 0 && pat[j] == str[s + j])
                    --j;

                if (j < 0)
                {
                    return true;
                }
                else
                {
                    s += Math.Max(1, j - badChar[str[s + j]]);
                }
            }

            return false;
        }

        private static void BadCharHeuristic(string str, int size, ref int[] badChar)
        {
            int i;

            for (i = 0; i < 256; i++)
                badChar[i] = -1;

            for (i = 0; i < size; i++)
                badChar[(int)str[i]] = i;
        }

        //Checking objects off/on
        public void CheckShutdown(float time)
        {
            ShutdownObjects((time >= _startTime && time <= _endTime) == _shutdownObject);
        }

        //Turning objects off/on
        private void ShutdownObjects(bool isActive)
        {
            if (_gameObjectsShutdown.Count == 0 ||
                _gameObjectsShutdown[0].activeSelf == isActive)
                return;

            for (int i = 0; i < _gameObjectsShutdown.Count; i++)
            {
                _gameObjectsShutdown[i].SetActive(isActive);
            }
        }
    }
}
