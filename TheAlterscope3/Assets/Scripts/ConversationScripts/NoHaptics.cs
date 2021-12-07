using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class NoHaptics : MonoBehaviour, IHasSetup, IHapticFeedback
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
            _logger.Log("Using No Haptics");
        }

        public bool IsSupported() => true;
    }
}