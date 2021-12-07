using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class HapticFallback : MonoBehaviour, IHasSetup, IHapticFeedback
    {
        ILogger _logger;
        bool _hasBeenSetup;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _hasBeenSetup = true;
        }

        public void Trigger(iOSHapticFeedback.iOSFeedbackType myFeedbackType)
        {
            _logger.Log("using Haptic Fallback");
            if (myFeedbackType == iOSHapticFeedback.iOSFeedbackType.Success)
                Handheld.Vibrate();
        }

        public bool IsSupported() => true;
    }
}