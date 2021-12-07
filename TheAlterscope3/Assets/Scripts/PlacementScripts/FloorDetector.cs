#define AR_ENABLED
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARSubsystems;

namespace TheAlterscope2
{
    public class FloorDetector : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        ARPlaneManager _planeManager;
        ARKitCoachingOverlay _coachingOverlay;
        float _minimumPlaneLength;
        bool _floorDetected, _hasBeenSetup;
        ARPlane _floor;
        PlaceAvatarsOnFloor _placeAvatarsOnFloor;
        HashSet<string> SuitableFloorNames;
        public List<ARPlane> SuitableFloorList;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
#if AR_ENABLED
            _planeManager = myDependencies.MyARPlaneManager;
            _coachingOverlay = myDependencies.MyCoachingOverlay;
#endif
            _minimumPlaneLength = myDependencies.MinimumPlaneLength;
            _placeAvatarsOnFloor = myDependencies.MyPlaceAvatarsOnFloor;
#if !UNITY_EDITOR
            _logger.Log("Activating coaching from within FloorDetector setup");
            _coachingOverlay.ActivateCoaching(true);
#endif
            SuitableFloorNames = new HashSet<string>();
            SuitableFloorList = new List<ARPlane>();

            _logger.Log("FloorDetector setup ran");
#if AR_ENABLED
            _planeManager.planesChanged += OnPlanesChanged;
            // _logger.Log("arbitrary plane" + _planeManager.subsystem.SubsystemDescriptor.supportsArbitraryPlaneDetection.ToString());
            // _logger.Log("boundary vertices" + _planeManager.subsystem.SubsystemDescriptor.supportsBoundaryVertices.ToString());
            // _logger.Log("plane classification" + _planeManager.subsystem.SubsystemDescriptor.supportsClassification.ToString());
            // _logger.Log("horizontal detection" + _planeManager.subsystem.SubsystemDescriptor.supportsHorizontalPlaneDetection.ToString());
#endif
            _hasBeenSetup = true;
        }

        bool IsSuitableFloor(ARPlane plane)
        {
            // if (plane.classification != PlaneClassification.Floor && plane.classification != PlaneClassification.None)
            //     return false;

            // if (plane.extents.y < _minimumPlaneLength && plane.extents.x < _minimumPlaneLength)
            //     return false;

            return true;
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
        {
            _logger.Log("OnPlanesChanged called");
            for (var i = 0; i < eventArgs.added.Count; ++i)
            {
                if (IsSuitableFloor(eventArgs.added[i]))
                {
                    if (!SuitableFloorNames.Contains(eventArgs.added[i].name))
                    {
                        _logger.Log("Added a suitable floor: " + eventArgs.added[i].name);
                        SuitableFloorNames.Add(eventArgs.added[i].name);
                        SuitableFloorList.Add(eventArgs.added[i]);
                    }
                }
            }

            for (var i = 0; i < eventArgs.updated.Count; ++i)
            {
                if (!SuitableFloorNames.Contains(eventArgs.updated[i].name))
                {
                    if (IsSuitableFloor(eventArgs.updated[i]))
                    {
                        _logger.Log("Updated a suitable floor: " + eventArgs.updated[i].name);
                        SuitableFloorNames.Add(eventArgs.updated[i].name);
                        SuitableFloorList.Add(eventArgs.updated[i]);
                    }
                }
            }
        }

        public void DisableCoaching()
        {
            if (_coachingOverlay.enabled == true)
                _coachingOverlay.DisableCoaching(true);
        }

        public void Reset()
        {
            foreach (var plane in _planeManager.trackables)
                plane.gameObject.SetActive(false);

            _floor = null;
            _floorDetected = false;
#if !UNITY_EDITOR
            _coachingOverlay.enabled = true;
            _coachingOverlay.ActivateCoaching(true);
#endif
            _logger.Log("FloorDetector Reset has run");
        }
    }
}