using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    interface IHasSetup
    {
        void Setup(Dependencies myDependencies);
    }
}