// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu(fileName = "StateVisualizerDefinition", menuName = "Mixed Reality Toolkit/State Visualizer/State Visualizer Definition")]
    public class StateVisualizerDefinition : ScriptableObject
    {
        [SerializeField]
        private TrackedStates trackedStates;

        /// <summary>
        /// The scriptable object that contains the states being tracked by an interactive element.  The tracked
        /// states scriptable within this StateVisualizerDefinition must match the tracked states in the interactive 
        /// element.
        /// </summary>
        public TrackedStates TrackedStates
        {
            get => trackedStates;
            set => trackedStates = value;
           
        }

        [SerializeField]
        private List<StateContainer> stateContainers = new List<StateContainer>();

        /// <summary>
        /// The container for each state that stores the list of the state style properties.  Each state in tracked states
        /// has an associated state container.
        /// </summary>
        public List<StateContainer> StateContainers
        {
            get => stateContainers;
            protected set => stateContainers = value;
        }

        [SerializeField]
        private GameObject defaultTarget = null;

        /// <summary>
        /// The default target game object for a new state style property.
        /// </summary>
        public GameObject DefaultTarget
        {
            get => defaultTarget;
            set => defaultTarget = value;
        }

        // List of the core style propery names
        private string[] coreStyleProperties = Enum.GetNames(typeof(CoreStyleProperty)).ToArray();

        public void InitializeStateContainers(GameObject target)
        {
            if (TrackedStates != null)
            {
                foreach (InteractionState state in TrackedStates.States)
                {
                    AddStateStyleContainer(state.Name);
                }
            }

            DefaultTarget = target;
        }

        public void UpdateStateStyleContainers()
        {
            if (TrackedStates.States.Count != StateContainers.Count)
            {
                // If the state is in tracked states but there is not a state container associated with it,
                // add a new container for the state
                if (TrackedStates.States.Count > StateContainers.Count)
                {
                    foreach (InteractionState state in TrackedStates.States)
                    {
                        // Find the container that matches the state
                        StateContainer styleContainer = StateContainers.Find((container) => (container.StateName == state.Name));

                        if (styleContainer == null)
                        {
                            AddStateStyleContainer(state.Name);
                        }
                    }
                }

                // If there is a state that has a state container but the state is not in tracked states
                else if (TrackedStates.States.Count < StateContainers.Count) 
                {
                    // If there is no tracked state for the matching container
                    // then remove the container from the definition
                    foreach (StateContainer styleContainer in StateContainers.ToList())
                    {
                        // Find the state in tracked states for this container
                        InteractionState trackedState = TrackedStates.States.Find((state) => (state.Name == styleContainer.StateName));

                        // Do not remove the default state
                        if (trackedState == null && styleContainer.StateName != "Default")
                        {
                            RemoveStateStyleContainer(styleContainer.StateName);
                        }
                    }
                }
            }
        }
        public StateContainer GetStateContainer(string stateName)
        {
            StateContainer stateContainer = StateContainers.Find((container) => container.StateName == stateName);

            if (stateContainer != null)
            {
                return stateContainer;
            }
            else
            {
                Debug.LogError($"The {stateName} state does not have an existing state container for state style properties");
                return null;
            }
        }

        private void RemoveStateStyleContainer(string stateName)
        {
            StateContainer containerToRemove = StateContainers.Find((container) => container.StateName == stateName);

            StateContainers.Remove(containerToRemove);
        }

        private void AddStateStyleContainer(string stateName)
        {
            //StateContainer stateStyleContainer = ScriptableObject.CreateInstance<StateContainer>();
            StateContainer stateStyleContainer = new StateContainer(stateName)
            {
                DefaultTarget = DefaultTarget
            };

            StateContainers.Add(stateStyleContainer);
        }

        // For the inspector 
        public StateStylePropertyConfiguration AddStateStyleProperty(StateContainer stateContainer, CoreStyleProperty styleProperty, GameObject target = null)
        {

            StateStylePropertyConfiguration stateStylePropertyInstance = (StateStylePropertyConfiguration)ScriptableObject.CreateInstance(styleProperty + "StateStylePropertyConfiguration");
            stateStylePropertyInstance.StateName = stateContainer.StateName;
            stateStylePropertyInstance.name = styleProperty + stateContainer.StateName;

            if (target == null)
            {
                stateStylePropertyInstance.Target = DefaultTarget;
            }
            else
            {
                stateStylePropertyInstance.Target = target;
            }

            stateContainer.StateStyleProperties.Add(stateStylePropertyInstance);

            return stateStylePropertyInstance;
        }


        public StateStylePropertyConfiguration AddStateStyleProperty(StateContainer stateContainer, string styleProperty, GameObject target = null)
        {
            if (coreStyleProperties.Contains(styleProperty))
            {
                StateStylePropertyConfiguration stateStylePropertyInstance = (StateStylePropertyConfiguration)ScriptableObject.CreateInstance(styleProperty + "StateStylePropertyConfiguration");
                stateStylePropertyInstance.StateName = stateContainer.StateName;
                stateStylePropertyInstance.name = styleProperty + stateContainer.StateName;

                if (target == null)
                {
                    stateStylePropertyInstance.Target = DefaultTarget;
                }
                else
                {
                    stateStylePropertyInstance.Target = target;
                }

                stateContainer.StateStyleProperties.Add(stateStylePropertyInstance);

                return stateStylePropertyInstance;
            }
            else
            {
                Debug.LogError($"The {styleProperty} style property could not be added becasue a configuration does not exist for the property.");
                return null;
            }
        }

        public void RemoveStateStyleProperty(StateContainer stateContainer, int index)
        {
            stateContainer.StateStyleProperties.RemoveAt(index);
        }

    }
}
