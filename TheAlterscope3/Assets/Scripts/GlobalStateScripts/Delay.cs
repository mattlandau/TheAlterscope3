using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class Delay : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        float _delayBeforePlayback;
        float _delayAfterPlayback;
        float _delayAfterRecording;
        DialogueMediator _dialogueMediator;
        ConversationStateMachine _conversationStateMachine;
        EmbodimentDetector _embodimentDetector;
        GameObject _avatarOne;
        GameObject _avatarTwo;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _delayBeforePlayback = myDependencies.DelayBeforePlaybackDuration;
            _delayAfterPlayback = myDependencies.DelayAfterPlaybackDuration;
            _delayAfterRecording = myDependencies.DelayAfterRecordingDuration;
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _embodimentDetector = myDependencies.MyEmbodimentDetector;
            _avatarOne = _dialogueMediator.GetAvatarOne();
            _avatarTwo = _dialogueMediator.GetAvatarTwo();
            _hasBeenSetup = true;
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.DelayBeforePlayback)
            {
                _logger.Log("Delay state entered: " + _delayBeforePlayback);
                StartCoroutine(WaitUntilDelayCompleted(_delayBeforePlayback));
            }
            if (newState == ConversationState.DelayAfterPlayback)
            {
                _logger.Log("Delay state entered: " + _delayAfterPlayback);
                StartCoroutine(WaitUntilDelayCompleted(_delayAfterPlayback));
            }
            if (newState == ConversationState.DelayAfterRecording)
            {
                _logger.Log("Delay state entered: " + _delayAfterRecording);
                StartCoroutine(WaitUntilDelayCompleted(_delayAfterRecording));
            }
        }

        IEnumerator WaitUntilDelayCompleted(float delaySeconds)
        {
            _logger.Log($"begin delay of {delaySeconds} seconds");
            yield return new WaitForSeconds(delaySeconds);
            _logger.Log($"end delay of {delaySeconds} seconds");
            NextStateAfterDelay();
        }

        void NextStateAfterDelay()
        {
            if (_conversationStateMachine.CurrentState == ConversationState.DelayBeforePlayback)
                _conversationStateMachine.MoveNext(Command.BeginPlayback);

            if (_conversationStateMachine.CurrentState == ConversationState.DelayAfterPlayback)
            {
                if (_embodimentDetector.IsEmbodied(_avatarOne) || _embodimentDetector.IsEmbodied(_avatarTwo))
                    _conversationStateMachine.MoveNext(Command.BeginReadyToRecord);
                else
                    _conversationStateMachine.MoveNext(Command.BeginWalkingAround);
            }

            if (_conversationStateMachine.CurrentState == ConversationState.DelayAfterRecording)
                if (_embodimentDetector.MostRecentlyEmbodiedAvatar == _dialogueMediator.GetAvatarOne())
                    _conversationStateMachine.MoveNext(Command.BeginSlideIn);
                else
                    _conversationStateMachine.MoveNext(Command.BeginSlideOut);
        }

        // void OnDisable()
        // {
        //     _conversationStateMachine.OnStateEntered -= OnNewState;
        // }

        // void OnEnable()
        // {
        //     if (!_hasBeenSetup)
        //         return;

        //     _conversationStateMachine.OnStateEntered -= OnNewState;
        //     _conversationStateMachine.OnStateEntered += OnNewState;
        // }
    }
}
