using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public abstract class StateStylePropertyConfiguration: ScriptableObject
    {
        public string StylePropertyName { get; protected set; }

        [SerializeField]
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
            get 
            {
                return target; 
            }
            set
            {
                if (target != value)
                {
                    target = value;
                }
            }
        }

        public StateStyleProperty StateStyleProperty;

        public abstract StateStyleProperty CreateRuntimeInstance();
    }
}
