using UnityEngine;
using UnityEngine.UI;

namespace TheAlterscope2
{
    public class SpeechBubbleModifier : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        Sprite _canRecordBubble;
        Sprite _isRecordingBubble;
        GameObject _bubbleMicrophone;
        GameObject _vuDots;
        Button _speechBubble;
        Animator _speechBubbleSlider;
        Animator _instructions;
        ConversationStateMachine _conversationStateMachine;
        EmbodimentDetector _embodimentDetector;
        GameObject _avatarOne;
        GameObject _avatarTwo;
        GameObject _speechBubblePanel;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _canRecordBubble = myDependencies.CanRecordBubble;
            _isRecordingBubble = myDependencies.IsRecordingBubble;
            _bubbleMicrophone = myDependencies.BubbleMicrophone;
            _vuDots = myDependencies.VUPanel;
            _speechBubble = myDependencies.MySpeechBubble;
            _speechBubbleSlider = myDependencies.SpeechBubbleSlider;
            _instructions = myDependencies.Instructions;
            _embodimentDetector = myDependencies.MyEmbodimentDetector;
            _avatarOne = myDependencies.MyDialogueMediator.GetAvatarOne();
            _avatarTwo = myDependencies.MyDialogueMediator.GetAvatarTwo();
            _speechBubblePanel = myDependencies.SpeechButtonPanel;
            _hasBeenSetup = true;
        }

        void ClearTriggers()
        {
            _logger.Log("Clear SpeechBubbleSlider triggers");
            _speechBubbleSlider.ResetTrigger("TriggerShow");
            _speechBubbleSlider.ResetTrigger("TriggerHide");
            _instructions.ResetTrigger("TriggerShow");
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.Recording)
            {
                _speechBubble.image.sprite = _isRecordingBubble;
                _vuDots.SetActive(true);
                _bubbleMicrophone.SetActive(false);
            }
            if (newState == ConversationState.ReadyToRecord)
            {
                _speechBubble.image.sprite = _canRecordBubble;
                _vuDots.SetActive(false);
                _bubbleMicrophone.SetActive(true);
                TriggerShow();
            }

            if (newState == ConversationState.DelayAfterRecording)
            {
                TriggerHide();
            }
        }

        public void TriggerShow()
        {
            if (_conversationStateMachine.CurrentState == ConversationState.ReadyToRecord)
            {
                ClearTriggers();
                _speechBubbleSlider.SetTrigger("TriggerShow");
                _instructions.SetTrigger("TriggerShow");
                _logger.Log("TriggerShow");
            }
        }

        public void TriggerHide()
        {
            if (_conversationStateMachine.CurrentState != ConversationState.Recording)
            {
                ClearTriggers();
                _speechBubbleSlider.SetTrigger("TriggerHide");
                _logger.Log("TriggerHide");
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