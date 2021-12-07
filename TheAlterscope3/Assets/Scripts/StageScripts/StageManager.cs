using UnityEngine;

namespace TheAlterscope2
{
    public class StageManager : MonoBehaviour, IHasSetup
    {
        DialogueMediator _dialogueMediator;
        ILogger _logger;
        ConversationStateMachine _conversationStateMachine;
        bool _hasBeenSetup;
        StageEvents _stageEvents;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _stageEvents = myDependencies.MyStageEvents;
            _hasBeenSetup = true;
        }

        void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.AnimateSlideIn)
            {
                _logger.Log("State machine delegate - rotate in!");
                _stageEvents.PlayRevolveOutEffects();
            }
            if (newState == ConversationState.AnimateSlideOut)
            {
                _logger.Log("State machine delegate - rotate out!");
                _stageEvents.PlayRevolveInEffects();
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