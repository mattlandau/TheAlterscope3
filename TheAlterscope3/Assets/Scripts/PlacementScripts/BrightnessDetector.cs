// #if INCLUDE_OTHER_DEPRECATD
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace TheAlterscope2
{
    public class BrightnessDetector : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        ARCameraManager MyCameraManager;
        // PlacementMessage _messageVisualier;
        float? _brightness;
        float _brightnessThreshold;
        float _timeLag;
        GameObject _brightnessMessage;
        float _initialTimeOfTooDim;
        float _initialTimeOfNotTooDim;
        bool _tooDimDetected, _brightEnough;
        bool _hasBeenSetup;
        public float TestBrightnessLevel = 0f; //for testing

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            MyCameraManager = myDependencies.MyARCameraManager;
#if !UNITY_EDITOR
            MyCameraManager.frameReceived += FrameChanged;
#endif
            _brightnessThreshold = myDependencies.BrightnessThreshold;
            _brightnessMessage = myDependencies.BrightnessMessage;
            _timeLag = myDependencies.BrightnessIndicatorLag;
            // _brightnessMessage = myDependencies.BrightnessMessage;
            // _messageVisualier = myDependencies.MyMessageVisualizer;
            _logger.Log("BrightnessDetector setup ran");
            _hasBeenSetup = true;
        }

#if !UNITY_EDITOR
        void OnDisable()
        {
            MyCameraManager.frameReceived -= FrameChanged;
        }

        void OnEnable()
        {
            if (!_hasBeenSetup)
                return;

            MyCameraManager.frameReceived -= FrameChanged;
            MyCameraManager.frameReceived += FrameChanged;
        }
#endif

        void TooDim()
        {
            if (_tooDimDetected == false)
            {
                _initialTimeOfTooDim = Time.time;
                _tooDimDetected = true;
                _brightEnough = false;
            }

            if (_brightnessMessage.activeSelf == true)
                return;

            if (Time.time - _initialTimeOfTooDim > _timeLag)
                _brightnessMessage.SetActive(true);
        }

        void BrightEnough()
        {
            if (_brightEnough == false)
            {
                _initialTimeOfNotTooDim = Time.time;
                _tooDimDetected = false;
                _brightEnough = true;
            }

            if (_brightnessMessage.activeSelf == false)
                return;

            if (Time.time - _initialTimeOfTooDim > _timeLag)
                _brightnessMessage.SetActive(false);
        }
#if UNITY_EDITOR
        void Update()
        {
            _brightness = TestBrightnessLevel;
            if (_brightness <= _brightnessThreshold)
                TooDim();
            else
                BrightEnough();
        }
#endif

#if !UNITY_EDITOR
        void FrameChanged(ARCameraFrameEventArgs args)
        {
            _brightness = args.lightEstimation.averageBrightness;
            if (_brightness <= _brightnessThreshold)
                TooDim();
            else
                BrightEnough();
        }
#endif
    }
}
// #endif