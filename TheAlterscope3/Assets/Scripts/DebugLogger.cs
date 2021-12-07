// #define DEBUG_IPAD
using UnityEngine;
using UnityEngine.UI;

namespace TheAlterscope2
{
    public class DebugLogger : ILogger
    {
#if DEBUG_IPAD
        InputField _debugLog;
#endif
        public void Log(string debugMessage)
        {
            Debug.Log("MATTSLOG: " + Time.time + " " + debugMessage);
#if DEBUG_IPAD
            _debugLog = GameObject.Find("DebugField").GetComponent<InputField>();
            _debugLog.text += Time.time + " " + debugMessage + "\n";
#endif
        }
    }
}
