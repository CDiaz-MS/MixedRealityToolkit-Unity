using Microsoft.MixedReality.Toolkit.UI;
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


        private void Update()
        {

            //Debug.Log(StateManager.CheckStateChange());
            InteractionState state = StateManager.CheckStateChange();

            if (state != null)
            {
                Debug.Log(state.Name);

                ActivateStateStyleProperty(state.Name);
                
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
                    MaterialStateStylePropertyConfiguration config = stylePropertyConfig as MaterialStateStylePropertyConfiguration;

                    config.CreateRuntimeInstance();

                    config.MaterialStateStyleProperty.SetStyleProperty();

                }
            }
        }
    }
}
