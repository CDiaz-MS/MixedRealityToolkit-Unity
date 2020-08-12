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
        public StateStyleProperty(StateStylePropertyConfiguration stateStylePropertyConfiguration)
        {
            //StateStylePropertyConfiguration = stateStylePropertyConfiguration;
            State = stateStylePropertyConfiguration.State;
            Target = stateStylePropertyConfiguration.Target;
            // enforce type for the creation of a state style property
        }

        protected StateStylePropertyConfiguration StateStylePropertyConfiguration;

        protected GameObject Target;

        protected InteractionState State;

        public string StateName => StateStylePropertyConfiguration.State.Name;

        public abstract void SetStyleProperty();

    }
}
