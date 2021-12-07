using UnityEngine;
using DG.Tweening;

namespace TheAlterscope2
{
    public enum StagePhase
    {
        In,
        Out
    }

    public class TweenStageObjects : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        ConversationStateMachine _conversationStateMachine;
        Vector3 _boothIn, _boothOut;
        GameObject _booth, _avatarOne, _avatarTwo, _leftOfCenter, _rightOfCenter;
        float _duration;
        StageEvents _stageEvents;
        EmbodimentDetector _embodimentDetector;
        public StagePhase _currentStagePhase;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _avatarOne = myDependencies.AvatarOne;
            _avatarTwo = myDependencies.AvatarTwo;
            _booth = myDependencies.BoothContainer;
            _stageEvents = myDependencies.MyStageEvents;
            _leftOfCenter = myDependencies.LeftOfCenter;
            _rightOfCenter = myDependencies.RightOfCenter;
            _leftOfCenter.transform.position = new Vector3(_leftOfCenter.transform.position.x, _avatarOne.transform.position.y, _leftOfCenter.transform.position.z);
            _rightOfCenter.transform.position = new Vector3(_rightOfCenter.transform.position.x, _avatarOne.transform.position.y, _rightOfCenter.transform.position.z);
            _embodimentDetector = myDependencies.MyEmbodimentDetector;
            _duration = myDependencies.TweenDuration;
            _currentStagePhase = StagePhase.In;
            _hasBeenSetup = true;
        }

        public void InitializeBoothPosition()
        {
            _logger.Log("Initializing positions");
            _boothIn = _booth.transform.position;
            var boothOutTemp = (_avatarOne.transform.position - _booth.transform.position) + _avatarTwo.transform.position;
            _boothOut = new Vector3(boothOutTemp.x, 0f, boothOutTemp.z);
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.AnimateSlideIn || newState == ConversationState.AnimateSlideOut)
            {
                // _logger.Log("tween - tweening in!");
                if (_embodimentDetector.MostRecentlyEmbodiedAvatar == _avatarOne)
                {
                    if (_currentStagePhase == StagePhase.Out)
                        TweenIn();
                    else
                        TweenOut();
                }
                else if (_embodimentDetector.MostRecentlyEmbodiedAvatar == _avatarTwo)
                {
                    if (_currentStagePhase == StagePhase.Out)
                        TweenIn();
                    else
                        TweenOut();
                }
            }
            // if (newState == ConversationState.AnimateSlideOut)
            // {
            //     // _logger.Log("tween - tweening out!");
            //     TweenOut();
            // }
        }

        void TweenOut()
        {
            Vector3[] avatarOnePath = { _leftOfCenter.transform.position, _avatarTwo.transform.position };
            Vector3[] avatarTwoPath = { _rightOfCenter.transform.position, _avatarOne.transform.position };

            _avatarOne.transform.DOPath(avatarOnePath, _duration, PathType.CatmullRom);
            _avatarTwo.transform.DOPath(avatarTwoPath, _duration, PathType.CatmullRom);

            _booth.transform.DOMoveX(_boothOut.x, _duration);
            _booth.transform.DOMoveZ(_boothOut.z, _duration);
            _booth.transform.DOLocalRotate(new Vector3(0f, -90f, 0f), _duration / 2f).OnComplete(() => _booth.transform.DOLocalRotate(new Vector3(0f, -180f, 0f), _duration / 2f));

            _avatarOne.transform.DOLocalRotate(new Vector3(0f, -90f, 0f), _duration);
            _avatarTwo.transform.DOLocalRotate(new Vector3(0f, -90f, 0f), _duration).OnComplete(_stageEvents.RevolveDone);

            _currentStagePhase = StagePhase.Out;
        }

        void TweenIn()
        {
            Vector3[] avatarOnePath = { _rightOfCenter.transform.position, _avatarTwo.transform.position };
            Vector3[] avatarTwoPath = { _leftOfCenter.transform.position, _avatarOne.transform.position };

            _avatarOne.transform.DOPath(avatarOnePath, _duration, PathType.CatmullRom);
            _avatarTwo.transform.DOPath(avatarTwoPath, _duration, PathType.CatmullRom);

            _booth.transform.DOMoveX(_boothIn.x, _duration);
            _booth.transform.DOMoveZ(_boothIn.z, _duration);

            _booth.transform.DOLocalRotate(new Vector3(0f, -270f, 0f), _duration / 2f).OnComplete(() => _booth.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), _duration / 2f));

            _avatarOne.transform.DOLocalRotate(new Vector3(0f, 90f, 0f), _duration);
            _avatarTwo.transform.DOLocalRotate(new Vector3(0f, 90f, 0f), _duration).OnComplete(_stageEvents.RevolveDone);

            _currentStagePhase = StagePhase.In;
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
            _currentStagePhase = StagePhase.In;
        }
    }
}