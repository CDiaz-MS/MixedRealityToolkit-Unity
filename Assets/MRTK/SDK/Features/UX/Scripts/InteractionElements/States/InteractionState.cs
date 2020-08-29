using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [System.Serializable]
    public class InteractionState
    {
        public InteractionState(string stateName)
        {
            Name = stateName;
        }

        [SerializeField]
        private string stateName;

        public string Name
        {
            get => stateName;
            internal set => stateName = value;
        }

        [SerializeField]
        private int stateValue = 0;

        public int Value
        {
            get => stateValue;
            internal set => stateValue = value; 
        }        
        
        [SerializeField]
        private int active = 0;

        public int Active
        {
            get => active;
            internal set => active = value; 
        }


        [SerializeField]
        private BaseInteractionEventConfiguration eventConfiguration = null;

        public BaseInteractionEventConfiguration EventConfiguration
        {
            get => eventConfiguration;
            internal set => eventConfiguration = value;
        }





    }
}
