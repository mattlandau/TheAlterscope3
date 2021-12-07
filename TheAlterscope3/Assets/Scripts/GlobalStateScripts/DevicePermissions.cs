using System.Collections;
using UnityEngine;

namespace TheAlterscope2
{
    public class DevicePermissions : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        bool _isCameraAuthorized, _isMicrophoneAuthorized;
        ChangeMode _changeMode;
        GameObject _enableCamera;
        GameObject _enableMicrophone;
        GameObject _enableMicrophoneAndCamera;
        GameObject _enableMicrophoneAndQuit;
        LaunchFactory _launchFactory;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _changeMode = myDependencies.MyChangeMode;
            _enableCamera = myDependencies.NeedCameraAccess;
            _enableMicrophone = myDependencies.NeedMicrophoneAccess;
            _enableMicrophoneAndCamera = myDependencies.NeedMicrophoneAndCameraAccess;
            _enableMicrophoneAndQuit = myDependencies.NeedMicrophoneAndQuit;
            _launchFactory = myDependencies.MyLaunchFactory;
            _hasBeenSetup = true;
        }

        public void CheckPermissions()
        {
            if (_launchFactory.IsInitialLaunch)
                StartCoroutine(GetInitialPermissions());
            else
                StartCoroutine(GetSubsequentPermissions());
        }

        IEnumerator GetInitialPermissions()
        {
            _logger.Log("GetInitialPermssions");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
                _changeMode.EnterPlacingMode();
            else
                ShowPermissionMessages();
        }

        IEnumerator GetSubsequentPermissions()
        {
            _logger.Log("GetSubsequentPermssions");
            _isMicrophoneAuthorized = Application.HasUserAuthorization(UserAuthorization.Microphone);
            if (!_isMicrophoneAuthorized)
            {
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
                _isMicrophoneAuthorized = Application.HasUserAuthorization(UserAuthorization.Microphone);
            }
            yield return new WaitForSeconds(0.5f);

            _isCameraAuthorized = Application.HasUserAuthorization(UserAuthorization.WebCam);
            if (!_isCameraAuthorized)
            {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
                _isCameraAuthorized = Application.HasUserAuthorization(UserAuthorization.WebCam);
            }

            if (_isCameraAuthorized && _isMicrophoneAuthorized)
                _changeMode.EnterPlacingMode();
            else
                ShowPermissionMessages();
        }

        public void ShowMicrophoneNeededAndQuit() => _enableMicrophoneAndQuit.SetActive(true);

        void ShowPermissionMessages()
        {
            _logger.Log($"camera {_isCameraAuthorized}, microphone {_isMicrophoneAuthorized}, initial {_launchFactory.IsInitialLaunch}");

            _isMicrophoneAuthorized = Application.HasUserAuthorization(UserAuthorization.WebCam);
            _isCameraAuthorized = Application.HasUserAuthorization(UserAuthorization.WebCam);

            if (!_isCameraAuthorized && !_isMicrophoneAuthorized)
                _enableMicrophoneAndCamera.SetActive(true);
            else if (!_isCameraAuthorized)
                _enableCamera.SetActive(true);
            else if (!_isMicrophoneAuthorized && !_launchFactory.IsInitialLaunch)
                _enableMicrophone.SetActive(true);
        }

    }
}