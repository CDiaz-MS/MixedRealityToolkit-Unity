using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class TrackedStates : ScriptableObject
    {
        [SerializeField]
        private List<InteractionState> states = new List<InteractionState>(); 

        /// <summary>
        /// 
        /// </summary>
        public List<InteractionState> States
        {
            get { return states; }
            set { states = value; }
        }

        public TrackedStates()
        {
            // Add default states to StateList
            States.Add(new InteractionState("Default"));
            States.Add(new InteractionState("Focus"));
        }


        public bool IsStateTracked(string stateName)
        {
            if (States.Exists((state) => state.Name == stateName))
            {
                Debug.LogError($"The {stateName} state is already being tracked and cannot be added to TrackedStates.");
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
