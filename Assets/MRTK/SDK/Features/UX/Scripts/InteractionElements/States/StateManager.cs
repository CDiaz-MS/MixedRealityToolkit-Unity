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

        public InteractionStateActiveEvent OnStateActivated = new InteractionStateActiveEvent();

        //public InteractionStateInactiveEvent OnStateDeactivated = new InteractionStateInactiveEvent();

        public InteractionState GetState(string stateName)
        {
            return TrackedStates.Find((x) => x.Name == stateName);
        }

        public InteractionState SetState(string stateName, int value)
        {
            // Add guards for if the user enters an invalid value or name

            InteractionState currentState = GetState(stateName);

            if (currentState != null)
            {
                currentState.Value = value;

                if (value > 0)
                {
                    currentState.Active = 1;

                    OnStateActivated.Invoke(currentState);

                }
                else
                {
                    currentState.Active = 0;

                    //OnStateDeactivated.Invoke(previousState, currentState);
                }

                //previousState = currentState;

                return currentState;
            }
            else
            {
                Debug.LogError("The state name " + stateName +  " does not exist within TrackedStates, check the spelling.");
                return null;
            }
            

        }

    }
}
