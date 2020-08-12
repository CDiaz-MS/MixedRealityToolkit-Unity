using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class TrackedStates : ScriptableObject
    {
        [SerializeField]
        private List<InteractionState> stateList = new List<InteractionState>(); 

        /// <summary>
        /// List of available states defined by asset
        /// </summary>
        public List<InteractionState> StateList
        {
            get { return stateList; }
            set { stateList = value; }
        }

        public TrackedStates()
        {
            // Add default states to StateList
            StateList.Add(new InteractionState("Default"));
            StateList.Add(new InteractionState("Focus"));
        }

    }
}
