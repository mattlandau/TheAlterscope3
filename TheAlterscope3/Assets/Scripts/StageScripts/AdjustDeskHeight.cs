using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class AdjustDeskHeight : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        GameObject _base, _camera;
        float _minCutoff, _maxCutoff;
        bool _hasBeenSetup;
        GameObject[] _objectsToLower;
        Vector3[] _originalPositions;
        Quaternion[] _originalRotations;
        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _base = myDependencies.PlacementIndicator;
            _camera = myDependencies.MyCamera;
            _objectsToLower = myDependencies.ObjectsToLower;
            _minCutoff = myDependencies.MinHeightCutoff;
            _maxCutoff = myDependencies.MaxHeightCutoff;
            _originalPositions = new Vector3[_objectsToLower.Length];
            for (var i = 0; i < _objectsToLower.Length; ++i)
                _originalPositions[i] = _objectsToLower[i].transform.position;

            _originalRotations = new Quaternion[_objectsToLower.Length];
            for (var i = 0; i < _objectsToLower.Length; ++i)
                _originalRotations[i] = _objectsToLower[i].transform.rotation;

            _hasBeenSetup = true;
        }

        public void LowerDesk()
        {
            var cameraHeight = _camera.transform.position.y - _base.transform.position.y;
            if (cameraHeight > _maxCutoff)
                return;

            var amountToLower = Mathf.Min(_maxCutoff - cameraHeight, _maxCutoff - _minCutoff);

            _logger.Log($"camera height is {cameraHeight}, and amount to lower {amountToLower}");

            for (var i = 0; i < _objectsToLower.Length; ++i)
            {
                var currentPosition = _objectsToLower[i].transform.position;
                var newPosition = new Vector3(currentPosition.x, currentPosition.y - amountToLower, currentPosition.z);
                _objectsToLower[i].transform.position = newPosition;
                _logger.Log($"old y: {currentPosition.y}, new y: {_objectsToLower[i].transform.position.y}");
            }
        }
    }
}