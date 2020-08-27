// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The base class for state style property configurations.  A state style property configuration is 
    /// a data container for visual aspects of a game object such as the material or color. 
    /// </summary>
    public abstract class StateStylePropertyConfiguration: ScriptableObject
    {
        public virtual string StylePropertyName { get; } = null;

        [SerializeField,HideInInspector]
        private string stateName = null;

        /// <summary>
        /// The name of the interaction state associated with this state style property configuration.
        /// </summary>
        public string StateName
        {
            get => stateName;
            set => stateName = value;
        }

        [SerializeField]
        [Tooltip("The Target game object to receive all the state style property configurations.")]
        private GameObject target = null;

        /// <summary>
        /// The Target game object to receive the state style property values.
        /// </summary>
        public GameObject Target
        {
            get => target;
            set => target = value;
        }

        /// <summary>
        /// The runtime class for this state style property configuration 
        /// </summary>
        public StateStyleProperty StateStyleProperty = null;

        public abstract void CreateRuntimeInstance();
    }
}
