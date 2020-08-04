using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class StateVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("")]
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

        public List<State> ActiveStates => BaseInteractable.States.StateList;

        private StateManager StateManager => BaseInteractable.StateManager;


        public void OnValidate()
        {
            if (StateVisualizerDefinition.StateStyleProperties.Count == 0)
            {
                StateVisualizerDefinition.SetUp(BaseInteractable, this.gameObject);
            }
        }


        private void Update()
        {
            if(StateManager.GetState("Focus").Value == 1)
            {
                OnStateChange("Focus");
            }
        }

        public void OnStateChange(string StateName)
        {
            // Change all properties with the StateName

            StateStyleProperties stateStyleProperties = StateVisualizerDefinition.StateStyleProperties.Find((x) => x.StateName == StateName);

            foreach (StateStylePropertyConfiguration stylePropertyConfig in stateStyleProperties.StateStylePropList)
            {
                MaterialStateStylePropertyConfiguration config = stylePropertyConfig as MaterialStateStylePropertyConfiguration;

                config.MaterialStateStyleProperty.SetStyleProperty();
            }

        }
    }
}
