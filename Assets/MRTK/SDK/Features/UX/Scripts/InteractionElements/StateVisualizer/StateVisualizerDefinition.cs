
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class StateVisualizerDefinition : ScriptableObject
    {
        [SerializeField]
        private List<StateStyleConfigurationContainer> stateStyleConfigurationContainers = new List<StateStyleConfigurationContainer>();

        /// <summary>
        /// 
        /// </summary>
        public List<StateStyleConfigurationContainer> StateStyleConfigurationContainers
        {
            get => stateStyleConfigurationContainers;
            protected set => stateStyleConfigurationContainers = value;
        }

        private GameObject defaultTarget = null;

        public void InitializeStateStyleContainers(TrackedStates trackedStates, GameObject target)
        {
            foreach(InteractionState state in trackedStates.States)
            {
                StateStyleConfigurationContainer stateStyleContainer = CreateInstance<StateStyleConfigurationContainer>();
                //stateStyleContainer.name = state.Name;
                stateStyleContainer.StateName = state.Name;
                stateStyleContainer.Target = target;

                StateStyleConfigurationContainers.Add(stateStyleContainer);
            }

            defaultTarget = target;
        }

        public void AddStateStyleProperty<T>(StateStyleConfigurationContainer stateStyleContainer, string stateName) where T : StateStylePropertyConfiguration
        {
            if (stateName != "Default")
            {
                if (stateName != null && stateStyleContainer != null)
                {
                    T stateStylePropertyInstance = CreateInstance<T>();
                    stateStylePropertyInstance.Target = stateStyleContainer.Target;
                    stateStylePropertyInstance.StateName = stateName;

                    stateStyleContainer.StateStyleProperties.Add(stateStylePropertyInstance);
                }
                else
                {
                    Debug.LogError($"The state entered {stateName} or the stateStyleContatiner does not exist and the state style property was not added.");
                }
            }
            else
            {
                Debug.LogError("The Default state is the appearance the object during edit mode an cannot have style properties added to it.");
            }
        }


        public void UpdateStateStyleContainers(TrackedStates trackedStates)
        {
            foreach (InteractionState state in trackedStates.States)
            {
                // If the state is in tracked states but there is not a state style container associated with it
                // Then add a new container for the state

                // Find the container that matches the state, if there is no container for the state then add one
                StateStyleConfigurationContainer styleContainer = StateStyleConfigurationContainers.Find((container) => (container.StateName == state.Name));

                if (styleContainer.IsNull())
                {
                    AddStateStyleContainer(state.Name);
                }
            }

            foreach (StateStyleConfigurationContainer styleContainer in StateStyleConfigurationContainers.ToList())
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
            StateStyleConfigurationContainer containerToRemove = StateStyleConfigurationContainers.Find((container) => container.StateName == stateName);

            StateStyleConfigurationContainers.Remove(containerToRemove);
        }

        private void AddStateStyleContainer(string stateName)
        {
            StateStyleConfigurationContainer stateStyleContainer = CreateInstance<StateStyleConfigurationContainer>();
            //stateStyleContainer.name = state.Name;
            stateStyleContainer.StateName = stateName;
            stateStyleContainer.Target = defaultTarget;
            StateStyleConfigurationContainers.Add(stateStyleContainer);
        }




        // Set the default state as the current appearance of the object
        private void SetDefaultState()
        {

        }

    }
}
