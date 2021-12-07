using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace TheAlterscope2
{
    public class AudioRecorder : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        AudioSource _audioSource;
        DialogueMediator _dialogueMediator;
        float[] _originalData;
        float[] _trimmedData;
        string _microphoneName;
        int _sampleRate;
        int _microphonePosition;
        GameObject _selectedAvatar;
        public bool IsRecording;
        public bool IsAvatarOneRecording;
        public bool IsAvatarTwoRecording;
        public bool WasRecordingSuccessful;
        bool _isMicrophoneInitialized;
        public VUMeter _vuMeter;
        int _count = 0;
        float[] _temp;
        bool _hasBeenSetup;
        int _framesPerVU;
        int _baseClipDuration;
        int _clipOverflowDuration; // this is a potential risk, overflows can lead to runtime errors
        int _endClipPosition;
        List<float[]> _recordingsList;
        ChangeMode _changeMode;
        LaunchFactory _launchFactory;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _audioSource = myDependencies.AudioSourceForInitialRecording;
            _audioSource.playOnAwake = false;
            _audioSource.mute = true;
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _sampleRate = myDependencies.MySampleRate;
            IsRecording = false;
            _vuMeter = myDependencies.MyVUMeter;
            _framesPerVU = myDependencies.FramesPerVU;
            _baseClipDuration = myDependencies.BaseClipDuration;
            _clipOverflowDuration = myDependencies.ClipOverflowBufferDuration;
            _endClipPosition = _baseClipDuration * _sampleRate;
            _recordingsList = new List<float[]>();
            _temp = new float[1];
            _microphoneName = Microphone.devices[0];
            _changeMode = myDependencies.MyChangeMode;
            _launchFactory = myDependencies.MyLaunchFactory;
            _hasBeenSetup = true;
        }

        IEnumerator InitializeMicrophoneAndThenRecord(GameObject targetAvatar)
        {
            if (_launchFactory.IsInitialLaunch)
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
                yield return new WaitForSeconds(0.25f);
                if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
                    _changeMode.EnterQuitMode();

            }

            StartRecordingAfterInitialization(targetAvatar);
        }

        void Update()
        {
            if (_hasBeenSetup == false)
                return;

            if (IsRecording == false)
                return;

            _count++;

            var currentMicrophonePosition = Microphone.GetPosition(_microphoneName);
            if (!(currentMicrophonePosition >= 0))
                return;

            if (currentMicrophonePosition > _endClipPosition)
                ExtendRecording();

            if (_count % _framesPerVU == 0)
            {
                _audioSource.clip.GetData(_temp, currentMicrophonePosition - 1);
                _vuMeter.ReceiveData(_temp[0]);
                _count = 0;
            }
        }

        public GameObject GetCurrentRecordedAvatar() => _selectedAvatar;

        void ExtendRecording()
        {
            Microphone.End(_microphoneName);
            float[] tempArray = new float[_endClipPosition];
            _audioSource.clip.GetData(tempArray, 0);
            _recordingsList.Add(tempArray);
            _audioSource.clip = Microphone.Start(_microphoneName, false, _baseClipDuration + _clipOverflowDuration, _sampleRate);
            while (!(Microphone.GetPosition(_microphoneName) > 0)) { } //prevent no microphone bug, supposedly
            _logger.Log("clip extended");
        }

        public void StartRecording(GameObject targetAvatar)
        {
            StartCoroutine(InitializeMicrophoneAndThenRecord(targetAvatar));
        }

        void StartRecordingAfterInitialization(GameObject targetAvatar)
        {
            _dialogueMediator.MoveToNextState(Command.BeginRecording);
            WasRecordingSuccessful = false;
            _microphoneName = Microphone.devices[0];
            _selectedAvatar = targetAvatar;
            _audioSource.clip = Microphone.Start(_microphoneName, false, _baseClipDuration + _clipOverflowDuration, _sampleRate);
            while (!(Microphone.GetPosition(_microphoneName) > 0)) { } //prevent no microphone bug, supposedly

            _logger.Log($"recording started on avatar {targetAvatar.name} with {_microphoneName}");
            IsRecording = true;
            IsAvatarOneRecording = targetAvatar == _dialogueMediator.GetAvatarOne();
            IsAvatarTwoRecording = targetAvatar == _dialogueMediator.GetAvatarTwo();
        }

        public void StopRecording()
        {
            _logger.Log("recording stopped");
            _microphonePosition = Microphone.GetPosition(_microphoneName);
            Microphone.End(_microphoneName);
            IsRecording = false;
            IsAvatarOneRecording = false;
            IsAvatarTwoRecording = false;
            _logger.Log("microphone position: " + _microphonePosition);
            if (_microphonePosition == 0)
            {
                WasRecordingSuccessful = false;
                _logger.Log("recording was not successful");
                return;
            }
            _logger.Log("recording was successful");
            _vuMeter.ClearMeterValues();
            WasRecordingSuccessful = true;
            AddTrimmedAudio();
            _dialogueMediator.MoveToNextState(Command.BeginDelayAfterRecording);
        }

        void AddTrimmedAudio()
        {
            float[] lastSegement = new float[_audioSource.clip.samples];
            _audioSource.clip.GetData(lastSegement, 0);
            _recordingsList.Add(lastSegement);
            _originalData = _recordingsList.SelectMany(i => i).ToArray();
            var finalMicrophonePosition = _microphonePosition + _endClipPosition * (_recordingsList.Count - 1);
            _trimmedData = new float[finalMicrophonePosition];
            Array.Copy(_originalData, _trimmedData, finalMicrophonePosition - 1);
            _recordingsList.Clear();
            _dialogueMediator.AddRecording(_selectedAvatar, _trimmedData);
        }
    }
}