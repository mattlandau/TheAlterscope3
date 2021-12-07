#define AR_ENABLED
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.UI;
// UNITY_XR_ARKIT_LOADER_ENABLED
namespace TheAlterscope2
{
    public enum VoicePitch
    {
        Low,
        High
    }

    public class Dependencies : MonoBehaviour
    {
        public ILogger MyLogger;
        [Header("Development -- Meta")]
        public Template MyTemplate;
        public InputField MyLog;
        [Header("AR Foundation Scripts")]
#if AR_ENABLED
        public ARPlaneManager MyARPlaneManager;
        public ARSessionOrigin MyARSessionOrigin;
        public ARRaycastManager MyARRaycastManager;
        public AROcclusionManager MyAROcclusionManager;
        public ARSession MyARSession;
        public ARCameraManager MyARCameraManager;
#endif
        [Header("Launch")]
        public InitialGameObjectActivations MyInitialGameObjectActivations;
        public ApplicationBehaviors MyApplicationBehaviors;
        public LaunchFactory MyLaunchFactory;
        public DevicePermissions MyDevicePermissions;
        public GameObject NeedMicrophoneAccess;
        public GameObject NeedCameraAccess;
        public GameObject NeedMicrophoneAndCameraAccess;
        public GameObject NeedMicrophoneAndQuit;
        public UserPreferences MyUserPreferences;
        [Header("Conversation Scripts")]
        public AudioRecordingList MyAudioRecordingList;
        public PlaybackAudio MyPlaybackAudio;
        public SpeechBubbleTap MySpeechBubbleTapListener;
        public DialogueMediator MyDialogueMediator;
        public ConstantPitchTimeScale MyTimeScaleConstantPitch;
        public PlaceAvatarsOnFloor MyPlaceAvatarsOnFloor;
        public ChangeMode MyChangeMode;
        public EmbodimentDetector MyEmbodimentDetector;
        public PlaybackControls MyPlaybackControls;
        public FadeAudioSource MyFadeAudioSource;
        [Header("UI: Playback Panel")]
        public PlaybackPanel MyPlaybackPanel;
        public Animator PlaybackPanelSlider;
        public Button PlayButton;
        public Button StopButton;
        public Animator StopButtonExpandAnimator;
        public Animator PlayButtonExpandAnimator;
        [Header("Instructions")]
        public Animator Instructions;
        [Header("UI: Speech Bubbles")]
        public Sprite CanRecordBubble;
        public Sprite IsRecordingBubble;
        public Button MySpeechBubble;
        public SpeechBubbleModifier MySpeechBubbleModifier;
        public Animator SpeechBubbleSlider;
        public Animator ExpandBubble;
        public GameObject SpeechButtonLeftCancelBoundary;
        public GameObject SpeechButtonRightCancelBoundary;
        public GameObject SpeechButtonTopCancelBoundary;
        [Header("UI: Canvases")]
        public GameObject MyScreenSpaceConversationControlsCanvas;
        public GameObject SpeechButtonPanel;
        public GameObject PlaybackButtonsPanel;
        public UIVisualModifier MyUIVisualModifier;
        public UIStateModifier MyUIStateModifier;
        public Animator UIStateAnimator;
        public RectTransform SafeArea;
        [Header("UI: VU Meter (i.e. Audio Visualizer")]
        public GameObject BubbleMicrophone;
        public GameObject VUPanel;
        public VUMeter MyVUMeter;
        public GameObject[] VUBars;
        public int FramesPerVU;
        public float MicrophoneMinimumAmplitude;
        public float VuMaxScale;
        public float VuMinScale;
        public float VuMultiplier;
        public AudioSource AudioSourceForInitialRecording;
        [Header("Placement")]
        public float MinimumDistanceBetweenAvatars;
#if AR_ENABLED
        public ARKitCoachingOverlay MyCoachingOverlay;
#endif
        public Material PlacementPlaneMaterial;
        public GameObject PlacementIndicator;
        public GameObject FindFloorMessage;
        public BrightnessDetector MyBrightnessDetector;
        public GameObject BrightnessMessage;
        public float BrightnessIndicatorLag;
        public float BrightnessThreshold;
        public GameObject LeftIndicatorBound;
        public GameObject RightIndicatorBound;
        public float MinimumPlaneLength;
        public FloorDetector MyFloorDetector;
        public StageCenterPointController MyStageCenterPointController;
        [Header("Save and Load Transforms")]
        public ResetTransforms MyResetTransforms;
        public GameObject[] RestorableTransformObjects;
        [Header("Desk Height")]
        public AdjustDeskHeight MyAdjustDeskHeight;
        [SerializeField]
        public GameObject[] ObjectsToLower;
        public float MaxHeightCutoff;
        public float MinHeightCutoff;
        [Header("Script Container Objects")]
        public GameObject MyPlacementScriptsObject;
        public GameObject MyConversationScriptsObject;
        public GameObject MyTransitionScriptsObject;
        [Header("Avatars (both)")]
        public GameObject Avatars;
        public AnimateAvatars MyAvatarAnimators;
        public Animator AvatarOneAnimator;
        public Animator AvatarTwoAnimator;
        public float MinFadeDistance;
        public float MaxFadeDistance;
        [Header("Avatar One")]
        public GameObject AvatarOne;
        public AudioSource AvatarOneAudioSource;
        public AvatarMaterialSwitcher MyAvatarOneMaterialSwitcher;
        public GameObject AvatarOneStoolBlob;
        public GameObject AvatarOneAvatarShadowBlob;
        public SkinnedMeshRenderer AvatarOneRenderer;
        public Renderer StoolOneRenderer;
        public AvatarEvents AvatarOneEventScript;
        public Animator AvatarOneListen;
        public Material AvatarOneMaterial;
        [Header("Avatar Two")]
        public GameObject AvatarTwo;
        public GameObject MyPersonObject;
        public AudioSource AvatarTwoAudioSource;
        public AvatarMaterialSwitcher MyAvatarTwoMaterialSwitcher;
        public GameObject AvatarTwoShadowBlob;
        public Animator AvatarTwoListen;
        public AvatarEvents AvatarTwoEventScript;
        public Material AvatarTwoMaterial;
        [Header("Other Objects")]
        public AudioListener MyAudioListener;
        public AudioRecorder MyAudioRecorder;
        public GameObject MyCamera;
        public GameObject MyEventSystemObject;
        public GameObject MyPlacementIndicator;
        [Header("Haptics")]
        public iOSHapticFeedback MyHapticFeedback;
        public HapticFallback MyHapticFallback;
        public NoHaptics MyNoHaptics;
        [Header("Audio Parameters")]
        public int MySampleRate;
        public float DelayBetweenTurns;
        public float AudioPlaybackStartDelay;
        public float MaleVoicePitch;
        public float FemaleVoicePitch;
        public int BaseClipDuration;
        public int ClipOverflowBufferDuration;
        public float FadeInDuration;
        public float FadeOutDuration;
        [Header("Lighting")]
        public LightAdjuster MyLightAdjuster;
        public Light MainLight;
        public float LightChangeability;
        [Header("Stage Rotation")]
        public GameObject MyStage;
        public StageManager MyStageManager;
        public StageEvents MyStageEvents;
        public AudioSource BoothSlideAudioSource;
        public AudioClip MySlideOutAudio;
        public AudioClip MySlideInAudio;
        public GameObject BoothContainer;
        public ParticleSystem BoothSlideDust;
        public ParticleSystem AvatarOneSlideDust;
        public ParticleSystem AvatarTwoSlideDust;
        [Header("Tween Stage")]
        public TweenStageObjects MyTweenStageObjects;
        public float TweenDuration;
        public GameObject DeskAvatarOneOrbital;
        public GameObject AvatarTwoOrbital;
        public GameObject LeftOfCenter;
        public GameObject RightOfCenter;
        [Header("State Controls")]
        public Delay MyDelayScript;
        public float DelayBeforePlaybackDuration;
        public float DelayAfterPlaybackDuration;
        public float DelayAfterRecordingDuration;
        public ConversationStateMachine MyConversationStateMachine;
        public ResumeFromPause MyResumeFromPause;
    }
}