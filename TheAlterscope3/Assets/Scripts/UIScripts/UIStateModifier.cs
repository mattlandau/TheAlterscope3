using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    // public enum UIState
    // {
    //     Interaction,
    //     Playback
    // }

    public class UIStateModifier : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        ConversationStateMachine _conversationStateMachine;
        Animator _uiStateAnimator;
        Animator _speechBubbleSlider;
        Animator _panelAnimator;
        DialogueMediator _dialogueMediator;
        PlaybackPanel _playbackPanel;

        // 1. Add he module to the appropriate game object
        // 2. Add a field for the module to the Dependencies module
        // 3. Fill in the Dependencies field with the module
        // 4. In the "factory" for that object, call the Setup
        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _uiStateAnimator = myDependencies.UIStateAnimator;
            _speechBubbleSlider = myDependencies.SpeechBubbleSlider;
            _panelAnimator = myDependencies.PlaybackPanelSlider;
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _playbackPanel = myDependencies.MyPlaybackPanel;
            // TriggerInteraction();
            _hasBeenSetup = true;
        }

        void ClearTriggers()
        {
            _logger.Log("Clear UIState triggers");
            _uiStateAnimator.ResetTrigger("TriggerInteraction");
            _uiStateAnimator.ResetTrigger("TriggerPlayback");
            _speechBubbleSlider.ResetTrigger("TriggerShow");
            _speechBubbleSlider.ResetTrigger("TriggerHide");
            _panelAnimator.ResetTrigger("TriggerShow");
            _panelAnimator.ResetTrigger("TriggerHide");
        }

        public void OnNewState(ConversationState newState)
        {
            // _logger.Log("New state detected in UIStateModifier");

            if (newState == ConversationState.WalkingAround)
                TriggerPlayback();

            if (newState == ConversationState.ReadyToRecord)
                TriggerInteraction();

        }

        void TriggerInteraction()
        {
            _logger.Log("TriggerInteraction");
            ClearTriggers();
            _uiStateAnimator.SetTrigger("TriggerInteraction");
            _speechBubbleSlider.SetTrigger("TriggerShow");
            _panelAnimator.SetTrigger("TriggerHide");

        }

        void TriggerPlayback()
        {
            _logger.Log("TriggerPlayback");
            ClearTriggers();
            _uiStateAnimator.SetTrigger("TriggerPlayback");
            _speechBubbleSlider.SetTrigger("TriggerHide");
            if (_dialogueMediator.GetRecordingCount() > 0)
            {
                _playbackPanel.RefreshButtonStatus();
                _panelAnimator.SetTrigger("TriggerShow");
            }
        }

        void OnDisable()
        {
            // _conversationStateMachine.OnStateEntered -= OnNewState;
        }

        void OnEnable()
        {
            if (!_hasBeenSetup)
                return;

            // _conversationStateMachine.OnStateEntered -= OnNewState;
            // _conversationStateMachine.OnStateEntered += OnNewState;
        }

    }
}