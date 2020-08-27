// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Runtime class for state style properties. 
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

        protected StateStylePropertyConfiguration StateStylePropertyConfiguration;

        protected GameObject Target;

        protected string StateName;

        protected string StylePropertyName;

        /// <summary>
        /// This method is called when an interaction state is set to on.
        /// </summary>
        public abstract void SetStyleProperty();

    }
}
