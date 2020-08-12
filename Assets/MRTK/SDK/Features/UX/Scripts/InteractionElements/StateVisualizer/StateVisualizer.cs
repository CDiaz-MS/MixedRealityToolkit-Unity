using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
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

        public List<InteractionState> TrackedStates => BaseInteractable.TrackedStates.StateList;

        private StateManager StateManager => BaseInteractable.StateManager;


        public void OnValidate()
        {
            if (StateVisualizerDefinition.StateStyleProperties.Count == 0)
            {
                StateVisualizerDefinition.StateStyleProperties = new List<StateStylePropertiesConfigurationContainer>();
                StateVisualizerDefinition.SetUp(BaseInteractable, this.gameObject);
            }
        }

        public void ActivateStateStyleProperty(string StateName)
        {
            // Change all properties with the StateName

            StateStylePropertiesConfigurationContainer stateStyleProperties = StateVisualizerDefinition.StateStyleProperties.Find((x) => x.StateName == StateName);

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
