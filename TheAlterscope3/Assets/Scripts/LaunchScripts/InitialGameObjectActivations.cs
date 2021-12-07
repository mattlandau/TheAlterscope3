using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class InitialGameObjectActivations : MonoBehaviour
    {
        public Dependencies MyDependencies;

        void Awake()
        {
            MyDependencies.MyScreenSpaceConversationControlsCanvas.SetActive(false);
            MyDependencies.MyPlacementScriptsObject.SetActive(false);
            MyDependencies.MyStage.SetActive(false);
            MyDependencies.MyPersonObject.SetActive(false);
        }

        public void PlacingModeActivations()
        {
            MyDependencies.MyScreenSpaceConversationControlsCanvas.SetActive(false);
            MyDependencies.MyPlacementScriptsObject.SetActive(false);
            MyDependencies.MyStage.SetActive(false);
            MyDependencies.MyPersonObject.SetActive(false);
        }
    }
}