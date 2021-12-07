using UnityEngine;

namespace TheAlterscope2
{
    public class AnimateAvatars : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        Animator _avatarOneAnimator;
        Animator _avatarTwoAnimator;
        ConversationStateMachine _conversationStateMachine;
        DialogueMediator _dialogueMediator;
        Material _avatarOneMaterial;
        Material _avatarTwoMaterial;
        // readonly string _avatarOneHighlightStrength = "Vector1_38bd88ebcec14d33a45bbc39b4a598fd";
        // readonly string _avatarTwoHighlightStrength = "Vector1_38bd88ebcec14d33a45bbc39b4a598fd";
        readonly string _avatarOneHighlightStrength = "Vector1_4B7002A3";
        // readonly string _avatarTwoHighlightStrength = "Vector1_4B7002A3";
        // readonly string _avatarTwoHighlightStrength = "Vector1_1E6FF86";
        readonly string _avatarTwoHighlightStrength = "Vector1_5E7E876D";
        int _avatarOneHighlightStrengthHash;
        int _avatarTwoHighlightStrengthHash;
        readonly float _on = 1.0f;
        readonly float _off = 0f;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _avatarOneAnimator = myDependencies.AvatarOneAnimator;
            _avatarTwoAnimator = myDependencies.AvatarTwoAnimator;
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _avatarOneMaterial = myDependencies.AvatarOneMaterial;
            _avatarTwoMaterial = myDependencies.AvatarTwoMaterial;
            _avatarOneHighlightStrengthHash = Shader.PropertyToID(_avatarOneHighlightStrength);
            _avatarTwoHighlightStrengthHash = Shader.PropertyToID(_avatarTwoHighlightStrength);
            _hasBeenSetup = true;
        }

        public void TriggerIdleAnimations()
        {
            _avatarOneAnimator.SetTrigger("TriggerIdleA");
            _avatarTwoAnimator.SetTrigger("TriggerIdle");
        }

        void ResetAllTriggers()
        {
            _avatarOneAnimator.ResetTrigger("TriggerSpin");
            _avatarOneAnimator.ResetTrigger("TriggerBounce");
            _avatarOneAnimator.ResetTrigger("TriggerIdleA");
            _avatarTwoAnimator.ResetTrigger("TriggerTalk");
            _avatarTwoAnimator.ResetTrigger("TriggerIdle");
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.AnimateSlideIn || newState == ConversationState.AnimateSlideOut)
            {
                ResetAllTriggers();
                _avatarOneAnimator.SetTrigger("TriggerSpin");
            }

            if (newState == ConversationState.DelayAfterPlayback)
            {
                ResetAllTriggers();
                TriggerIdleAnimations();
                _avatarOneMaterial.SetFloat(_avatarOneHighlightStrengthHash, _off);
                _avatarTwoMaterial.SetFloat(_avatarTwoHighlightStrengthHash, _off);

            }

            if (newState == ConversationState.DelayBeforePlayback)
            {
                ResetAllTriggers();
                TriggerIdleAnimations();
            }

            if (newState == ConversationState.ReadyToRecord)
            {
                TriggerIdleAnimations();
                _avatarOneMaterial.SetFloat(_avatarOneHighlightStrengthHash, _off);
                _avatarTwoMaterial.SetFloat(_avatarTwoHighlightStrengthHash, _off);
            }

            if (newState == ConversationState.WalkingAround)
            {
                ResetAllTriggers();
                TriggerIdleAnimations();
                _avatarOneMaterial.SetFloat(_avatarOneHighlightStrengthHash, _off);
                _avatarTwoMaterial.SetFloat(_avatarTwoHighlightStrengthHash, _off);
            }

            if (newState == ConversationState.Playback)
            {
                ResetAllTriggers();
                if (_dialogueMediator.GetCurrentPlaybackAvatar() == _dialogueMediator.GetAvatarOne())
                {
                    _avatarOneAnimator.SetTrigger("TriggerBounce");
                    _avatarOneMaterial.SetFloat(_avatarOneHighlightStrengthHash, _on);

                }
                else
                {
                    _avatarTwoAnimator.SetTrigger("TriggerTalk");
                    _avatarTwoMaterial.SetFloat(_avatarTwoHighlightStrengthHash, _on);
                }
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