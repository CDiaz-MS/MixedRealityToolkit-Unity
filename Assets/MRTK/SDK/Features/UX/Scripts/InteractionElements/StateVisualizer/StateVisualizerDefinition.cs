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

        public GameObject DefaultTarget;

        public void InitializeStateContainers(TrackedStates trackedStates, GameObject target)
        {
            foreach(InteractionState state in trackedStates.States)
            {
                StateContainer stateStyleContainer = CreateInstance<StateContainer>();
                stateStyleContainer.name = state.Name + "Container";
                stateStyleContainer.StateName = state.Name;
                stateStyleContainer.DefaultTarget = DefaultTarget;

                StateContainers.Add(stateStyleContainer);
            }
        }

        //public void AddStateStyleProperty<T>(StateContainer stateStyleContainer, string stateName, GameObject target) where T : StateStylePropertyConfiguration
        //{
        //    if (stateName != "Default")
        //    {
        //        if (stateName != null && stateStyleContainer != null)
        //        {
        //            T stateStylePropertyInstance = CreateInstance<T>();
        //            stateStylePropertyInstance.Target = target;
        //            stateStylePropertyInstance.StateName = stateName;

        //            stateStyleContainer.StateStyleProperties.Add(stateStylePropertyInstance);
        //        }
        //        else
        //        {
        //            Debug.LogError($"The state entered {stateName} or the stateStyleContatiner does not exist and the state style property was not added.");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("The Default state is the appearance the object during edit mode an cannot have style properties added to it.");
        //    }
        //}


        public void UpdateStateStyleContainers(TrackedStates trackedStates)
        {
            foreach (InteractionState state in trackedStates.States)
            {
                // If the state is in tracked states but there is not a state style container associated with it
                // Then add a new container for the state

                // Find the container that matches the state, if there is no container for the state then add one
                StateContainer styleContainer = StateContainers.Find((container) => (container.StateName == state.Name));

                if (styleContainer.IsNull())
                {
                    AddStateStyleContainer(state.Name);
                }
            }

            foreach (StateContainer styleContainer in StateContainers.ToList())
            {
                // Find the tracked state and the matching container, if there is no tracked state for the matching container
                // then remove the container from the definition
                InteractionState trackedState = trackedStates.States.Find((state) => (state.Name == styleContainer.StateName));

                if (trackedState.IsNull() && styleContainer.StateName != "Default")
                {
                    RemoveStateStyleContainer(styleContainer.StateName);
                }
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
            stateStyleContainer.name = stateName;
            stateStyleContainer.StateName = stateName;
            stateStyleContainer.DefaultTarget = DefaultTarget;
            StateContainers.Add(stateStyleContainer);
        }
    }
}
