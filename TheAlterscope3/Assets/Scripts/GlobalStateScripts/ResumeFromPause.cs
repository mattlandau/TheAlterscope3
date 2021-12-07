using UnityEngine;

namespace TheAlterscope2
{
    public class ResumeFromPause : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        DialogueMediator _dialogueMediator;
        bool _hasBeenSetup;
        ConversationStateMachine _conversationStateMachine;
        Animator _speechBubbleSlider;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _speechBubbleSlider = myDependencies.SpeechBubbleSlider;
            _hasBeenSetup = true;
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState != ConversationState.Placing)
                return;

            if (_conversationStateMachine.PreviousState == ConversationState.Recording)
            {
                _logger.Log("From Recording to Placing");
                _dialogueMediator.StopRecording();
                _speechBubbleSlider.SetTrigger("TriggerHide");
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