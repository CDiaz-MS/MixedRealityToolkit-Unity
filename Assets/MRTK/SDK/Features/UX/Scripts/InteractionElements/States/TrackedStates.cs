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
        /// 
        /// </summary>
        public List<InteractionState> StateList
        {
            get { return stateList; }
            set { stateList = value; }
        }


        [SerializeField]
        private List<string> availableStates = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public List<string> AvailableStates
        {
            get { return availableStates; }
            set { availableStates = value;  }
        }

        public TrackedStates()
        {
            // Add default states to StateList
            StateList.Add(new InteractionState("Default"));
            StateList.Add(new InteractionState("Focus"));

            AvailableStates.Add("Default");
            AvailableStates.Add("Focus");
        }

    }
}
