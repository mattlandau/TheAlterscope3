using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public enum PlaybackMode
    {
        All,
        Single,
        Altered
    }

    public class PlaybackControls : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        DialogueMediator _dialogueMediator;
        float _audioPlaybackStartDelayDuration;
        bool _hasBeenSetup;
        ConversationStateMachine _conversationStateMachine;
        GameObject _currentPlaybackAvatar;
        PlaybackAudio _playbackAudio;
        // Delay _delayScript;
        float _delayBeforePlayback;
        PlaybackMode _playbackMode = PlaybackMode.Single;
        AudioRecorder _audioRecorder;
        int _currentRecordingNumber;
        PlaybackPanel _playbackPanel;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _playbackAudio = myDependencies.MyPlaybackAudio;
            _audioPlaybackStartDelayDuration = myDependencies.AudioPlaybackStartDelay;
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _delayBeforePlayback = myDependencies.DelayBeforePlaybackDuration;
            _audioRecorder = myDependencies.MyAudioRecorder;
            _currentPlaybackAvatar = _dialogueMediator.GetAvatarTwo();
            _playbackPanel = myDependencies.MyPlaybackPanel;
            _hasBeenSetup = true;
        }


        public IEnumerator PlaybackAll()
        {
            // _dialogueMediator.PlaybackAllRecordings();
            _logger.Log("PlaybackAll Coroutine started");
            _playbackMode = PlaybackMode.All;
            for (_currentRecordingNumber = 0; _currentRecordingNumber < _dialogueMediator.GetNumberOfRecordings(); ++_currentRecordingNumber)
            {
                _currentPlaybackAvatar = _dialogueMediator.GetRecordingOwner(_currentRecordingNumber);
                _logger.Log("PlaybackAll avatar: " + _currentPlaybackAvatar.name + " for recording number: " + _currentRecordingNumber);
                _conversationStateMachine.MoveNext(Command.BeginDelayBeforePlayback);
                yield return new WaitForSeconds(_delayBeforePlayback);
                _playbackAudio.PlaybackRecording(_currentRecordingNumber);
                var waitForDuration = _playbackAudio.CurrentClipLength;
                _logger.Log("Playback controls waiting for: " + waitForDuration);
                yield return new WaitForSeconds(waitForDuration);
            }
            _playbackMode = PlaybackMode.Single;
            // _conversationStateMachine.MoveNext(Command.BeginWalkingAround);
            _playbackPanel.EnablePlayButton();
            _playbackPanel.DisableStopButton();
        }

        public void StopPlayback()
        {
            StopAllCoroutines();
            _playbackAudio.StopAllPlayback();
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.Playback && _playbackMode == PlaybackMode.Single)
            {
                _currentPlaybackAvatar = _audioRecorder.GetCurrentRecordedAvatar();
                _logger.Log("Current playback avatar: " + _currentPlaybackAvatar.name);
                _dialogueMediator.PlaybackLastRecordingAfterDelay(_audioPlaybackStartDelayDuration);
            }
        }

        public GameObject GetCurrentPlaybackAvatar()
        {
            if (_playbackMode == PlaybackMode.Single)
            {
                var numberOfRecordings = _dialogueMediator.GetRecordingList().Count;
                _currentPlaybackAvatar = _dialogueMediator.GetRecordingOwner(numberOfRecordings - 1);
            }
            else
            {
                _currentPlaybackAvatar = _dialogueMediator.GetRecordingOwner(_currentRecordingNumber);
            }
            return _currentPlaybackAvatar;
        }

        void OnDisable()
        {
            _conversationStateMachine.OnStateEntered -= OnNewState;
        }

        void OnEnable()
        {
            if (!_hasBeenSetup)
                return;
            _conversationStateMachine.OnStateEntered -= OnNewState;
            _conversationStateMachine.OnStateEntered += OnNewState;
        }
    }
}