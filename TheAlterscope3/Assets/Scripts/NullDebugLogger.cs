using UnityEngine;

namespace TheAlterscope2
{
    public class NullDebugLogger : ILogger
    {
        public void Log(string debugMessage)
        {
            return;
        }
    }
}
