// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    public abstract class StateStylePropertyConfiguration: ScriptableObject
    {
        public virtual string StylePropertyName { get; } = null;

        [SerializeField,HideInInspector]
        private string stateName = null;

        public string StateName
        {
            get => stateName;
            set => stateName = value;
        }

        [SerializeField]
        private GameObject target = null;

        public GameObject Target
        {
            get => target;
            set => target = value;
        }

        public StateStyleProperty StateStyleProperty = null;

        public abstract StateStyleProperty CreateRuntimeInstance();
    }
}
