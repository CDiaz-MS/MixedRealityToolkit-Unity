
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class StateVisualizerDefinition : ScriptableObject
    {
        // All the game objects contained in the definition 
        [SerializeField]
        private GameObject target = null;

        /// <summary>
        /// 
        /// </summary>
        public GameObject Target
        {
            get => target;
            set => target = value;
        }

        [SerializeField]
        private List<StateStyleConfigurationContainer> stateStyleConfigurationContainers = new List<StateStyleConfigurationContainer>();

        /// <summary>
        /// 
        /// </summary>
        public List<StateStyleConfigurationContainer> StateStyleConfigurationContainers
        {
            get => stateStyleConfigurationContainers;
            set => stateStyleConfigurationContainers = value;
        }

        public void SetUp(TrackedStates trackedStates, GameObject target)
        {
            foreach(InteractionState state in trackedStates.States)
            {
                StateStyleConfigurationContainer stateStyleContainer = CreateInstance<StateStyleConfigurationContainer>();
                //stateStyleContainer.name = state.Name;
                stateStyleContainer.StateName = state.Name;
                stateStyleContainer.Target = target;

                StateStyleConfigurationContainers.Add(stateStyleContainer);
            }
        }

        public void AddStateStyleProperty<T>(StateStyleConfigurationContainer stateStyleContainer, string stateName) where T : StateStylePropertyConfiguration
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
                Debug.LogError("The state entered or the stateStyleContatiner does not exist and the state style property was not added");
            }
        }


        // Set the default state as the current appearance of the object
        private void SetDefaultState()
        {

        }

    }
}
