using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;



namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public class StateManager 
    {
        public StateManager(TrackedStates trackedStates) 
        {
            TrackedStates = trackedStates.States;
        }

        public List<InteractionState> TrackedStates { get; protected set;}

        public InteractionStateActiveEvent OnStateActivated { get; protected set; } = new InteractionStateActiveEvent();

        public InteractionStateInactiveEvent OnStateDeactivated { get; protected set; } = new InteractionStateInactiveEvent();

        private string[] availableStates => Enum.GetNames(typeof(CoreInteractionState)).ToArray();

        private InteractionState currentStateSetActive;

        /// <summary>
        /// Get a non core state by name
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public InteractionState GetState(string stateName)
        {
            return TrackedStates.Find((state) => state.Name == stateName);
        }

        /// <summary>
        /// Get a core interaction state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public InteractionState GetState(CoreInteractionState coreState)
        {
            return TrackedStates.Find((state) => state.Name == coreState.ToString());
        }

        public InteractionState SetState(string stateName, int value)
        {
            // Add guards for if the user enters an invalid value or name

            InteractionState state = GetState(stateName);

            if (state != null)
            {
                state.Value = value;

                if (value > 0)
                {
                    state.Active = 1;

                    OnStateActivated.Invoke(state);

                    currentStateSetActive = state;

                }
                else
                {
                    state.Active = 0;

                    OnStateDeactivated.Invoke(currentStateSetActive, state);
                }

                return state;
            }
            else
            {
                Debug.LogError("The state name " + stateName +  " does not exist within TrackedStates, check the spelling.");
                return null;
            }
        }


        public InteractionState SetState(CoreInteractionState coreState, int value)
        {
            // Add guards for if the user enters an invalid value or name

            InteractionState state = GetState(coreState);

            if (state != null)
            {
                state.Value = value;

                if (value > 0)
                {
                    state.Active = 1;

                    // Used in the State Visualizer to trigger a property transition
                    OnStateActivated.Invoke(state);


                    // ORDER MATTERS, make it not matter
                    currentStateSetActive = state;
                }
                else
                {
                    state.Active = 0;

                }

                return state;
            }
            else
            {
                return null;
            }
        }


        public InteractionState SetStateOn(CoreInteractionState coreState)
        {
            InteractionState state = GetState(coreState);

            if (state != null)
            {
                state.Value = 1;

                state.Active = 1;

                OnStateActivated.Invoke(state);

                // ORDER MATTERS, make it not matter
                currentStateSetActive = state;

                return state;
            }
            else
            {
                Debug.LogError($"The core state {coreState} is not being tracked, add this state to TrackedStates to set it");
                return null;
            }
        }

        public InteractionState SetStateOff(CoreInteractionState coreState)
        {
            InteractionState state = GetState(coreState);

            if (state != null)
            {
                state.Value = 0;

                state.Active = 0;

                // We need to save the last state active state so we can add transitions 
                OnStateDeactivated.Invoke(state, currentStateSetActive);

                return state;
            }
            else
            {
                Debug.LogError($"The core state {coreState} is not being tracked, add this state to TrackedStates to set it");
                return null;
            }
        }


        public InteractionState AddCoreState(CoreInteractionState state)
        {
            if (GetState(state) == null)
            {
                InteractionState newState = new InteractionState(state.ToString());

                var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
                var eventConfigType = eventConfigurationTypes.Find(t => t.Name.Contains(state.ToString()));

                // Check if the core state has a custom event configuration
                if (eventConfigType != null)
                {
                    string className = eventConfigType.Name;

                    // Set the state event configuration 
                    newState.EventConfiguration = (BaseInteractionEventConfiguration)ScriptableObject.CreateInstance(className);
                    newState.EventConfiguration.StateName = state + "EventConfiguration";
                }

                TrackedStates.Add(newState);

                return newState;
            }

            return null;
        }


        public void CreateNewStateWithEventConfiguration(string stateName, BaseInteractionEventConfiguration eventConfiguration)
        {
            if (!stateName.IsNull() && !eventConfiguration.IsNull())
            {
                if (!availableStates.Contains(stateName))
                {
                    InteractionState newState = new InteractionState(stateName);
                    newState.EventConfiguration = eventConfiguration;
                    TrackedStates.Add(newState);
                }
                else
                {
                    Debug.LogError($"The state name {stateName} is a defined core state, please use AddCoreState() to add to Tracked States.");
                }

            }
            else
            {
                Debug.LogError($"The state name or event configuration entered is null");
            }
        }

        public void CreateAndAddNewState(string stateName)
        {
            if (GetState(stateName) == null)
            {
                InteractionState newState = new InteractionState(stateName);
                TrackedStates.Add(newState);
            }
            else
            {
                Debug.Log("The state name " + stateName + " is a defined core state, please use AddNewState(" + stateName + ") to add to Tracked States.");
            }
        }
    }
}
