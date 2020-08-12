using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public abstract class StateStylePropertyConfiguration: ScriptableObject
    {
        [HideInInspector]
        public string StateStylePropertyName;

        [HideInInspector]
        public InteractionState State;

        //public MaterialStateStylePropertyConfiguration materialConfig;

        [SerializeField]
        private GameObject target;

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
