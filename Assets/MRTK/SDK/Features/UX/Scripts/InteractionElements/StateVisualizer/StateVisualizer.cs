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

        public StateTransitionManager StateTransitionManager { get; protected set; } = new StateTransitionManager();

        public void OnValidate()
        {
            if (StateVisualizerDefinition != null)
            {
                if (StateVisualizerDefinition.StateStyleConfigurationContainers.Count == 0)
                {
                    StateVisualizerDefinition.InitializeStateStyleContainers(TrackedStates, gameObject);

                    //AddStateStylePropertyToAState<MaterialStateStylePropertyConfiguration>("Focus");
                    //AddStateStylePropertyToAState<TransformOffsetStateStylePropertyConfiguration>("Focus");                   
                }
            }
        }

        public void SyncTrackedStatesWithStateDefinition()
        {
            StateVisualizerDefinition.UpdateStateStyleContainers(TrackedStates);
        }


        public void AddStateStylePropertyToAState<T>(string stateName) where T : StateStylePropertyConfiguration
        {
            // Find the container with the state
            StateStyleConfigurationContainer stateStyleContainer = StateVisualizerDefinition.StateStyleConfigurationContainers.Find((container) => (container.StateName == stateName));

            StateVisualizerDefinition.AddStateStyleProperty<T>(stateStyleContainer, stateName);
        }



        public void ActivateStateStyleProperty(string stateName)
        {
            // Change all properties with the StateName

            StateStyleConfigurationContainer stateStyleProperties = StateVisualizerDefinition.StateStyleConfigurationContainers.Find((x) => x.StateName == stateName);

            foreach (StateStylePropertyConfiguration stylePropertyConfig in stateStyleProperties.StateStyleProperties)
            {
                if (stylePropertyConfig.GetType() == typeof(MaterialStateStylePropertyConfiguration))
                {
                    SetStyleProperty<MaterialStateStylePropertyConfiguration>(stylePropertyConfig);
                }

                if (stylePropertyConfig.GetType() == typeof(TransformOffsetStateStylePropertyConfiguration))
                {
                    SetStyleProperty<TransformOffsetStateStylePropertyConfiguration>(stylePropertyConfig);
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
            if (StateVisualizerDefinition == null)
            {
                StateVisualizerDefinition = ScriptableObject.CreateInstance<StateVisualizerDefinition>();
            }

            StateTransitionManager.SaveDefaultStates(gameObject);

            // Add listeners to the OnStateActivated(InteractionState)
            // enable all the style properties 
            StateManager.OnStateActivated.AddListener(
                (state) =>
                {
                    if (state.Name == "Default" && state.Value == 1)
                    {
                        StateTransitionManager.SetDefaults(gameObject);
                    }
                    else
                    {
                        ActivateStateStyleProperty(state.Name);
                    }
                    
                });

            StateManager.OnStateDeactivated.AddListener(
                (stateTurnedOff, currentStateOn) =>
                {
                    Debug.Log("StateTurnedOff: " + stateTurnedOff.Name);
                    Debug.Log("CurrentStateOn: " + currentStateOn.Name);
                });
        }


        private void Update()
        {
           
        }

    }
}
