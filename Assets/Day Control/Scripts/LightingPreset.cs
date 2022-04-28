using System;
using UnityEngine;

namespace DayControl
{
    [Serializable]
    [CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptables/Lighting Preset", order = 1)]
    public class LightingPreset : ScriptableObject
    {
        [Header("Light gradients")]
        [SerializeField, Tooltip("Update by render settings ambient color gradient")] private Gradient _ambientColor;
        [SerializeField, Tooltip("Update by directional light color gradient")] private Gradient _directionalColor;
        [SerializeField, Tooltip("Update by render settings fog color gradient")] private Gradient _fogColor;
        [SerializeField, Tooltip("Update by render settings skybox color gradient")] private Gradient _changingSkyboxColor;

        [Header("Day time")]
        [SerializeField, Tooltip("Time of day in minutes")] private float _dayDuration;
        [SerializeField, Range(0f, 24f), Tooltip("Sunrise time in 24 hour format")] private float _sunriseTime;
        [SerializeField, Range(0f, 24f), Tooltip("Sunset time in 24 hour format")] private float _sunsetTime;

        [Header("Light settings")]
        [SerializeField, Tooltip("The intensity of a light")] private LightingField _intensity;
        [SerializeField, Tooltip("The exposition of the skybox material")] private LightingField _exposition;

        public string NameFieldColorSkybox { get; private set; }

        public Gradient AmbientColor { get => _ambientColor; }
        public Gradient DirectionalColor { get => _directionalColor; }
        public Gradient FogColor { get => _fogColor; }
        public float DayDuration { get => _dayDuration; }
        public Gradient ChangingSkyboxColor { get => _changingSkyboxColor; }

        //Setting the name of the Skybox Color field
        public void Init()
        {
            if (RenderSettings.skybox.shader.name == "Skybox/Procedural")
            {
                NameFieldColorSkybox = "_SkyTint";
            }
            else
            {
                NameFieldColorSkybox = "_Tint";
            }
        }

        //Gives the angle of rotation depending on the time
        public float GetAnglesRotateLighing(float timeSingle)
        {
            var time = timeSingle * 24;

            if (time <= _sunriseTime)
            {
                return time / _sunriseTime * 90;
            }
            else if (time <= _sunsetTime)
            {
                return 90 + ((time - _sunriseTime) / (_sunsetTime - _sunriseTime) * 180);
            }
            else
            {
                return 270 + ((time - _sunsetTime) / (24 - _sunsetTime) * 90);
            }
        }

        //Gives the Intensity of directional light
        public float GetValueIntensity(float time) => _intensity.GetValueInTime(time, _sunriseTime, _sunsetTime);

        //Gives the exposure of skybox
        public float GetValueExposition(float time) => _exposition.GetValueInTime(time, _sunriseTime, _sunsetTime);
    }
}
