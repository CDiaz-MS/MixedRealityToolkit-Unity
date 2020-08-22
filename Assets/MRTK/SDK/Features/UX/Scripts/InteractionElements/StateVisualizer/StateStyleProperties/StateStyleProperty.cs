using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Runtime class for state style properties 
    /// </summary>
    public abstract class StateStyleProperty
    {
        public StateStyleProperty(StateStylePropertyConfiguration stateStylePropertyConfiguration, string stylePropertyName)
        {
            StateStylePropertyConfiguration = stateStylePropertyConfiguration;

            StateName = stateStylePropertyConfiguration.StateName;
            StylePropertyName = stylePropertyName;

            Target = stateStylePropertyConfiguration.Target;

            if (Target.IsNull())
            {
                Debug.LogError($"The target object for the {StylePropertyName} style property within the {StateName} state in the StateVisualizer has not been set.");
            }
        }

        public StateStylePropertyConfiguration StateStylePropertyConfiguration { get; protected set; }

        protected GameObject Target;

        protected string StateName;

        protected string StylePropertyName;

        public abstract void SetStyleProperty();

    }
}
