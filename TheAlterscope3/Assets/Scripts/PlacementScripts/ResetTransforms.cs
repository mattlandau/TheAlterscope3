using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class ResetTransforms : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        Vector3[] _originalPositions;
        Quaternion[] _originalRotations;
        GameObject[] _objects;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _objects = myDependencies.RestorableTransformObjects;
            _originalPositions = new Vector3[_objects.Length];
            _originalRotations = new Quaternion[_objects.Length];
            SaveTransforms();
            _hasBeenSetup = true;
        }

        void SaveTransforms()
        {
            _logger.Log("Saving transforms");
            for (var i = 0; i < _objects.Length; ++i)
                _originalRotations[i] = _objects[i].transform.rotation;

            for (var i = 0; i < _objects.Length; ++i)
                _originalPositions[i] = _objects[i].transform.position;
        }

        public void LoadSavedTransforms()
        {
            _logger.Log("Loading saved transforms");
            for (var i = 0; i < _objects.Length; ++i)
                _objects[i].transform.rotation = _originalRotations[i];

            for (var i = 0; i < _objects.Length; ++i)
                _objects[i].transform.position = _originalPositions[i];
        }
    }
}