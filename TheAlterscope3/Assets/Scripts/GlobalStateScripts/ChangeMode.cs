#define AR_ENABLED
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

namespace TheAlterscope2
{
    public enum Mode
    {
        Conversation,
        Placing,
        Quit
    }

    public class ChangeMode : MonoBehaviour, IStateResponder, IHasSetup
    {
        ILogger _logger;
        PlaceAvatarsOnFloor _placeAvatarsOnFloor;
        GameObject _conversationControls;
        AROcclusionManager _occlusionManager;
        DialogueMediator _dialogueMediator;
        Material _placementPlaneMaterial;
        GameObject _placementIndicator, _stage;
        ARPlaneManager _planeManager;
        ARSession _arSession;
        GameObject _conversationScripts, _placementScriptsGameObject, _brightnessMessage;
        ConversationStateMachine _conversationStateMachine;
        ARKitCoachingOverlay _coaching;
        DevicePermissions _devicePermissions;
        UIVisualModifier _uiVisualModifier;
        AvatarMaterialSwitcher _avatarOneMaterialSwitcher;
        StageEvents _stageEvents;
        TweenStageObjects _tweenStageObjects;
        ResetTransforms _resetTransforms;
        AdjustDeskHeight _deskHeightAdjuster;
        EmbodimentDetector _embodimentDetector;
        bool _hasBeenSetup;
        public Mode CurrentMode;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _placeAvatarsOnFloor = myDependencies.MyPlaceAvatarsOnFloor;
            _conversationControls = myDependencies.MyScreenSpaceConversationControlsCanvas;
#if AR_ENABLED
            _occlusionManager = myDependencies.MyAROcclusionManager;
            _planeManager = myDependencies.MyARPlaneManager;
            _arSession = myDependencies.MyARSession;
            _coaching = myDependencies.MyCoachingOverlay;
#endif
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _placementScriptsGameObject = myDependencies.MyPlacementScriptsObject;
            _placementIndicator = myDependencies.PlacementIndicator;
            _conversationScripts = myDependencies.MyConversationScriptsObject;
            _conversationStateMachine = myDependencies.MyConversationStateMachine;
            _conversationStateMachine.OnStateEntered += OnNewState;
            _devicePermissions = myDependencies.MyDevicePermissions;
            _uiVisualModifier = myDependencies.MyUIVisualModifier;
            _avatarOneMaterialSwitcher = myDependencies.MyAvatarOneMaterialSwitcher;
            _stageEvents = myDependencies.MyStageEvents;
            _resetTransforms = myDependencies.MyResetTransforms;
            _tweenStageObjects = myDependencies.MyTweenStageObjects;
            _deskHeightAdjuster = myDependencies.MyAdjustDeskHeight;
            _embodimentDetector = myDependencies.MyEmbodimentDetector;
            _brightnessMessage = myDependencies.BrightnessMessage;
            _stage = myDependencies.MyStage;
            _hasBeenSetup = true;
        }

        public void OnNewState(ConversationState newState)
        {
            if (newState == ConversationState.AvatarsPlaced)
                EnterConversationMode();
        }

        public void EnterConversationMode()
        {
            CurrentMode = Mode.Conversation;
            _logger.Log("mode: EnterConversationMode");
            _conversationScripts.SetActive(true);
            _conversationControls.SetActive(true);
            _placementScriptsGameObject.SetActive(false);
            _placementIndicator.SetActive(false);
            _embodimentDetector.RefreshEmbodiment();
            _avatarOneMaterialSwitcher.SetVisibility(true);
            _brightnessMessage.SetActive(false);
            foreach (var plane in _planeManager.trackables)
                plane.gameObject.SetActive(false);

            _planeManager.enabled = false;
            _stageEvents.StopDust();
            _tweenStageObjects.InitializeBoothPosition();
            _dialogueMediator.MoveToNextState(Command.BeginReadyToRecord);
        }

        public void ReEnterPlacingMode()
        {
            if (_conversationStateMachine.CurrentState == ConversationState.AnimateSlideOut)
                return;

            if (_conversationStateMachine.CurrentState == ConversationState.AnimateSlideIn)
                return;

            _logger.Log("mode: ReEnterPlacingMode");
            _placeAvatarsOnFloor.UnplaceAvatars();
            _resetTransforms.LoadSavedTransforms();
            EnterPlacingMode();
        }

        public void EnterQuitMode()
        {
            CurrentMode = Mode.Quit;
            _logger.Log("mode: EnterQuitMode");
            StopAllCoroutines();
            _uiVisualModifier.HideUI();
            _devicePermissions.ShowMicrophoneNeededAndQuit();
            _conversationScripts.SetActive(false);
            _conversationControls.SetActive(false);
            _placementIndicator.SetActive(false);
        }

        public void EnterPlacingMode()
        {
            CurrentMode = Mode.Placing;
            _logger.Log("mode: EnterPlacingMode");
            _uiVisualModifier.HideUI();
            _conversationScripts.SetActive(false);
            _conversationControls.SetActive(false);
            _placementIndicator.SetActive(false);
            _stage.SetActive(false);
            _placementScriptsGameObject.SetActive(true);
            _planeManager.enabled = true;

#if !UNITY_EDITOR
            _coaching.enabled = true;
#endif

            foreach (var plane in _planeManager.trackables)
                plane.gameObject.SetActive(true);

            _dialogueMediator.MoveToNextState(Command.BeginPlacing);
        }
    }
}