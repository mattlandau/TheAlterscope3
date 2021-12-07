using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class AudioRecordingList : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        DialogueMediator _dialogueMediator;
        // float _malePitch;
        // float _femalePitch;
        public List<(GameObject targetAvatar, float[] originalAudioSamples, float[] alteredAudioSamples)> AudioRecordings { get; set; }
        bool _hasBeenSetup;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _dialogueMediator = myDependencies.MyDialogueMediator;
            // _malePitch = myDependencies.MaleVoicePitch;
            // _femalePitch = myDependencies.FemaleVoicePitch;
            _hasBeenSetup = true;
        }

        void Awake()
        {
            AudioRecordings = new List<(GameObject, float[], float[])>();
        }

        public int GetNumberOfRecordings() => AudioRecordings.Count;

        public GameObject GetRecordingOwner(int recordingNumber) => AudioRecordings[recordingNumber].targetAvatar;


        public void AddRecording(GameObject targetAvatar, float[] audioSamples)
        {
            float[] alteredAudioSamples;
            _logger.Log("Audio samples lenght: " + audioSamples.Length);
            _dialogueMediator.SetPitchModiferSamples(audioSamples);
            _dialogueMediator.SetPitchModifierPitch(_dialogueMediator.GetPitch(VoicePitch.Low));
            _dialogueMediator.GetPitchModifiedAudio(out alteredAudioSamples);

            AudioRecordings.Add((targetAvatar, audioSamples, alteredAudioSamples));
        }

        public void LoadSavedRecordings(List<(GameObject targetAvatar, float[] originalAudioSamples, float[] alteredAudioSamples)> audioRecordings)
        {
            AudioRecordings.Clear();
            foreach (var recording in audioRecordings)
            {
                AudioRecordings.Add((recording.targetAvatar, recording.originalAudioSamples, recording.alteredAudioSamples));
            }
        }
    }
}