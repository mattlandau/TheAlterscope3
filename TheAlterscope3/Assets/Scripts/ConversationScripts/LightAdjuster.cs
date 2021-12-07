#define AR_ENABLED
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace TheAlterscope2
{
    public class LightAdjuster : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
#if UNITY_EDITOR
        public float _currentLightLevel = 1f;
#endif
#if !UNITY_EDITOR
        float? _currentLightLevel = 1f;
#endif
        float _changeability;
        Light _light;
        float _initialMainIntensity;
        float _initialAmbientIntensity;
#if AR_ENABLED
        ARCameraManager _cameraManager;
#endif

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _light = myDependencies.MainLight;
            _initialMainIntensity = _light.intensity;
            _initialAmbientIntensity = RenderSettings.ambientIntensity;
            _changeability = myDependencies.LightChangeability;
#if UNITY_EDITOR
            _currentLightLevel = _initialMainIntensity;
#endif
#if !UNITY_EDITOR
#if AR_ENABLED
            _cameraManager = myDependencies.MyARCameraManager;
            _cameraManager.frameReceived += FrameChanged;
#endif
#endif
            _hasBeenSetup = true;
        }

#if UNITY_EDITOR
        void Update()
        {
            _light.intensity = (_currentLightLevel * _changeability) + (_initialMainIntensity * (1f - _changeability));
            RenderSettings.ambientIntensity = (_currentLightLevel * _changeability) + (_initialAmbientIntensity * (1f - _changeability));
        }
#endif

#if !UNITY_EDITOR
        void OnDisable()
        {
#if AR_ENABLED
            if (_cameraManager != null)
                _cameraManager.frameReceived -= FrameChanged;
#endif
        }

        void OnEnable()
        {
            if (!_hasBeenSetup)
                return;
                
#if AR_ENABLED
            _cameraManager.frameReceived -= FrameChanged;
            _cameraManager.frameReceived += FrameChanged;
#endif
        }
#if AR_ENABLED
        void FrameChanged(ARCameraFrameEventArgs args)
        {
            if (args.lightEstimation.averageBrightness.HasValue)
            {
                _currentLightLevel = args.lightEstimation.averageBrightness;
                float tempCurrentLightLevel = _currentLightLevel ?? 1f;
                _light.intensity = (tempCurrentLightLevel * _changeability) + (_initialMainIntensity * (1f - _changeability));
                RenderSettings.ambientIntensity = (tempCurrentLightLevel * _changeability) + (_initialAmbientIntensity * (1f - _changeability));
            }
            if (args.lightEstimation.colorCorrection.HasValue)
            {
                Color colorCorrection = args.lightEstimation.colorCorrection ?? _light.color;
                _light.color = colorCorrection;
            }
            if (args.lightEstimation.averageColorTemperature.HasValue)
            {
                float averageColorTemperature = args.lightEstimation.averageColorTemperature ?? _light.colorTemperature;
                _light.colorTemperature = averageColorTemperature;
            }
        }
#endif
#endif
    }
}