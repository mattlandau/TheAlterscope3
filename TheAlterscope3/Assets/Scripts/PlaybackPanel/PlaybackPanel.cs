using UnityEngine.UI;
using UnityEngine;

namespace TheAlterscope2
{
    public class PlaybackPanel : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        ConversationStateMachine _conversationStateMachine;
        // Animator _panelAnimator;
        Button _stopButton;
        Button _playButton;
        DialogueMediator _dialogueMediator;
        PlaybackControls _playbackControls;
        Color _playButtonOriginalColor;
        Color _stopButtonOriginalColor;
        Color _gray;
        bool _stopEnabled = false;
        bool _playEnabled = true;
        EmbodimentDetector _embodimentDetector;
        GameObject _avatarOne;
        GameObject _avatarTwo;
        GameObject _playbackButtonsPanel;
        Animator _stopExpandAnimator;
        Animator _playExpandAnimator;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _dialogueMediator = myDependencies.MyDialogueMediator;
            // _panelAnimator = myDependencies.PlaybackPanelSlider;
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _stopButton = myDependencies.StopButton;
            _playButton = myDependencies.PlayButton;
            _playbackControls = myDependencies.MyPlaybackControls;
            _playButtonOriginalColor = _playButton.image.color;
            _stopButtonOriginalColor = _playButton.image.color;
            _gray.r = .4f;
            _gray.b = .4f;
            _gray.g = .4f;
            _gray.a = 1f;
            _embodimentDetector = myDependencies.MyEmbodimentDetector;
            _playbackButtonsPanel = myDependencies.PlaybackButtonsPanel;
            _avatarOne = _dialogueMediator.GetAvatarOne();
            _avatarTwo = _dialogueMediator.GetAvatarTwo();
            _stopExpandAnimator = myDependencies.StopButtonExpandAnimator;
            _playExpandAnimator = myDependencies.PlayButtonExpandAnimator;
            // DisablePlayButton();
            // DisableStopButton();
            _hasBeenSetup = true;
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.WalkingAround)
            {
                if (_embodimentDetector.IsEmbodied(_avatarOne) || _embodimentDetector.IsEmbodied(_avatarTwo))
                {
                    _conversationStateMachine.MoveNext(Command.BeginReadyToRecord);
                }
            }
            // if (newState == ConversationState.ReadyToRecord)
            // {
            //     ClearTriggers();
            //     _panelAnimator.SetTrigger("TriggerHide");
            // }
        }

        // public void ShowPlaybackPanel()
        // {
        //     DisableStopButton();
        //     ClearTriggers();
        //     _panelAnimator.SetTrigger("TriggerShow");
        // }

        // public void HidePlaybackPanel()
        // {
        //     ClearTriggers();
        //     _panelAnimator.SetTrigger("TriggerHide");
        // }

        public void RefreshButtonStatus()
        {
            if (_conversationStateMachine.CurrentState == ConversationState.Playback)
                EnableStopButton();
            else
                DisableStopButton();

            if (_dialogueMediator.GetNumberOfRecordings() > 0)
                EnablePlayButton();
            else
                DisablePlayButton();
        }

        public void DisableStopButton()
        {
            _stopButton.image.color = _gray;
            _stopEnabled = false;
            _logger.Log("Stop button disabled");
        }

        public void DisablePlayButton()
        {
            _playButton.image.color = _gray;
            _playEnabled = false;
            _logger.Log("Play button disabled");

        }

        public void EnableStopButton()
        {
            _stopButton.image.color = _stopButtonOriginalColor;
            _stopEnabled = true;
        }

        public void EnablePlayButton()
        {
            _playButton.image.color = _playButtonOriginalColor;
            _playEnabled = true;
        }

        public void ClickUpPlayButton()
        {
            if (_playEnabled == false)
                return;

            ClearTriggers();
            _playExpandAnimator.SetTrigger("TriggerShrink");
            StartCoroutine(_playbackControls.PlaybackAll());
            DisablePlayButton();
            EnableStopButton();
        }

        public void ClickUpStopButton()
        {
            if (_stopEnabled == false)
                return;

            ClearTriggers();
            _stopExpandAnimator.SetTrigger("TriggerShrink");
            StopAllCoroutines();
            _playbackControls.StopPlayback();
            _conversationStateMachine.MoveNext(Command.BeginWalkingAround);
            DisableStopButton();
            EnablePlayButton();
        }

        public void ClickDownPlayButton()
        {
            if (_playEnabled == false)
                return;

            ClearTriggers();
            _playExpandAnimator.SetTrigger("TriggerExpand");
        }

        public void ClickDownStopButton()
        {
            if (_stopEnabled == false)
                return;

            ClearTriggers();
            _stopExpandAnimator.SetTrigger("TriggerExpand");
        }

        void ClearTriggers()
        {
            // _panelAnimator.ResetTrigger("TriggerShow");
            // _panelAnimator.ResetTrigger("TriggerHide");
            _stopExpandAnimator.ResetTrigger("TriggerExpand");
            _stopExpandAnimator.ResetTrigger("TriggerShrink");
            _playExpandAnimator.ResetTrigger("TriggerExpand");
            _playExpandAnimator.ResetTrigger("TriggerShrink");
        }

        // public void HideWithPriority()
        // {
        //     _logger.Log("hide with priority");
        //     // RectTransform myTransform = _playbackButtonsPanel.GetComponent<RectTransform>();
        //     // myTransform.position = new Vector3(myTransform.position.x, -300f, myTransform.position.z);
        //     HidePlaybackPanel();
        //     EnableStopButton();
        //     EnablePlayButton();
        // }


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