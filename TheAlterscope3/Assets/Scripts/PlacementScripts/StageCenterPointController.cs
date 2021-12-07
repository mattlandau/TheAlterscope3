using UnityEngine;

namespace TheAlterscope2
{
    public class StageCenterPointController : MonoBehaviour, IHasSetup
    {
        Vector3 _centerOfStage;
        GameObject _stage;
        ILogger _logger;
        GameObject _avatarOne;
        GameObject _avatarTwo;
        GameObject _camera;
        Vector3 _avatarOneInitialPosition;
        Vector3 _avatarTwoInitialPosition;
        Vector3 _stageInitialPosition;
        Vector3 _cameraInitialPosition;
        Quaternion _avatarOneInitialRotation;
        Quaternion _avatarTwoInitialRotation;
        Quaternion _stageInitialRotation;
        Quaternion _cameraInitialRotation;
        TweenStageObjects _tweenStageObjects;
        float _minimumSpacing;
        GameObject _tapToPlace;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _stage = myDependencies.MyStage;
            _avatarOne = myDependencies.DeskAvatarOneOrbital;
            _avatarTwo = myDependencies.AvatarTwoOrbital;
            _camera = myDependencies.MyCamera;
            _tweenStageObjects = myDependencies.MyTweenStageObjects;
            _avatarOneInitialPosition = _avatarOne.transform.position;
            _avatarTwoInitialPosition = _avatarTwo.transform.position;
            _stageInitialPosition = _stage.transform.position;
            _avatarOneInitialRotation = _avatarOne.transform.rotation;
            _avatarTwoInitialRotation = _avatarTwo.transform.rotation;
            _stageInitialRotation = _stage.transform.rotation;
            _minimumSpacing = myDependencies.MinimumDistanceBetweenAvatars;
            _tapToPlace = myDependencies.PlacementIndicator;
        }

        public void AdjustAvatarLocationsAndStagePivot()
        {
            MoveAvatarTwoToCameraPosition();
            AdjustPositionsIfOverlapping();
            RePivotTheStage();
        }

        // public void ResetAvatarLocationsAndStagePivot()
        // {
        //     _stage.transform.position = _stageInitialPosition;
        //     _stage.transform.rotation = _stageInitialRotation;
        //     _avatarOne.transform.position = _avatarOneInitialPosition;
        //     _avatarOne.transform.rotation = _avatarOneInitialRotation;
        //     _avatarTwo.transform.position = _avatarTwoInitialPosition;
        //     _avatarTwo.transform.rotation = _avatarTwoInitialRotation;
        // }

        void MoveAvatarTwoToCameraPosition()
        {
            _centerOfStage = _avatarOne.transform.position - _avatarTwo.transform.position;
#if UNITY_EDITOR
            MoveCameraForPlayTestingInEditor();
#endif
            _avatarTwo.transform.position = new Vector3(_camera.transform.position.x, _avatarTwo.transform.position.y, _camera.transform.position.z);

        }

        void MoveCameraForPlayTestingInEditor()
        {
            // _camera.transform.position = new Vector3(0f, 1.15f, -2.4f);
            _camera.transform.position = new Vector3(0f, 1.15f, 0f);
        }

        void AdjustPositionsIfOverlapping()
        {
            _logger.Log("position difference is:" + (_avatarOne.transform.position - _avatarTwo.transform.position));
            _logger.Log("postion difference: rotation of indicator " + _tapToPlace.transform.rotation.eulerAngles.y);

            var zDiff = _avatarOne.transform.position.z - _avatarTwo.transform.position.z;
            var xDiff = _avatarOne.transform.position.x - _avatarTwo.transform.position.x;

            var position2DDifference = Mathf.Sqrt((zDiff * zDiff) + (xDiff * xDiff));
            _logger.Log("position difference magnitude is: " + position2DDifference);
            // var position2DDifference = zDiff;

            if (position2DDifference < _minimumSpacing)
            {
                // var currentPosition = _avatarOne.transform.position;
                // currentPosition.z += (_minimumSpacing - position2DDifference) * Mathf.Cos(_tapToPlace.transform.rotation.eulerAngles.y);
                // currentPosition.x += (_minimumSpacing - position2DDifference) * Mathf.Sin(_tapToPlace.transform.rotation.eulerAngles.y);

                // _avatarOne.transform.position = currentPosition;

                _avatarOne.transform.Translate(new Vector3(0f, 0f, _minimumSpacing - position2DDifference), Space.Self);
                _logger.Log("position difference adjusted");
            }
            _logger.Log("position difference is:" + (_avatarOne.transform.position - _avatarTwo.transform.position));
        }

        void RePivotTheStage()
        {
            var oldCenterOfStage = _centerOfStage;
            _centerOfStage = _avatarOne.transform.position - _avatarTwo.transform.position;
            var vectorToNewPivot = _centerOfStage - oldCenterOfStage;

            // Move stage to back to new center point
            _stage.transform.position = _stage.transform.position - vectorToNewPivot / 2f;

            // Move avatar two and booth forward to tapped location
            _avatarOne.transform.position = _avatarOne.transform.position + vectorToNewPivot / 2f;
            // _booth.transform.position = _booth.transform.position + vectorToNewPivot / 2f;

            // Move avatar one foward to camera location
            _avatarTwo.transform.position = _avatarTwo.transform.position + vectorToNewPivot / 2f;

            _logger.Log("Move " + ((_centerOfStage - oldCenterOfStage)).ToString());
        }
    }
}