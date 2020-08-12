
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class StateVisualizerDefinition : ScriptableObject
    {
        // All the game objects contained in the definition 

        public GameObject Target;


        public List<StateStylePropertiesConfigurationContainer> StateStyleProperties;


        //private Dictionary<GameObject, List<StateStylePropertiesConfigurationContainer>> gameObjectStateStylePropertyMap = new Dictionary<GameObject, List<StateStylePropertiesConfigurationContainer>>();


        public void SetUp(BaseInteractable baseInteractable, GameObject target)
        {

            foreach(InteractionState state in baseInteractable.TrackedStates.StateList)
            {
                StateStylePropertiesConfigurationContainer stateStyleProps = CreateInstance<StateStylePropertiesConfigurationContainer>();
                stateStyleProps.name = state.Name;
                stateStyleProps.StateName = state.Name;
                stateStyleProps.Target = target;
                stateStyleProps.StateStylePropList = new List<StateStylePropertyConfiguration>();
                stateStyleProps.StateStylePropList.Add(CreateInstance<MaterialStateStylePropertyConfiguration>());
                
                foreach (StateStylePropertyConfiguration configuration in stateStyleProps.StateStylePropList)
                {
                    if (configuration.GetType() == typeof(MaterialStateStylePropertyConfiguration))
                    {
                        MaterialStateStylePropertyConfiguration materialConfig = configuration as MaterialStateStylePropertyConfiguration;
                        materialConfig.name = "Material";
                        materialConfig.StateStylePropertyName = "Material";
                        materialConfig.Target = target;
                        materialConfig.State = state;
                    }
                }

                StateStyleProperties.Add(stateStyleProps);
            }
        }
    }
}
