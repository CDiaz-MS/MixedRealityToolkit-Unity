
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
        private List<StateStylePropertiesConfigurationContainer> stateStyleProperties = new List<StateStylePropertiesConfigurationContainer>();

        /// <summary>
        /// 
        /// </summary>
        public List<StateStylePropertiesConfigurationContainer> StateStyleProperties
        {
            get => stateStyleProperties;
            set => stateStyleProperties = value;
        }

        public void SetUp(TrackedStates trackedStates, GameObject target)
        {
            foreach(InteractionState state in trackedStates.StateList)
            {
                StateStylePropertiesConfigurationContainer stateStyleProps = CreateInstance<StateStylePropertiesConfigurationContainer>();
                stateStyleProps.name = state.Name;
                stateStyleProps.StateName = state.Name;
                stateStyleProps.Target = target;

                StateStyleProperties.Add(stateStyleProps);
            }
        }

        public void AddStateStyleProperty<T>(StateStylePropertiesConfigurationContainer stateStyleContainer, InteractionState state) where T : StateStylePropertyConfiguration
        {
            T stateStylePropertyInstance = CreateInstance<T>();
            stateStylePropertyInstance.name = stateStylePropertyInstance.StateStylePropertyName;
            stateStylePropertyInstance.Target = stateStyleContainer.Target;
            stateStylePropertyInstance.State = state;

            stateStyleContainer.StateStylePropList.Add(stateStylePropertyInstance);
        }


        // Set the default state as the current appearance of the object
        private void SetDefaultState()
        {

        }

    }
}
