using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheAlterscope2
{
    public class UIVisualModifier : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        bool _hasBeenSetup;
        GameObject _bubbleMicrophone;
        GameObject _VUPanel;
        VUMeter _MyVUMeter;
        GameObject[] _VUBars;
        Button _mySpeechBubble;
        Button _playButton;
        Button _stopButton;
        RectTransform _safeArea;
        ScreenOrientation _orientation;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _bubbleMicrophone = myDependencies.BubbleMicrophone;
            _VUBars = myDependencies.VUBars;
            _mySpeechBubble = myDependencies.MySpeechBubble;
            _playButton = myDependencies.PlayButton;
            _stopButton = myDependencies.StopButton;
            _safeArea = myDependencies.SafeArea;
            _orientation = Screen.orientation;
            ScaleToSafeArea();
            _hasBeenSetup = true;
        }

        public void HideUI()
        {
            HideImage(_mySpeechBubble.GetComponent<Image>());
            HideImage(_bubbleMicrophone.GetComponent<Image>());
            for (var i = 0; i < _VUBars.Length; ++i)
                HideImage(_VUBars[i].GetComponent<Image>());

            HideImage(_playButton.GetComponent<Image>());
            HideImage(_stopButton.GetComponent<Image>());
        }

        void HideImage(Image myImage)
        {
            var myColor = myImage.color;
            myColor.a = 0f;
            myImage.color = myColor;
        }

        void ScaleToSafeArea()
        {
            var tempSafeArea = Screen.safeArea;
            var tempMinAnchor = tempSafeArea.position;
            var tempMaxAnchor = tempMinAnchor + tempSafeArea.size;

            tempMinAnchor.x /= Screen.width;
            tempMinAnchor.y /= Screen.height;
            tempMaxAnchor.x /= Screen.width;
            tempMaxAnchor.y /= Screen.height;

            _safeArea.anchorMin = tempMinAnchor;
            _safeArea.anchorMax = tempMaxAnchor;
        }

        void Update()
        {
            if (!_hasBeenSetup)
                return;

            if (Screen.orientation != _orientation)
            {
                ScaleToSafeArea();
                _orientation = Screen.orientation;
            }
        }
    }
}