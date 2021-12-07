using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class FadeAudioSource : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        float _fadeInDuration, _fadeOutDuration;
        // ConversationStateMachine _conversationStateMachine;

        // 1. Add he module to the appropriate game object
        // 2. Add a field for the module to the Dependencies module
        // 3. Fill in the Dependencies field with the module
        // 4. In the "factory" for that object, call the Setup
        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _fadeInDuration = myDependencies.FadeInDuration;
            _fadeOutDuration = myDependencies.FadeOutDuration;
            _hasBeenSetup = true;
        }

        public IEnumerator FadeInAndOut(AudioSource audioSource, float clipLength)
        {
            float currentTime = 0;
            // float start = audioSource.volume
            // _logger.Log($"audio volume {start}");
            while (currentTime < _fadeInDuration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0f, 1f, currentTime / _fadeInDuration);
                yield return null;
            }

            var timeRemaining = clipLength - _fadeInDuration;

            if (timeRemaining < _fadeOutDuration)
                yield break;

            yield return new WaitForSeconds(timeRemaining - _fadeOutDuration);

            currentTime = 0f;
            while (currentTime < _fadeInDuration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(1f, 0f, currentTime / _fadeInDuration);
                yield return null;
            }
        }
    }
}