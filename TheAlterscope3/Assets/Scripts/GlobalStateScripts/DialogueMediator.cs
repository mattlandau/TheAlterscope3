using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class DialogueMediator : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        AudioRecorder _audioRecorder;
        PlaybackAudio _playbackAudio;
        GameObject _avatarOne;
        GameObject _avatarTwo;
        AudioSource _avatarOneAudioSource;
        AudioSource _avatarTwoAudioSource;
        AudioRecordingList _audioRecordingList;
        ConstantPitchTimeScale _timeScaleModifier;
        Dependencies _dependencies;
        ConversationStateMachine _conversationStateMachine;
        bool _hasBeenSetup;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _avatarOne = myDependencies.AvatarOne;
            _avatarTwo = myDependencies.AvatarTwo;
            _avatarOneAudioSource = myDependencies.AvatarOneAudioSource;
            _avatarTwoAudioSource = myDependencies.AvatarTwoAudioSource;
            _audioRecordingList = myDependencies.MyAudioRecordingList;
            _audioRecorder = myDependencies.MyAudioRecorder;
            _playbackAudio = myDependencies.MyPlaybackAudio;
            _timeScaleModifier = myDependencies.MyTimeScaleConstantPitch;
            _dependencies = myDependencies;
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _logger.Log("DialogueMediator has been setup");
            _hasBeenSetup = true;
        }

        public void AddRecording(GameObject targetAvatar, float[] audioSamples) => _audioRecordingList.AddRecording(targetAvatar, audioSamples);
        public void StartRecording(GameObject targetAvatar) => _audioRecorder.StartRecording(targetAvatar);
        public bool IsInitialLaunch() => _dependencies.MyLaunchFactory.IsInitialLaunch;
        public void StopRecording() => _audioRecorder.StopRecording();
        public int GetNumberOfRecordings() => _audioRecordingList.GetNumberOfRecordings();
        public GameObject GetRecordingOwner(int recordingNumber) => _audioRecordingList.GetRecordingOwner(recordingNumber);
        public List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> GetRecordingList() => _audioRecordingList.AudioRecordings;
        public GameObject GetAvatarOne() => _avatarOne;
        public GameObject GetAvatarTwo() => _avatarTwo;
        public AudioSource GetAvatarOneAudioSource() => _avatarOneAudioSource;
        public AudioSource GetAvatarTwoAudioSource() => _avatarTwoAudioSource;
        public int GetSampleRate() => _dependencies.MySampleRate;
        public float GetDelayBetweenTurns() => _dependencies.DelayBetweenTurns;
        public void SetPitchModiferSamples(float[] inputSamples) => _timeScaleModifier.Create(inputSamples);
        public void SetPitchModifierPitch(float pitch) => _timeScaleModifier.SetChangeFactor(1f / pitch);
        public void GetPitchModifiedAudio(out float[] modifiedAudio) => _timeScaleModifier.GetModifiedAudio(out modifiedAudio);
        public float GetCurrentPitch(GameObject targetAvatar) => targetAvatar == _avatarOne ? _dependencies.MaleVoicePitch : _dependencies.FemaleVoicePitch;
        public int GetRecordingCount() => _audioRecordingList.AudioRecordings.Count;
        public GameObject GetCurrentPlaybackAvatar() => _dependencies.MyPlaybackControls.GetCurrentPlaybackAvatar();
        public void PlaybackLastRecordingAfterDelay(float delayDuration) => _playbackAudio.PlaybackLastRecordingAfterDelay(delayDuration);
        public VoicePitch GetPitchLevel(GameObject targetAvatar) => targetAvatar == _avatarOne ? VoicePitch.Low : VoicePitch.High;
        public float GetPitch(VoicePitch targetPitchLevel) => targetPitchLevel == VoicePitch.Low ? _dependencies.MaleVoicePitch : _dependencies.FemaleVoicePitch;
        public ConversationState MoveToNextState(Command stateCommand) => _conversationStateMachine.MoveNext(stateCommand);
        public ConversationState GetState() => _conversationStateMachine.CurrentState;
        public ConversationState GetPreviousState() => _conversationStateMachine.PreviousState;
        public bool IsAvatarOneEmbodied() => _dependencies.MyEmbodimentDetector.IsEmbodied(_avatarOne);
        public bool IsAvatarTwoEmbodied() => _dependencies.MyEmbodimentDetector.IsEmbodied(_avatarTwo);
        public void AdjustAvatarLocationsAndStagePivot() => _dependencies.MyStageCenterPointController.AdjustAvatarLocationsAndStagePivot();
        public void TriggerBubbleHide() => _dependencies.MySpeechBubbleModifier.TriggerHide();
        public void LowerPositions() => _dependencies.MyAdjustDeskHeight.LowerDesk();

    }
}
