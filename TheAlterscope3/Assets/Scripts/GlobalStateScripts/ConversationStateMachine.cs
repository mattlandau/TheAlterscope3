using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheAlterscope2
{
    public enum ConversationState
    {
        Placing,
        AvatarsPlaced,
        Recording,
        Playback,
        AnimateSlideOut,
        AnimateSlideIn,
        ReadyToRecord,
        DelayAfterPlayback,
        DelayBeforePlayback,
        DelayAfterRecording,
        WalkingAround,
    }

    public enum Command { BeginRecording, BeginPlayback, BeginPlaybackAll, BeginPlacing, BeginSlideOut, BeginSlideIn, BeginDelayBeforePlayback, BeginDelayAfterPlayback, BeginDelayAfterRecording, BeginReadyToRecord, BeginWalkingAround, BeginAvatarsPlaced }

    public class ConversationStateMachine : MonoBehaviour, IHasSetup
    {
        ILogger _logger;
        Dictionary<StateTransition, ConversationState> _transitions;
        public ConversationState CurrentState;
        public ConversationState PreviousState;
        public delegate void StateEntered(ConversationState newState);
        public StateEntered OnStateEntered;
        public delegate void StateExited(ConversationState oldState);
        public StateExited OnStateExited;

        public void Setup(Dependencies myDependencies)
        {
            _logger = myDependencies.MyLogger ?? Factory.CreateLogger(myDependencies);
            _transitions = new Dictionary<StateTransition, ConversationState>
            {
                { new StateTransition(ConversationState.Placing, Command.BeginAvatarsPlaced), ConversationState.AvatarsPlaced },
                { new StateTransition(ConversationState.AvatarsPlaced, Command.BeginReadyToRecord), ConversationState.ReadyToRecord },
                { new StateTransition(ConversationState.ReadyToRecord, Command.BeginRecording), ConversationState.Recording },
                { new StateTransition(ConversationState.Recording, Command.BeginDelayAfterRecording), ConversationState.DelayAfterRecording },
                { new StateTransition(ConversationState.DelayAfterRecording, Command.BeginSlideIn), ConversationState.AnimateSlideIn },
                { new StateTransition(ConversationState.DelayAfterRecording, Command.BeginSlideOut), ConversationState.AnimateSlideOut },
                { new StateTransition(ConversationState.AnimateSlideIn, Command.BeginDelayBeforePlayback), ConversationState.DelayBeforePlayback },
                { new StateTransition(ConversationState.AnimateSlideOut, Command.BeginDelayBeforePlayback), ConversationState.DelayBeforePlayback },
                { new StateTransition(ConversationState.DelayBeforePlayback, Command.BeginPlayback), ConversationState.Playback },
                { new StateTransition(ConversationState.DelayBeforePlayback, Command.BeginWalkingAround), ConversationState.WalkingAround },
                { new StateTransition(ConversationState.Playback, Command.BeginDelayAfterPlayback), ConversationState.DelayAfterPlayback },
                { new StateTransition(ConversationState.Playback, Command.BeginWalkingAround), ConversationState.WalkingAround },
                { new StateTransition(ConversationState.WalkingAround, Command.BeginDelayBeforePlayback), ConversationState.DelayBeforePlayback },
                { new StateTransition(ConversationState.DelayAfterPlayback, Command.BeginReadyToRecord), ConversationState.ReadyToRecord },
                { new StateTransition(ConversationState.DelayAfterPlayback, Command.BeginDelayBeforePlayback), ConversationState.DelayBeforePlayback },
                { new StateTransition(ConversationState.DelayAfterPlayback, Command.BeginWalkingAround), ConversationState.WalkingAround },
                { new StateTransition(ConversationState.WalkingAround, Command.BeginReadyToRecord), ConversationState.ReadyToRecord },
                { new StateTransition(ConversationState.DelayBeforePlayback, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.DelayAfterPlayback, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.DelayAfterRecording, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.WalkingAround, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.AnimateSlideIn, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.AnimateSlideOut, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.ReadyToRecord, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.Playback, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.Recording, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.Placing, Command.BeginPlacing), ConversationState.Placing },
                { new StateTransition(ConversationState.ReadyToRecord, Command.BeginWalkingAround), ConversationState.WalkingAround },
                

                //on resumption
                { new StateTransition(ConversationState.Placing, Command.BeginDelayAfterRecording), ConversationState.DelayAfterRecording },
                { new StateTransition(ConversationState.AnimateSlideIn, Command.BeginAvatarsPlaced), ConversationState.AvatarsPlaced },
                { new StateTransition(ConversationState.AnimateSlideOut, Command.BeginAvatarsPlaced), ConversationState.AvatarsPlaced },

                // bug fix
                { new StateTransition(ConversationState.DelayAfterRecording, Command.BeginAvatarsPlaced), ConversationState.AvatarsPlaced },


            };
            CurrentState = ConversationState.Placing;
            PreviousState = CurrentState;
        }

        public ConversationState GetNext(Command command)
        {
            StateTransition transition = new StateTransition(CurrentState, command);
            ConversationState nextState;
            if (!_transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
            return nextState;
        }

        public ConversationState MoveNext(Command command)
        {
            var oldState = CurrentState;
            CurrentState = GetNext(command);
            _logger.Log("Old state " + oldState.ToString());
            OnStateExited?.Invoke(oldState);
            _logger.Log("Entered state: " + CurrentState);
            PreviousState = oldState;
            OnStateEntered?.Invoke(CurrentState);
            return CurrentState;
        }
    }
}