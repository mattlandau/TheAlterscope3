using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace TheAlterscope2
{
    public class PlaceAvatarsOnFloor : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        FloorDetector _floorDetector;
        GameObject _placementIndicator;
        GameObject _camera;
        Vector3 _cameraAngle;
        Ray _ray;
        RaycastHit[] _hits;
        bool _hasBeenSetup;
        List<RaycastHit> _floorHits;
        GameObject _leftBound;
        GameObject _rightBound;
        public bool AreAvatarsPlaced;
        public GameObject Avatars;
        DialogueMediator _dialogueMediator;
        ConversationStateMachine _conversatationStateMachine;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _floorDetector = myDependencies.MyFloorDetector;
            _placementIndicator = myDependencies.PlacementIndicator;
            _camera = myDependencies.MyCamera;
            _leftBound = myDependencies.LeftIndicatorBound;
            _rightBound = myDependencies.RightIndicatorBound;
            _dialogueMediator = myDependencies.MyDialogueMediator;
            _conversatationStateMachine = myDependencies.MyConversationStateMachine;
            EnhancedTouchSupport.Enable();
            _logger.Log("PlaceAvatarsOnFloor setup has run");
            _hasBeenSetup = true;
        }

        void Update()
        {
            if (!_hasBeenSetup)
                return;

#if !UNITY_EDITOR
            if (_floorDetector.SuitableFloorList.Count == 0)
                return;

            _floorHits = RaycastToFloor();
            if (_floorHits.Count == 0 && _placementIndicator.activeSelf == false)
                return;

            PlaceIndicatorOnFloor();
            
            if (IsIndicatorWithinFloor())
                _floorDetector.DisableCoaching();

            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0 && UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (_placementIndicator.activeSelf)
                    PlaceAvatars();
                else
                    _logger.Log("tap heard, but placement indicator not active");
            }
#endif
#if UNITY_EDITOR
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                PlaceAvatars();
#endif
        }

        List<RaycastHit> RaycastToFloor()
        {
            List<RaycastHit> floorHits = new List<RaycastHit>();
            var myFloors = _floorDetector.SuitableFloorList;
            _cameraAngle = _camera.transform.forward;
            _ray.origin = _camera.transform.position;
            _ray.direction = _cameraAngle;
            _hits = Physics.RaycastAll(_ray);
            foreach (var hit in _hits)
                foreach (var floor in myFloors)
                    if (GetHitID(hit.transform.gameObject.name) == floor.trackableId.ToString().Trim())
                        floorHits.Add(hit);
            return floorHits;
        }

        void PlaceIndicatorOnFloor()
        {
            if (_floorHits.Count == 0)
                return;

            if (IsIndicatorWithinFloor() && _placementIndicator.activeSelf == false)
                _placementIndicator.SetActive(true);

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            var previousPosition = _placementIndicator.transform.position;
            var previousRotation = _placementIndicator.transform.rotation;
            _placementIndicator.transform.rotation = Quaternion.LookRotation(cameraBearing);
            _placementIndicator.transform.position = _floorHits[0].point;

            if (!IsIndicatorWithinFloor())
            {
                _placementIndicator.transform.position = previousPosition;
                _placementIndicator.transform.rotation = previousRotation;
            }
        }

        string GetHitID(string myName)
        {
            var tokens = myName.Split(' ');
            return tokens[tokens.Length - 1].Trim();
        }

        public bool IsIndicatorWithinFloor()
        {
            bool isLeftOnFloor, isRightOnFloor;

            var leftVector = _leftBound.transform.position - _camera.transform.position;
            var leftRay = new Ray();
            leftRay.origin = _camera.transform.position;
            leftRay.direction = leftVector;
            var leftHits = Physics.RaycastAll(leftRay);
            isLeftOnFloor = CheckForFloorInHits(leftHits);

            var rightVector = _rightBound.transform.position - _camera.transform.position;
            var rightRay = new Ray();
            rightRay.origin = _camera.transform.position;
            rightRay.direction = rightVector;
            var rightHits = Physics.RaycastAll(rightRay);
            isRightOnFloor = CheckForFloorInHits(rightHits);

            return isLeftOnFloor && isRightOnFloor;
        }

        bool CheckForFloorInHits(RaycastHit[] hits)
        {
            var myFloors = _floorDetector.SuitableFloorList;
            foreach (var hit in hits)
                foreach (var floor in myFloors)
                    if (GetHitID(hit.transform.gameObject.name) == floor.trackableId.ToString().Trim())
                        return true;
            return false;
        }

        void PlaceAvatars()
        {
            Avatars.transform.position = _placementIndicator.transform.position;
            Avatars.transform.rotation = _placementIndicator.transform.rotation;
            _dialogueMediator.AdjustAvatarLocationsAndStagePivot();
            _dialogueMediator.LowerPositions();
            Avatars.SetActive(true);
            AreAvatarsPlaced = true;
            _conversatationStateMachine.MoveNext(Command.BeginAvatarsPlaced);
        }

        public void UnplaceAvatars()
        {
            Avatars.SetActive(false);
            AreAvatarsPlaced = false;
            _floorDetector.Reset();
            _logger.Log("Avatars unplaced");
        }
    }
}