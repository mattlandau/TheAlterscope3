using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class ApplicationBehaviors : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        [SerializeField]
        bool _hasBeenPaused = false;
        ChangeMode _changeMode;
        UserPreferences _userPreferences;
        ConversationStateMachine _conversationStateMachine;
        float _oldTime, _newTime;
        public bool HasBeenPausedAndNoFurtherInteractions;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _changeMode = myDependencies.MyChangeMode;

            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _logger.Log("Device name/model/type: " + SystemInfo.deviceName + " / " + SystemInfo.deviceModel + " / " + SystemInfo.deviceType);
            _userPreferences = myDependencies.MyUserPreferences;
            _oldTime = Time.time;
            _hasBeenSetup = true;
        }

        void OnApplicationFocus(bool hasFocus)
        {
            _logger.Log("OnApplicationFocus: " + hasFocus);
            _newTime = Time.realtimeSinceStartup;
            _logger.Log("Time since pause: " + (_newTime - _oldTime));
            _oldTime = _newTime;
            if (_changeMode.CurrentMode == Mode.Quit)
                return;

            _userPreferences.Refresh();
            HasBeenPausedAndNoFurtherInteractions = true;

            _logger.Log($"Current state: { _conversationStateMachine.CurrentState} and previous state: {_conversationStateMachine.PreviousState}");
            if (_hasBeenPaused == true)
                if (_conversationStateMachine.PreviousState != ConversationState.AvatarsPlaced)
                    _changeMode.ReEnterPlacingMode();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            _logger.Log("OnApplicationPause: " + pauseStatus);
            _oldTime = Time.time;
            _hasBeenPaused = pauseStatus;
            _logger.Log($"Current state: { _conversationStateMachine.CurrentState} and previous state: {_conversationStateMachine.PreviousState}");
        }

        void OnAudioConfigurationChanged(bool deviceWasChanged)
        {
            _logger.Log(deviceWasChanged ? "Device was changed" : "Reset was called");
            _logger.Log($"Current state: { _conversationStateMachine.CurrentState} and previous state: {_conversationStateMachine.PreviousState}");

        }
    }
}