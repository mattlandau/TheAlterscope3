using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class PlaybackAudio : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        AudioSource _audioSource;
        DialogueMediator _dialogueMediator;
        bool _alter;
        public bool IsCurrentlyPlaying;
        public bool IsAvatarOnePlaying;
        public bool IsAvatarTwoPlaying;
        public float CurrentClipLength { get; private set; }
        FadeAudioSource _fader;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _fader = myDependencies.MyFadeAudioSource;
            _dialogueMediator = myDependencies.MyDialogueMediator;
        }

        public void PlaybackRecording(int recordingNumber)
        {
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myRecordings = _dialogueMediator.GetRecordingList();
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> currentRecording = new List<(GameObject, float[], float[])>();
            currentRecording.Add(myRecordings[recordingNumber]);
            StartCoroutine(PlaybackTheseRecordingsCoroutine(currentRecording));
        }

        public void PlaybackAllRecordings()
        {
            if (IsCurrentlyPlaying == true)
                return;

            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myRecordings = _dialogueMediator.GetRecordingList();
            StartCoroutine(PlaybackTheseRecordingsCoroutine(myRecordings));
        }

        public void StopAllPlayback()
        {
            StopAllCoroutines();
            _audioSource?.Stop();
        }

        public void PlaybackLastRecording()
        {
            _logger.Log("PlaybackLastRecording");
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myRecordings = _dialogueMediator.GetRecordingList();
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> lastRecording = new List<(GameObject, float[], float[])>();
            lastRecording.Add(myRecordings[myRecordings.Count - 1]);
            StartCoroutine(PlaybackTheseRecordingsCoroutine(lastRecording));
        }

        public void PlaybackLastRecordingAfterDelay(float delayDuration)
        {
            _logger.Log("PlaybackLastRecordingAfterDelay, delay length: " + delayDuration);
            StartCoroutine(AsynchronousPlaybackLastRecordingAfterDelay(delayDuration));
        }

        IEnumerator AsynchronousPlaybackLastRecordingAfterDelay(float delayDuration)
        {
            yield return new WaitForSeconds(delayDuration);
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myRecordings = _dialogueMediator.GetRecordingList();
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> lastRecording = new List<(GameObject, float[], float[])>();
            lastRecording.Add(myRecordings[myRecordings.Count - 1]);
            StartCoroutine(PlaybackTheseRecordingsCoroutine(lastRecording));
        }

        public void PlaybackMostRecent(int numberToPlay)
        {
            _logger.Log("PlaybackMostRecent " + numberToPlay);
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myRecordings = _dialogueMediator.GetRecordingList();
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myTargetRecordings = new List<(GameObject, float[], float[])>();
            var orignalRecordingsCount = myRecordings.Count;
            for (var i = 0; i < orignalRecordingsCount; ++i)
            {
                if (i >= orignalRecordingsCount - numberToPlay)
                    myTargetRecordings.Add(myRecordings[i]);
            }
            _logger.Log($"total recording count: {myRecordings.Count}");
            _logger.Log($"most recent recording count: {myTargetRecordings.Count}");
            StartCoroutine(PlaybackTheseRecordingsCoroutine(myTargetRecordings));
        }

        public void PlaybackAvatar(GameObject targetAvatar)
        {
            _logger.Log("PlaybackAvatar " + targetAvatar);
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myRecordings = _dialogueMediator.GetRecordingList();
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myTargetRecordings = new List<(GameObject, float[], float[])>();
            Stack<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myTargetRecordingsStack = new Stack<(GameObject, float[], float[])>();
            var orignalRecordingsCount = myRecordings.Count;
            bool foundTargetRecording = false;
            for (var i = orignalRecordingsCount - 1; i >= 0; --i)
            {
                if (myRecordings[i].targetAvatar == targetAvatar)
                {
                    foundTargetRecording = true;
                    myTargetRecordingsStack.Push(myRecordings[i]);
                }
                if (foundTargetRecording && myRecordings[i].targetAvatar != targetAvatar)
                    break;
            }
            while (myTargetRecordingsStack.Count > 0)
            {
                myTargetRecordings.Add(myTargetRecordingsStack.Pop());
            }

            if (myTargetRecordings.Count == 0)
            {
                _dialogueMediator.MoveToNextState(Command.BeginDelayBeforePlayback);
                return;
            }
            _logger.Log($"total recording count: {myRecordings.Count}");
            _logger.Log($"most recent recording count: {myTargetRecordings.Count}");
            StartCoroutine(PlaybackTheseRecordingsCoroutine(myTargetRecordings));
        }

        public void PlaybackMostRecentFullTurn()
        {
            _logger.Log("PlaybackMostRecentFullTurn");
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myRecordings = _dialogueMediator.GetRecordingList();
            List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myTargetRecordings = new List<(GameObject, float[], float[])>();
            bool changeOne = false;
            bool changeTwo = false;
            var orignalRecordingsCount = myRecordings.Count;
            bool[] includeRecording = new bool[myRecordings.Count];
            includeRecording[orignalRecordingsCount - 1] = true;
            for (var i = orignalRecordingsCount - 1; i >= 1; --i)
            {
                if (myRecordings[i].targetAvatar != myRecordings[i - 1].targetAvatar)
                {
                    if (changeOne == false)
                        changeOne = true;
                    else if (changeTwo == false)
                        changeTwo = true;
                }
                includeRecording[i - 1] = !(changeOne && changeTwo);
            }

            for (var i = 0; i < orignalRecordingsCount; ++i)
            {
                if (includeRecording[i] == true)
                    myTargetRecordings.Add(myRecordings[i]);
            }
            _logger.Log($"total recording count: {myRecordings.Count}");
            _logger.Log($"most recent recording count: {myTargetRecordings.Count}");
            StartCoroutine(PlaybackTheseRecordingsCoroutine(myTargetRecordings));
        }

        IEnumerator PlaybackTheseRecordingsCoroutine(List<(GameObject targetAvatar, float[] audioSamples, float[] alteredAudioSamples)> myRecordings)
        {
            IsCurrentlyPlaying = true;
            for (var i = 0; i < myRecordings.Count; ++i)
            {
                var recording = myRecordings[i];
                float pitch = _dialogueMediator.GetCurrentPitch(recording.targetAvatar);
                _audioSource = (recording.targetAvatar == _dialogueMediator.GetAvatarOne()) ? _dialogueMediator.GetAvatarOneAudioSource() : _dialogueMediator.GetAvatarTwoAudioSource();
                float[] currentAudioSamples = _dialogueMediator.GetPitchLevel(recording.targetAvatar) == VoicePitch.Low ? recording.alteredAudioSamples : recording.audioSamples;

                _audioSource.clip = AudioClip.Create($"trimmed clip ({pitch}) {(i + 1)} {recording.targetAvatar.name}", currentAudioSamples.Length, 1, _dialogueMediator.GetSampleRate(), false);
                _audioSource.clip.SetData(currentAudioSamples, 0);
                _audioSource.pitch = pitch;
                var adjustedClipLength = _audioSource.clip.length / pitch;
                CurrentClipLength = adjustedClipLength;
                StartCoroutine(_fader.FadeInAndOut(_audioSource, CurrentClipLength));
                _audioSource.Play();
                IsAvatarOnePlaying = recording.targetAvatar == _dialogueMediator.GetAvatarOne();
                IsAvatarTwoPlaying = recording.targetAvatar == _dialogueMediator.GetAvatarTwo();
                _logger.Log($"playing back recording number {(i + 1)} with length: {_audioSource.clip.length}, adjusted to: {adjustedClipLength} on {recording.targetAvatar.name} with pitch {pitch}");
                yield return new WaitForSeconds(adjustedClipLength);
                IsAvatarOnePlaying = false;
                IsAvatarTwoPlaying = false;

                if ((i + 1) < myRecordings.Count) // this is so that we don't add delay after the last turn
                {
                    _logger.Log($"delay between turns of {_dialogueMediator.GetDelayBetweenTurns()}");
                    yield return new WaitForSeconds(_dialogueMediator.GetDelayBetweenTurns());
                }
                _logger.Log($"playback of recording number {(i + 1)} of {myRecordings.Count} is done");
            }
            IsCurrentlyPlaying = false;
            _dialogueMediator.MoveToNextState(Command.BeginDelayAfterPlayback);
        }
    }
}