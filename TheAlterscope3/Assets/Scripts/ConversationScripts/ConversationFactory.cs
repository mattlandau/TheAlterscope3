using UnityEngine;

namespace TheAlterscope2
{
    public class ConversationFactory : MonoBehaviour
    {
        public Dependencies MyDependencies;
        void Awake()
        {
            MyDependencies.MyAudioRecorder.Setup(MyDependencies);
            MyDependencies.MyAudioRecordingList.Setup(MyDependencies);
            MyDependencies.MyPlaybackAudio.Setup(MyDependencies);
            MyDependencies.MyPlaybackControls.Setup(MyDependencies);
            MyDependencies.MyEmbodimentDetector.Setup(MyDependencies);
            MyDependencies.MySpeechBubbleTapListener.Setup(MyDependencies);
            MyDependencies.MyAvatarOneMaterialSwitcher.Setup(MyDependencies);
            MyDependencies.MyAvatarTwoMaterialSwitcher.Setup(MyDependencies);
            MyDependencies.AvatarOneEventScript.Setup(MyDependencies);
            MyDependencies.AvatarTwoEventScript.Setup(MyDependencies);
            MyDependencies.MyAvatarAnimators.Setup(MyDependencies);
            MyDependencies.MySpeechBubbleModifier.Setup(MyDependencies);
            MyDependencies.MyStageEvents.Setup(MyDependencies);
            MyDependencies.MyStageManager.Setup(MyDependencies);
            MyDependencies.MyVUMeter.Setup(MyDependencies);
            MyDependencies.MyPlaybackPanel.Setup(MyDependencies);
            MyDependencies.MyLightAdjuster.Setup(MyDependencies);
            MyDependencies.MyFadeAudioSource.Setup(MyDependencies);
            MyDependencies.MyHapticFallback.Setup(MyDependencies);
            MyDependencies.MyNoHaptics.Setup(MyDependencies);
        }
    }
}