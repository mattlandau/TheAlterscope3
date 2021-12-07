using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHapticFeedback
{
    void Trigger(iOSHapticFeedback.iOSFeedbackType myFeedbackType);
    bool IsSupported();
}

