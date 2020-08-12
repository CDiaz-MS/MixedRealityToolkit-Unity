using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{

    public class StateManager 
    {
        public StateManager() 
        {
            //ActiveStates = defaultStates;
        }

        public List<InteractionState> TrackedStates;

        public InteractionState CurrentState = new InteractionState("None");

        private bool stateChanged = false;

        private List<int> currentStateValues = new List<int>();

        private List<int> pastStateValues = new List<int>();

        public InteractionState GetState(string stateName)
        {
            return TrackedStates.Find((x) => x.Name == stateName);
        }

        public InteractionState SetState(string stateName, int value)
        {
            InteractionState currentState = TrackedStates.Find((x) => x.Name == stateName);
            


            currentState.Value = value;

            if (value > 0)
            {
                currentState.Active = 1;

                CurrentState = currentState;

                stateChanged = true;

            }
            else
            {
                currentState.Active = 0;
            }


            return currentState;
        }


        public InteractionState CheckStateChange()
        {
            if (stateChanged)
            {
                stateChanged = false;


                return CurrentState;
                

                

            }
            CurrentState = null;
            return null;
            
            
        }

    }
}
