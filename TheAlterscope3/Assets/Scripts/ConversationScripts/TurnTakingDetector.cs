using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public enum WhoRecordsNext
    {
        AvatarOne,
        AvatarTwo
    }

    public class TurnTakingDetector : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        ConversationStateMachine _conversationStateMachine;
        public WhoRecordsNext _whoRecordsNext, _previousWhoRecordsNext, _expectedToRecordsNext;
        EmbodimentDetector _embodimentDetector;
        GameObject _avatarOne, _avatarTwo;
        public bool IsTurnSwitched;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _whoRecordsNext = WhoRecordsNext.AvatarTwo;
            _expectedToRecordsNext = WhoRecordsNext.AvatarTwo;
            // _previousWhoRecordsNext = WhoRecordsNext.AvatarOne;
            _embodimentDetector = myDependencies.MyEmbodimentDetector;
            _avatarOne = myDependencies.AvatarOne;
            _avatarTwo = myDependencies.AvatarTwo;
            _hasBeenSetup = true;
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.ReadyToRecord)
            {
                _whoRecordsNext = _embodimentDetector.IsEmbodied(_avatarOne) ? WhoRecordsNext.AvatarOne : WhoRecordsNext.AvatarTwo;
                if (_whoRecordsNext == _expectedToRecordsNext)
                    IsTurnSwitched = false;
                else
                    IsTurnSwitched = true;

                _expectedToRecordsNext = _expectedToRecordsNext == WhoRecordsNext.AvatarOne ? WhoRecordsNext.AvatarTwo : WhoRecordsNext.AvatarOne;

                // if (_embodimentDetector.IsEmbodied(_avatarOne))
                // {
                //     if (_previousWhoRecordsNext == WhoRecordsNext.AvatarOne)
                //         IsTurnSwitched = true;
                //     else
                //         IsTurnSwitched = false;
                // }
                // if (_embodimentDetector.IsEmbodied(_avatarTwo))
                // {
                //     if (_previousWhoRecordsNext == WhoRecordsNext.AvatarTwo)
                //         IsTurnSwitched = true;
                //     else
                //         IsTurnSwitched = false;
                // }

                _previousWhoRecordsNext = _whoRecordsNext;
            }
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