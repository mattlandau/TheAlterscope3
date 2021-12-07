using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class AvatarEvents : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        GameObject _targetAvatar;
        DialogueMediator _dialogueMediator;
        GameObject _arCamera;
        ConversationStateMachine _conversatationStateMachine;
        Animator _targetAnimator;
        AvatarMaterialSwitcher _materialSwitcher;
        EmbodimentDetector _embodimentDetector;
        bool _hasBeenSetup;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _arCamera = myDependencies.MyCamera;
            _targetAvatar = this.gameObject == _dialogueMediator.GetAvatarOne() ? _dialogueMediator.GetAvatarOne() : _dialogueMediator.GetAvatarTwo();
            _targetAnimator = this.gameObject == _dialogueMediator.GetAvatarOne() ? myDependencies.AvatarOneListen : myDependencies.AvatarTwoListen;
            _conversatationStateMachine = myDependencies.MyConversationStateMachine;
            _conversatationStateMachine.OnStateEntered += OnNewState;
            _materialSwitcher = this.gameObject == _dialogueMediator.GetAvatarOne() ? myDependencies.MyAvatarOneMaterialSwitcher : myDependencies.MyAvatarTwoMaterialSwitcher;
            _embodimentDetector = myDependencies.MyEmbodimentDetector;
            // _avatarTwoMaterialSwitcher = myDependencies.MyAvatarTwoMaterialSwitcher;
            _hasBeenSetup = true;
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject != _arCamera)
                return;

            _embodimentDetector.PlayEmbodiementEffect(_targetAvatar);
            _embodimentDetector.SetEmbodiment(_targetAvatar, true);
            _logger.Log("Collider enter (embodiment): " + collider.name + " by avatar " + _targetAvatar.name);
            _materialSwitcher.SetVisibility(false);
            _logger.Log($"More info: collider attached to: {collider.gameObject.name} in module for target avatar: {_targetAvatar}");
            if (_dialogueMediator.GetState() == ConversationState.WalkingAround)
                _dialogueMediator.MoveToNextState(Command.BeginReadyToRecord);
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject == _dialogueMediator.GetAvatarOne() || collider.gameObject == _dialogueMediator.GetAvatarTwo())
                return;

            _dialogueMediator.TriggerBubbleHide();
            _logger.Log("Collider exit (embodiment): " + collider.name + " by avatar " + _targetAvatar.name);
            _embodimentDetector.SetEmbodiment(_targetAvatar, false);
            _materialSwitcher.SetVisibility(true);
            if (_dialogueMediator.GetState() == ConversationState.ReadyToRecord)
                _dialogueMediator.MoveToNextState(Command.BeginWalkingAround);
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.Recording)
            {
                // ClearTriggers();
                // _targetAnimator.SetTrigger("TriggerListen");
            }
            if (newState == ConversationState.DelayAfterRecording)
            {
                ClearTriggers();
                _targetAnimator.SetTrigger("TriggerAtEase");
            }
        }

        void ClearTriggers()
        {
            _targetAnimator.ResetTrigger("TriggerListen");
            _targetAnimator.ResetTrigger("TriggerAtEase");
        }
    }
}