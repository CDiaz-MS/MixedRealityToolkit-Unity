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

            Target = stateStylePropertyConfiguration.Target;
            StateName = stateStylePropertyConfiguration.StateName;
            StylePropertyName = stylePropertyName;

            // enforce type for the creation of a state style property
        }

        public StateStylePropertyConfiguration StateStylePropertyConfiguration { get; protected set; }

        protected GameObject Target;

        protected string StateName;

        protected string StylePropertyName;

        public abstract void SetStyleProperty();

    }
}
