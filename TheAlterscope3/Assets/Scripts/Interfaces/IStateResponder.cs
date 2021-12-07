using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    interface IStateResponder
    {
        void OnNewState(ConversationState newState);
    }
}