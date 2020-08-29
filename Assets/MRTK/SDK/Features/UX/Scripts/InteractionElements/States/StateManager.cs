// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        // forces the use of the AddCoreState(), RemoveCoreState() methods that contain checks to edit the list
        public IList<InteractionState> States => states.AsReadOnly();

        public InteractionStateActiveEvent OnStateActivated { get; protected set; } = new InteractionStateActiveEvent();

        public InteractionStateInactiveEvent OnStateDeactivated { get; protected set; } = new InteractionStateInactiveEvent();

        private string[] coreStates = Enum.GetNames(typeof(CoreInteractionState)).ToArray();

        private InteractionState currentStateSetActive;

        public List<InteractionState> activeStates = new List<InteractionState>();


        #region State Methods for CoreInteractionStates 


        /// <summary>
        /// Get a Core Interaction State
        /// </summary>
        /// <param name="state">The type of CoreInteractionState</param>
        /// <returns>The Interaction State contained in the Tracked States scriptable object</returns>
        public InteractionState GetState(CoreInteractionState coreState)
        {
            InteractionState interactionState = states.Find((state) => state.Name == coreState.ToString());

            if (interactionState != null)
            {
                return interactionState;
            }
            else
            {
                return null;
            }
        }

        public InteractionState SetState(CoreInteractionState coreState, int value)
        {
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
                Debug.LogError($"The {coreState} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
        }

        public InteractionState SetStateOn(CoreInteractionState coreState)
        {
            InteractionState state = GetState(coreState);

            if (state != null)
            {
                if (state.Value != 1)
                {
                    state.Value = 1;

                    state.Active = 1;

                    OnStateActivated.Invoke(state);

                    // Only add the state to activeStates if it is not present 
                    if (!activeStates.Contains(state))
                    {
                        activeStates.Add(state);
                    }

                    InteractionState defaultState = GetState(CoreInteractionState.Default);

                    // If the state getting switched on is NOT the default state, then make sure the default state is off
                    // The default state is only active when ALL other states are not active
                    if (state.Name != "Default" && defaultState.Value == 1)
                    {
                        Debug.Log("Set Defalut off");
                        SetStateOff(CoreInteractionState.Default);
                    }
                }

                return state;
            }
            else
            {
                Debug.LogError($"The {coreState} state is not being tracked, add this state using AddState(state) to set it");
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

                    // If the only state in active states is going to be removed, then activate the default state
                    if (activeStates.Count == 1 && activeStates.First() == state)
                    {
                        SetStateOn(CoreInteractionState.Default);
                    }

                    // We need to save the last state active state so we can add transitions 
                    OnStateDeactivated.Invoke(state, activeStates.Last());

                    activeStates.Remove(state);
                }

                return state;
            }
            else
            {
                Debug.LogError($"The {coreState} state is not being tracked, add this state using AddState(state) to set it");
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

                // When support for the touch and grab states are added:

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
                Debug.Log($" The {state} state is already being tracked and does not need to be added.");
                return coreState;
            }
        }

        public void RemoveCoreState(CoreInteractionState state)
        {
            InteractionState coreState = GetState(state);

            if (coreState != null)
            {
                if (state != CoreInteractionState.Default)
                {
                    states.Remove(coreState);
                }
                else
                {
                    Debug.LogError($"The Default state cannot be removed");
                }
            }
            else
            {
                Debug.LogError($"The {state} state is not being tracked and was not removed.");

            }
        }
        
        #endregion


        #region State Methods for Non-CoreInteractionStates

        /// <summary>
        /// Get a non core or core interaction state by name
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public InteractionState GetState(string stateName)
        {
            InteractionState interactionState = states.Find((state) => state.Name == stateName);

            if (interactionState != null)
            {
                return interactionState;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set the state and value of an Interaction State by state name 
        /// </summary>
        /// <param name="stateName">The name of the state to set</param>
        /// <param name="value">The value to set the state to.  Use 0 to turn the state off, 1 to set the state to on.</param>
        /// <returns></returns>
        public InteractionState SetState(string stateName, int value)
        {
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
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
        }

        public InteractionState SetStateOn(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state != null)
            {
                if (state.Value != 1)
                {
                    state.Value = 1;

                    state.Active = 1;

                    OnStateActivated.Invoke(state);

                    // Only add the state to activeStates if it is not present 
                    if (!activeStates.Contains(state))
                    {
                        activeStates.Add(state);
                    }

                    InteractionState defaultState = GetState(CoreInteractionState.Default);

                    // If the state getting switched on is NOT the default state, then make sure the default state is off
                    // The default state is only active when ALL other states are not active
                    if (state.Name != "Default" && defaultState.Value == 1)
                    {
                        Debug.Log("Set Defalut off");
                        SetStateOff(CoreInteractionState.Default);
                    }
                }

                return state;
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
        }

        public InteractionState SetStateOff(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state != null)
            {
                if (state.Value != 0)
                {
                    state.Value = 0;

                    state.Active = 0;

                    // If the only state in active states is going to be removed, then activate the default state
                    if (activeStates.Count == 1 && activeStates.First() == state)
                    {
                        SetStateOn(CoreInteractionState.Default);
                    }

                    // We need to save the last state active state so we can add transitions 
                    OnStateDeactivated.Invoke(state, activeStates.Last());

                    activeStates.Remove(state);
                }

                return state;
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
        }

        public InteractionState AddNewState(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state == null)
            {
                InteractionState newState = new InteractionState(stateName);
                states.Add(newState);
                return state;
            }
            else
            {
                Debug.Log($" The {stateName} state is already being tracked and does not need to be added.");
                return state;
            }
        }

        public void RemoveState(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state != null)
            {
                if (stateName != "Default")
                {
                    states.Remove(state);
                }
                else
                {
                    Debug.LogError($"The {state.Name} state cannot be removed.");
                }
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked and was not removed.");
            }

        }

        public void AddNewStateWithEventConfiguration(string stateName, BaseInteractionEventConfiguration eventConfiguration)
        {
            if (stateName != null && eventConfiguration != null)
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

        #endregion

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
    }
}
