// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu(fileName = "states", menuName = "Mixed Reality Toolkit/Interactive Element/Tracked States")]
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
            // Add default states to the States list 
            States.Add(new InteractionState("Default"));
            States.Add(new InteractionState("Focus"));
        }


        public bool IsStateTracked(string stateName)
        {
            if (States.Exists((state) => state.Name == stateName))
            {
                Debug.LogError($"The {stateName} state is already being tracked and cannot be added to states.");
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
