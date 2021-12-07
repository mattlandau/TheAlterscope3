using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public class VUMeter : MonoBehaviour
    {
        ILogger _logger;
        bool _hasBeenSetup;
        GameObject _vuPanel;
        float[] _vuValues;
        int _vuBarCount = 5;
        float _minimumAmplitude;
        GameObject[] _vuBars;
        float _vuMaxScale;
        float _vuMinScale;
        float _vuMultiplier;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _vuPanel = myDependencies.VUPanel;
            _vuBars = myDependencies.VUBars;
            _vuBarCount = _vuBars.Length;
            _vuValues = new float[_vuBarCount];
            _minimumAmplitude = myDependencies.MicrophoneMinimumAmplitude;
            _vuMaxScale = myDependencies.VuMaxScale;
            _vuMinScale = myDependencies.VuMinScale;
            _vuMultiplier = myDependencies.VuMultiplier;
            ClearMeterValues();
            _hasBeenSetup = true;
        }

        public void ReceiveData(float dataPoint)
        {
            if (dataPoint < _minimumAmplitude)
                dataPoint = 0f;

            dataPoint -= _minimumAmplitude; // normalize range to start at 0
            dataPoint *= _vuMultiplier;
            dataPoint = Mathf.Clamp(dataPoint, _vuMinScale, _vuMaxScale);
            for (var i = 0; i < _vuBarCount - 1; ++i)
                _vuValues[i] = _vuValues[i + 1];

            _vuValues[_vuBarCount - 1] = dataPoint;

            RenderVUValues();
        }

        void RenderVUValues()
        {
            for (var i = 0; i < _vuBarCount; ++i)
                _vuBars[i].transform.localScale = new Vector3(_vuValues[i], _vuValues[i], 1f);
        }

        public void ClearMeterValues()
        {
            for (var i = 0; i < _vuValues.Length; ++i)
                _vuValues[i] = _vuMinScale;

            RenderVUValues();
        }
    }
}