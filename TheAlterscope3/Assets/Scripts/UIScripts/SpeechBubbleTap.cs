// #define SCREENSHOTS
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;

namespace TheAlterscope2
{
    public class SpeechBubbleTap : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        DialogueMediator _dialogueMediator;
        Animator _expandBubble;
        Animator _avatarOneListen;
        Animator _avatarTwoListen;
        GameObject _speechPanel;
        GameObject _leftCancel;
        GameObject _rightCancel;
        GameObject _topCancel;
        UserPreferences _userPreferences;
        bool _cancel;
        bool _hasBeenSetup;
        bool _permissionCancelButtonBugAvoided;
        ApplicationBehaviors _applicationBehaviors;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _expandBubble = myDependencies.ExpandBubble;
            _avatarOneListen = myDependencies.AvatarOneListen;
            _avatarTwoListen = myDependencies.AvatarTwoListen;
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _speechPanel = myDependencies.SpeechButtonPanel;
            _leftCancel = myDependencies.SpeechButtonLeftCancelBoundary;
            _rightCancel = myDependencies.SpeechButtonRightCancelBoundary;
            _topCancel = myDependencies.SpeechButtonTopCancelBoundary;
            _userPreferences = myDependencies.MyUserPreferences;
            _applicationBehaviors = myDependencies.MyApplicationBehaviors;
            EnhancedTouchSupport.Enable();
            _hasBeenSetup = true;
        }

        public void PressDownTalkButton()
        {
            _logger.Log("talk button press down!!");
            _cancel = false;
            ClearTriggers();
            _expandBubble.SetTrigger("TriggerExpand");
            _avatarOneListen.SetTrigger("TriggerListen");
            _avatarTwoListen.SetTrigger("TriggerListen");
            if (_dialogueMediator.GetState() == ConversationState.ReadyToRecord)
            {
                _logger.Log("trigger vibe");
                _userPreferences.CurrentHapticFeedback().Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactLight);
            }
        }

        bool CheckForCancel()
        {
            bool result = false;
#if !UNITY_EDITOR
            _logger.Log("END DRAG position: " + UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition.ToString());
            var endDragScreenPosition = new Vector2(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition.x, UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition.y);
#endif
#if UNITY_EDITOR
            var endDragScreenPosition = new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue());
#endif
            _logger.Log("End drag position " + endDragScreenPosition);

            if (endDragScreenPosition.x < _leftCancel.transform.position.x || endDragScreenPosition.x > _rightCancel.transform.position.x || endDragScreenPosition.y > _topCancel.transform.position.y)
            {
                if (_applicationBehaviors.HasBeenPausedAndNoFurtherInteractions) //permissions popup cause an issue with drag positions
                    result = false;
                else
                    result = true;
            }
            else
                result = false;

            _logger.Log("Cancel - " + result);
            _applicationBehaviors.HasBeenPausedAndNoFurtherInteractions = false;
#if SCREENSHOTS
            return false;
#else
            return result;
#endif
        }

        public void PressUpTalkButton()
        {
            _logger.Log("talk button press up!");
            ClearTriggers();
            _expandBubble.SetTrigger("TriggerShrink");

            if (CheckForCancel() == true)
                return;

            if (_dialogueMediator.GetState() == ConversationState.ReadyToRecord)
            {
                _logger.Log("trigger vibe");
                _userPreferences.CurrentHapticFeedback().Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactLight);
                GameObject targetAvatar;
                targetAvatar = _dialogueMediator.IsAvatarOneEmbodied() ? _dialogueMediator.GetAvatarOne() : _dialogueMediator.GetAvatarTwo();
                _dialogueMediator.StartRecording(targetAvatar);
                return;
            }

            if (_dialogueMediator.GetState() == ConversationState.Recording)
            {
                _dialogueMediator.StopRecording();
                return;
            }
        }

        void ClearTriggers()
        {
            _expandBubble.ResetTrigger("TriggerExpand");
            _expandBubble.ResetTrigger("TriggerShrink");
            _avatarOneListen.ResetTrigger("TriggerListen");
            _avatarTwoListen.ResetTrigger("TriggerListen");
            _avatarOneListen.ResetTrigger("TriggerAtEase");
            _avatarTwoListen.ResetTrigger("TriggerAtEase");

        }
    }
}