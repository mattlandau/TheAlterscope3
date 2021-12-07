using System;
using UnityEngine;

namespace TheAlterscope2
{
    public class EmbodimentDetector : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        DialogueMediator _dialogueMediator;
        [SerializeField]
        bool _isAvatarOneEmbodied;
        [SerializeField]
        bool _isAvatarTwoEmbodied;
        GameObject _avatarOne;
        GameObject _avatarTwo;
        GameObject _mainCamera;
        ConversationStateMachine _conversatationStateMachine;
        UserPreferences _userPreferences;
        bool _hasBeenSetup;
        public GameObject MostRecentlyEmbodiedAvatar;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _avatarOne = _dialogueMediator.GetAvatarOne();
            _avatarTwo = _dialogueMediator.GetAvatarTwo();
            _mainCamera = myDependencies.MyCamera;
            _conversatationStateMachine = myDependencies.MyConversationStateMachine;
            _userPreferences = myDependencies.MyUserPreferences;
            _hasBeenSetup = true;
        }

        public void PlayEmbodiementEffect(GameObject targetAvatar)
        {
            _logger.Log("Embodiment effect played for " + targetAvatar.name);
            _userPreferences.CurrentHapticFeedback().Trigger(iOSHapticFeedback.iOSFeedbackType.Success);

            if (_conversatationStateMachine.CurrentState == ConversationState.DelayAfterPlayback)
                _conversatationStateMachine.MoveNext(Command.BeginReadyToRecord);
        }

        public void SetEmbodiment(GameObject targetAvatar, bool isEmbodied)
        {
            if (targetAvatar == _dialogueMediator.GetAvatarOne())
                _isAvatarOneEmbodied = isEmbodied;
            else
                _isAvatarTwoEmbodied = isEmbodied;

            MostRecentlyEmbodiedAvatar = _isAvatarOneEmbodied ? _avatarOne : _avatarTwo;
        }

        public bool IsEmbodied(GameObject targetAvatar)
        {
            return targetAvatar == _avatarOne ? _isAvatarOneEmbodied : _isAvatarTwoEmbodied;
        }

        public void RefreshEmbodiment()
        {
            var avatarOneBounds = _avatarOne.GetComponent<BoxCollider>().bounds;
            var isWithinAvatarOne = avatarOneBounds.Contains(_mainCamera.transform.position);
            var avatarTwoBounds = _avatarTwo.GetComponent<BoxCollider>().bounds;
            var isWithinAvatarTwo = avatarTwoBounds.Contains(_mainCamera.transform.position);

            _isAvatarOneEmbodied = false;
            _isAvatarTwoEmbodied = false;

            if (isWithinAvatarOne && isWithinAvatarTwo)
                throw new ArgumentOutOfRangeException("both avatars embodies simulataneously");
            else if (isWithinAvatarOne)
                _isAvatarOneEmbodied = true;
            else if (isWithinAvatarTwo)
                _isAvatarTwoEmbodied = true;

            MostRecentlyEmbodiedAvatar = _isAvatarOneEmbodied ? _avatarOne : _avatarTwo;
        }
    }
}
