using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Microsoft.MixedReality.Toolkit.UI
{

    public class StateManager
    {
        public StateManager() 
        {
            //ActiveStates = defaultStates;
        }

        public List<State> ActiveStates;

        private List<State> defaultStates = new List<State>()
        {
            { new State() { Index = 0, Name = "Default", ActiveIndex = -1, Bit = 0, Value = 0 } },
            { new State() { Index = 1, Name = "Focus", ActiveIndex = -1, Bit = 0, Value = 0 } }
        };

        public State GetState(string stateName)
        {
            return ActiveStates.Find((x) => x.Name == stateName);
        }

        public State SetState(string stateName, int value)
        {
            State currentState = ActiveStates.Find((x) => x.Name == stateName);

            currentState.Value = value;

            return currentState;
        }

        //public bool CheckStateChange()
        //{
        //    foreach (State state in ActiveStates)
        //    {

        //    }
        //    return false;
        //}
    }
}
