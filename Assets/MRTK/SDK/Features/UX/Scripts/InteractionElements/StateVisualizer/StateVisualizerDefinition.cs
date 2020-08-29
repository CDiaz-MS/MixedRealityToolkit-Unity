// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu(fileName = "StateVisualizerDefinition", menuName = "Mixed Reality Toolkit/State Visualizer/State Visualizer Definition")]
    public class StateVisualizerDefinition : ScriptableObject
    {
        [SerializeField]
        private List<StateContainer> stateStyleConfigurationContainers = new List<StateContainer>();

        /// <summary>
        /// 
        /// </summary>
        public List<StateContainer> StateContainers
        {
            get => stateStyleConfigurationContainers;
            protected set => stateStyleConfigurationContainers = value;
        }

        [SerializeField]
        private GameObject defaultTarget = null;

        /// <summary>
        /// 
        /// </summary>
        public GameObject DefaultTarget
        {
            get => defaultTarget;
            set => defaultTarget = value;
        }

        public void InitializeStateContainers(TrackedStates trackedStates, GameObject target)
        {
            foreach(InteractionState state in trackedStates.States)
            {
                StateContainer stateStyleContainer = CreateInstance<StateContainer>();
                stateStyleContainer.name = state.Name + "Container";
                stateStyleContainer.StateName = state.Name;
                stateStyleContainer.DefaultTarget = target;

                StateContainers.Add(stateStyleContainer);
            }

            DefaultTarget = target;
        }

        public void UpdateStateStyleContainers(TrackedStates trackedStates)
        {
            if (trackedStates.States.Count != StateContainers.Count)
            {
                // If the state is in tracked states but there is not a state container associated with it,
                // add a new container for the state
                if (trackedStates.States.Count > StateContainers.Count)
                {
                    foreach (InteractionState state in trackedStates.States)
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
                else if (trackedStates.States.Count < StateContainers.Count) 
                {
                    // If there is no tracked state for the matching container
                    // then remove the container from the definition
                    foreach (StateContainer styleContainer in StateContainers.ToList())
                    {
                        // Find the state in tracked states for this container
                        InteractionState trackedState = trackedStates.States.Find((state) => (state.Name == styleContainer.StateName));

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
            StateContainer stateStyleContainer = CreateInstance<StateContainer>();
            stateStyleContainer.name = stateName + "Container";
            stateStyleContainer.StateName = stateName;
            stateStyleContainer.DefaultTarget = DefaultTarget;

            StateContainers.Add(stateStyleContainer);
        }
    }
}
