// #define INCLUDE_LOGS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public static class Factory
    {
        public static ILogger CreateLogger(Dependencies myDependencies)
        {
            if (myDependencies.MyLogger != null)
                return myDependencies.MyLogger;

#if INCLUDE_LOGS
            myDependencies.MyLogger = new DebugLogger();
#else
            myDependencies.MyLogger = new NullDebugLogger();
#endif
            return myDependencies.MyLogger;
        }
    }
}