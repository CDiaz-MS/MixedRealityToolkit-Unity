using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [RequireComponent(typeof(BaseInteractable))]
    public class StateVisualizer : MonoBehaviour
    {
        [SerializeField]
        private StateVisualizerDefinition stateVisualizerDefinition;

        /// <summary>
        /// 
        /// </summary>
        public StateVisualizerDefinition StateVisualizerDefinition
        {
            get => stateVisualizerDefinition;
            set
            {
                stateVisualizerDefinition = value;
            }
        }

        public BaseInteractable BaseInteractable => GetComponent<BaseInteractable>();

        public TrackedStates TrackedStates => BaseInteractable.TrackedStates;

        private StateManager StateManager => BaseInteractable.StateManager;


        public void OnValidate()
        {
            //StateVisualizerDefinition = ScriptableObject.CreateInstance<StateVisualizerDefinition>();


            Debug.Log("State Visualizer Definition");

            if (StateVisualizerDefinition != null)
            {
                if (StateVisualizerDefinition.StateStyleProperties.Count == 0)
                {
                    StateVisualizerDefinition.SetUp(TrackedStates, gameObject);

                    AddStateStylePropertyToAState<MaterialStateStylePropertyConfiguration>("Focus");
                }
            }
        }


        public void AddStateStylePropertyToAState<T>(string stateName) where T : StateStylePropertyConfiguration
        {
            // Find the container with the state
            StateStylePropertiesConfigurationContainer stateStyleContainer = StateVisualizerDefinition.StateStyleProperties.Find((container) => (container.StateName == stateName));

            InteractionState interactionState = TrackedStates.StateList.Find((state) => (state.Name == stateName));

            StateVisualizerDefinition.AddStateStyleProperty<T>(stateStyleContainer, interactionState);
        }



        public void ActivateStateStyleProperty(string stateName)
        {
            // Change all properties with the StateName

            StateStylePropertiesConfigurationContainer stateStyleProperties = StateVisualizerDefinition.StateStyleProperties.Find((x) => x.StateName == stateName);

            foreach (StateStylePropertyConfiguration stylePropertyConfig in stateStyleProperties.StateStylePropList)
            {
                if (stylePropertyConfig.GetType() == typeof(MaterialStateStylePropertyConfiguration))
                {
                    SetStyleProperty<MaterialStateStylePropertyConfiguration>(stylePropertyConfig);
                }
            }
        }

        public void SetStyleProperty<T>(StateStylePropertyConfiguration stylePropertyConfig) where T: StateStylePropertyConfiguration
        {
            T configuration = stylePropertyConfig as T;

            configuration.CreateRuntimeInstance();
            configuration.StateStyleProperty.SetStyleProperty();
        }

        private void Start()
        {
            // Add listeners to the OnStateActivated(InteractionState)
            // enable all the style properties 
            StateManager.OnStateActivated.AddListener(
                (state) =>
                {
                    //Debug.Log(state.Name + " Event");
                    ActivateStateStyleProperty(state.Name);
                });

            //StateManager.OnStateDeactivated.AddListener(
            //    (previousState, currentState) =>
            //    {
            //        Debug.Log("PreviousState: " + previousState.Name );
            //        Debug.Log("CurrentState: " + currentState.Name );
            //    });
        }

    }
}
