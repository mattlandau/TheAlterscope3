using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class Template : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        ConversationStateMachine _conversationStateMachine;

        // 1. Add he module to the appropriate game object
        // 2. Add a field for the module to the Dependencies module
        // 3. Fill in the Dependencies field with the module
        // 4. In the "factory" for that object, call the Setup
        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _hasBeenSetup = true;
        }

        public void OnNewState(ConversationState newState)
        {

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