#define AR_ENABLED 
using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation.Samples;

namespace TheAlterscope2
{

    public class LaunchFactory : MonoBehaviour
    {
        public Dependencies MyDependencies;
        ARKitCoachingOverlay _coaching;
        GameObject _placementScripts;
        DevicePermissions _devicePermissions;
        string _isInitialLaunchString;
        public bool IsInitialLaunch;

        void Awake()
        {
#if AR_ENABLED
            MyDependencies.MyARSession.Reset();
#endif
            MyDependencies.MyDialogueMediator.Setup(MyDependencies);
            MyDependencies.MyBrightnessDetector.Setup(MyDependencies);

            MyDependencies.MyFloorDetector.Setup(MyDependencies);
            MyDependencies.MyPlaceAvatarsOnFloor.Setup(MyDependencies);

            MyDependencies.MyResetTransforms.Setup(MyDependencies);
            MyDependencies.MyStageCenterPointController.Setup(MyDependencies);
            MyDependencies.MyAdjustDeskHeight.Setup(MyDependencies);
            MyDependencies.MyTweenStageObjects.Setup(MyDependencies);

            MyDependencies.MyDelayScript.Setup(MyDependencies);
            MyDependencies.MyApplicationBehaviors.Setup(MyDependencies);
            MyDependencies.MyConversationStateMachine.Setup(MyDependencies);
            MyDependencies.MyChangeMode.Setup(MyDependencies);
            MyDependencies.MyResumeFromPause.Setup(MyDependencies);
            MyDependencies.MyDevicePermissions.Setup(MyDependencies);
            MyDependencies.MyUIVisualModifier.Setup(MyDependencies);
            MyDependencies.MyUIStateModifier.Setup(MyDependencies);
            MyDependencies.MyUserPreferences.Setup(MyDependencies);

            _isInitialLaunchString = PlayerPrefs.GetString("IsInitialLaunch", "");
            if (_isInitialLaunchString == "")
                PlayerPrefs.SetString("IsInitialLaunch", "true");
            else
                PlayerPrefs.SetString("IsInitialLaunch", "false");

            _isInitialLaunchString = PlayerPrefs.GetString("IsInitialLaunch", "");
            IsInitialLaunch = _isInitialLaunchString == "true" ? true : false;

            Debug.Log("MATTSLOG: initial launch string: " + _isInitialLaunchString + " and bool: " + IsInitialLaunch);
            _devicePermissions = MyDependencies.MyDevicePermissions;
            _devicePermissions.CheckPermissions();
            Debug.Log("MATTSLOG: checking permissions");
        }
    }
}
