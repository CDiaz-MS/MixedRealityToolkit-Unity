// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Manages state values of Interaction States within the Tracked States Scriptable Object.  The Tracked States
    /// scriptable object is contained within a class that derives from BaseInteractionElement.  This class contains helper
    /// methods for setting, getting and creating Interaction States.
    /// </summary>
    public class StateManager 
    {
        /// <summary>
        /// Create a new state manager with a given states scriptable object.
        /// </summary>
        /// <param name="trackedStates"></param>
        public StateManager(TrackedStates trackedStates) 
        {
            states = trackedStates.States;
        }

        private List<InteractionState> states = null;

        // Make the public list readonly to prevent users from editing the list directly
        // forces the use of the AddCoreState, RemoveCoreState methods that contain checks and error logs
        public IList<InteractionState> States => states.AsReadOnly();

        public InteractionStateActiveEvent OnStateActivated { get; protected set; } = new InteractionStateActiveEvent();

        public InteractionStateInactiveEvent OnStateDeactivated { get; protected set; } = new InteractionStateInactiveEvent();

        private string[] coreStates = Enum.GetNames(typeof(CoreInteractionState)).ToArray();

        private InteractionState currentStateSetActive;

        /// <summary>
        /// Get a non core or core interaction state by name
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public InteractionState GetState(string stateName)
        {
            return states.Find((state) => state.Name == stateName);
        }

        /// <summary>
        /// Get a core interaction state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public InteractionState GetState(CoreInteractionState coreState)
        {
            return states.Find((state) => state.Name == coreState.ToString());
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
                Debug.LogError("The state name " + stateName +  " does not exist within states, check the spelling.");
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

            if (state != null )
            {
                if (state.Value != 1)
                {
                    state.Value = 1;

                    state.Active = 1;

                    OnStateActivated.Invoke(state);
                }


                // ORDER MATTERS, make it not matter
                currentStateSetActive = state;

                return state;
            }
            else
            {
                Debug.LogError($"The core state {coreState} is not being tracked, add this state to states to set it");
                return null;
            }
        }

        public InteractionState SetStateOff(CoreInteractionState coreState)
        {
            InteractionState state = GetState(coreState);

            if (state != null)
            {
                if (state.Value != 0)
                {
                    state.Value = 0;

                    state.Active = 0;

                    // We need to save the last state active state so we can add transitions 
                    OnStateDeactivated.Invoke(state, currentStateSetActive);
                }

                return state;
            }
            else
            {
                Debug.LogError($"The core state {coreState} is not being tracked, add this state to states to set it");
                return null;
            }
        }


        public InteractionState AddCoreState(CoreInteractionState state)
        {
            InteractionState coreState = GetState(state);

            if (coreState == null)
            {
                // Create a new core state
                InteractionState newState = new InteractionState(state.ToString());

                // Set the event configuration if one exists for the state
                SetEventConfigurationOfCoreState(newState);

                states.Add(newState);

                //// Add a near interaction touchable if it is not present on the item 
                //if (state == CoreInteractionState.Touch)
                //{

                //}

                //// Add a near interaction grabbable if it is not present on the item
                //if (state == CoreInteractionState.Grab)
                //{

                //}

                return newState;
            }
            else
            {
                Debug.Log($" The {state} state is already being tracked and does not need to be added");
                return coreState;
            }
        }

        public void RemoveCoreState(CoreInteractionState state)
        {
            InteractionState coreState = GetState(state);

            if (state == CoreInteractionState.Default)
            {
                if (coreState != null)
                {
                    states.Remove(coreState);
                }
                else
                {
                    Debug.Log($"The {state} state is not tracking and has already been removed.");
                }
            }
            else
            {
                Debug.Log($"The {state} state cannot be removed");
            }
        }


        private void SetEventConfigurationOfCoreState(InteractionState coreState)
        {
            var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
            var eventConfigType = eventConfigurationTypes.Find(t => t.Name.StartsWith(coreState.ToString()));

            // Check if the core state has a custom event configuration
            if (eventConfigType != null)
            {
                string className = eventConfigType.Name;

                // Set the state event configuration 
                coreState.EventConfiguration = (BaseInteractionEventConfiguration)ScriptableObject.CreateInstance(className);
            }
            else
            {
                Debug.Log($" The {coreState.Name} state does not have an existing event configuration");
            }
        }


        public void CreateNewStateWithEventConfiguration(string stateName, BaseInteractionEventConfiguration eventConfiguration)
        {
            if (!stateName.IsNull() && !eventConfiguration.IsNull())
            {
                if (!coreStates.Contains(stateName))
                {
                    InteractionState newState = new InteractionState(stateName);
                    newState.EventConfiguration = eventConfiguration;
                    states.Add(newState);
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
                states.Add(newState);
            }
            else
            {
                Debug.Log("The state name " + stateName + " is a defined core state, please use AddNewState(" + stateName + ") to add to Tracked States.");
            }
        }

        // IF the tracked states list is edited during runtime, make sure to remove the state from the state visualizer

        //public bool HasTrackedStatesBeenModified()
        //{
        //    if (states.Count != trackedStatesCount)
        //    {
        //        return true;
        //    }

        //    return false;
        //}
    }
}
