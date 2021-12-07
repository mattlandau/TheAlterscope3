using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    [System.Serializable]
    public class ConversationData
    {
        public float[][] ArrayOfOriginalAudioSamples;
        public float[][] ArrayOfAlteredMaleAudioSamples;
        public float[][] ArrayOfAlteredFemaleAudioSamples;
        public string[] ArrayOfAvatarNames;
        public int NumberOfTurns;

        public ConversationData(List<(GameObject targetAvatar, float[] originalAudioSamples, float[] alteredMaleAudioSamples, float[] alteredFemaleAudioSamples)> audioRecordings)
        {
            NumberOfTurns = audioRecordings.Count;
            ArrayOfOriginalAudioSamples = new float[NumberOfTurns][];
            ArrayOfAlteredMaleAudioSamples = new float[NumberOfTurns][];
            ArrayOfAlteredFemaleAudioSamples = new float[NumberOfTurns][];
            ArrayOfAvatarNames = new string[NumberOfTurns];

            for (var i = 0; i < NumberOfTurns; ++i)
            {
                ArrayOfOriginalAudioSamples[i] = new float[audioRecordings[i].originalAudioSamples.Length];
                ArrayOfAlteredMaleAudioSamples[i] = new float[audioRecordings[i].alteredMaleAudioSamples.Length];
                ArrayOfAlteredFemaleAudioSamples[i] = new float[audioRecordings[i].alteredFemaleAudioSamples.Length];

                audioRecordings[i].originalAudioSamples.CopyTo(ArrayOfOriginalAudioSamples[i], 0);
                audioRecordings[i].alteredMaleAudioSamples.CopyTo(ArrayOfAlteredMaleAudioSamples[i], 0);
                audioRecordings[i].alteredFemaleAudioSamples.CopyTo(ArrayOfAlteredFemaleAudioSamples[i], 0);

                ArrayOfAvatarNames[i] = audioRecordings[i].targetAvatar.name;
            }
        }
    }
}