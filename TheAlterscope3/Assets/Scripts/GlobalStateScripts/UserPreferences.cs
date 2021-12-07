using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace TheAlterscope2
{
    public class UserPreferences : MonoBehaviour
    {


        [DllImport("__Internal")]
        private static extern bool GetBoolValue(string key);

        [DllImport("__Internal")]
        private static extern void SetBoolValue(string key, bool value);

        public bool IsVibrationEnabled;
        public bool IsEnvironmentalSoundEnabled;

        ILogger _logger;
        bool _hasBeenSetup;
        iOSHapticFeedback _hapticFeedback;
        HapticFallback _hapticFallback;
        NoHaptics _noHaptics;
        AudioSource _boothAudioSource;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _hapticFeedback = myDependencies.MyHapticFeedback;
            _hapticFallback = myDependencies.MyHapticFallback;
            _noHaptics = myDependencies.MyNoHaptics;
            _boothAudioSource = myDependencies.BoothSlideAudioSource;
            Refresh();
            _hasBeenSetup = true;
        }

        public void Refresh()
        {
#if !UNITY_EDITOR
            IsVibrationEnabled = GetBoolValue("enable_vibrations");
            IsEnvironmentalSoundEnabled = GetBoolValue("enable_environmental_sounds");
#endif
            _boothAudioSource.mute = !IsEnvironmentalSoundEnabled;
        }

        public IHapticFeedback CurrentHapticFeedback()
        {
            var currentFeedback = _hapticFeedback.IsSupported() ? (IHapticFeedback)_hapticFeedback : (IHapticFeedback)_hapticFallback;
            if (_hapticFeedback.IsSupported())
                _logger.Log("using Haptics");
            else
                _logger.Log("using Haptic fallback");

            if (IsVibrationEnabled)
                return currentFeedback;
            else
            {
                _logger.Log("Haptics off by user preference");
                return (IHapticFeedback)_noHaptics;
            }
        }
    }
}