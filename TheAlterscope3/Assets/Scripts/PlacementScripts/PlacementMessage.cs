#if INCLUDE_OTHER_DEPRECATD
using UnityEngine;

namespace TheAlterscope2
{
    public enum Message
    {
        MakeBrighter,
        FindFloor,
        None
    }

    public class PlacementMessage : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        GameObject _findFloorMessage;
        GameObject _makeBrighterMessage;
        Message _previousMessage;
        Message _activeMessage;
        bool _hasBeenSetup;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _findFloorMessage = myDependencies.FindFloorMessage;
            _makeBrighterMessage = myDependencies.BrightnessMessage;
            ActivateMessage(Message.FindFloor);
            _logger.Log("MessageVisualizer setup ran");
            _hasBeenSetup = true;
        }

        public void ActivateMessage(Message myMessage)
        {
            _previousMessage = GetActiveMessage();
            _activeMessage = myMessage;
            if (myMessage == Message.FindFloor)
            {
                _findFloorMessage.SetActive(true);
                _makeBrighterMessage.SetActive(false);
            }
            else if (myMessage == Message.MakeBrighter)
            {
                _findFloorMessage.SetActive(false);
                _makeBrighterMessage.SetActive(true);
            }
            else if (myMessage == Message.None)
            {
                _findFloorMessage.SetActive(false);
                _makeBrighterMessage.SetActive(false);
            }

        }

        public Message GetActiveMessage() => _activeMessage;
        public Message GetPreviousMessage() => _previousMessage;
    }
}
#endif