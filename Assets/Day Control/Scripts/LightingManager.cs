using UnityEngine;

namespace DayControl
{
    [ExecuteAlways]
    public class LightingManager : MonoBehaviour
    {
        [Header("Light references")]
        [SerializeField, Tooltip("Directional light")] private Light _directionalLight;
        [SerializeField, Tooltip("Variable lighting values")] private LightingPreset _preset;
        [SerializeField, Tooltip("Will the skybox parameters change")] private bool _skyboxIsControlled = true;

        [Header("Sun settings")]
        [SerializeField, Range(0, 24), Tooltip("The present time of the day in 24 hour format")] private float _timeOfDay;
        [SerializeField, Range(0, 359), Tooltip("Sun start psotion - east ")] private float _sunStartPosition;
        [SerializeField, Range(-90, 90), Tooltip("Sun angle")] private float _sunAngle;

        [Header("Shutdown game objects")]
        [SerializeField, Tooltip("Time intervals of switching objects off/on")] private ObjectsShutdownTime[] _objectsShutdownTime;
        [SerializeField, Tooltip("Events that run at a certain time")] private EventTime[] _events;

        [Header("Clock options")]
        [SerializeField, Tooltip("Realtime clock")] private Timer _clockOptions;

        private float _maxTimeInTimeOfDay;
        private float _currentTimeInSeconds;
        private Quaternion _startRotation;
        private float _nextTimeOfDay;

        public Light DirectionalLight { get => _directionalLight; }
        public LightingPreset Preset { get => _preset; }
        public bool SkyboxIsControlled { get => _skyboxIsControlled; }
        public float TimeOfDay { get => _timeOfDay; }
        public float SunStartPosition { get => _sunStartPosition; }
        public float SunAngle { get => _sunAngle; }
        public ObjectsShutdownTime[] ObjectsShutdownTime { get => _objectsShutdownTime; }
        public EventTime[] Events { get => _events; }
        public Timer ClockOptions { get => _clockOptions; }

        //Setting initial values of maxima and initialization
        private void Awake()
        {
            _maxTimeInTimeOfDay = _preset.DayDuration * 60;
            _currentTimeInSeconds = _timeOfDay * _maxTimeInTimeOfDay / 24;
            InitObjectsShutdownTime();
            _preset.Init();
        }

        //Initializing the search for objects by name to disable/enable
        private void InitObjectsShutdownTime()
        {
            for (int i = 0; i < _objectsShutdownTime.Length; i++)
            {
                _objectsShutdownTime[i].Init();
            }
        }

        //Increasing the time and calculating the values of fields depending on the time
        private void Update()
        {
            if (_preset == null)
                return;

            if (Application.isPlaying)
            {
                IncreasingTime();
            }

            Tick();
        }

        private void IncreasingTime()
        {
            _currentTimeInSeconds += Time.deltaTime;
            _timeOfDay = _currentTimeInSeconds * 24 / _maxTimeInTimeOfDay;
            _nextTimeOfDay = _timeOfDay;
            _timeOfDay %= 24;
            _currentTimeInSeconds %= _maxTimeInTimeOfDay;
        }

        //Updating the characteristics of light and events
        private void Tick()
        {
            UpdateLight();
            UpdateEvents();
        }

        //Checking events occurring at a certain time and updating the clock
        private void UpdateEvents()
        {
            CheckShutdownObjects(_timeOfDay);
            CheckInvokeEvents(_timeOfDay);
            UpdateTimers(_timeOfDay);
        }

        //Updating the characteristics of the light
        private void UpdateLight()
        {
            var timePercent = _timeOfDay / 24f;
            UpdateLighting(timePercent);
            UpdateIntensity(timePercent);
            UpdateSkyboxExposure(timePercent);
            UpdateSkyboxColor(timePercent);
        }

        private void UpdateSkyboxColor(float timePercent)
        {
            RenderSettings.skybox.SetColor(_preset.NameFieldColorSkybox, _preset.ChangingSkyboxColor.Evaluate(timePercent));
        }

        //Updating the intensity of directional light
        private void UpdateIntensity(float singleTime)
        {
            _directionalLight.intensity = _preset.GetValueIntensity(singleTime);
        }

        //Updating the exposure of skybox
        private void UpdateSkyboxExposure(float singleTime)
        {
            if (!_skyboxIsControlled)
                return;

            RenderSettings.skybox.SetFloat("_Exposure", _preset.GetValueExposition(singleTime));
        }

        private void CheckShutdownObjects(float time)
        {
            for (int i = 0; i < _objectsShutdownTime.Length; i++)
            {
                _objectsShutdownTime[i].CheckShutdown(time);
            }
        }

        private void CheckInvokeEvents(float time)
        {
            for (int i = 0; i < _events.Length; i++)
            {
                _events[i].CheckInvoke(time, _nextTimeOfDay);
            }
        }

        //Updating analog and digital clocks
        private void UpdateTimers(float time)
        {
            _clockOptions.SetValue(time);
        }

        //Updating the ambient and fog color and turning the directional light
        private void UpdateLighting(float timePercent)
        {
            RenderSettings.ambientLight = _preset.AmbientColor.Evaluate(timePercent);
            RenderSettings.fogColor = _preset.FogColor.Evaluate(timePercent);

            if (_directionalLight != null)
            {
                _startRotation = Quaternion.Euler(0f, _sunStartPosition, _sunAngle);
                _directionalLight.color = _preset.DirectionalColor.Evaluate(timePercent);

                _directionalLight.transform.rotation = _startRotation * Quaternion.Euler(_preset.GetAnglesRotateLighing(timePercent) - 90f, 0f, 0f);
            }
        }

        //Search for directional light
        private void OnValidate()
        {
            if (_directionalLight != null)
                return;

            if (RenderSettings.sun != null)
            {
                _directionalLight = RenderSettings.sun;
            }
            else
            {
                Light[] lights = GameObject.FindObjectsOfType<Light>();
                foreach (Light light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        _directionalLight = light;
                        return;
                    }
                }
            }
        }
    }
}