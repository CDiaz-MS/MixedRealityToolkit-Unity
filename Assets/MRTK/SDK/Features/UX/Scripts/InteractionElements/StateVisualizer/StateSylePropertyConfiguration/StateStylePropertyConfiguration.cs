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
        [SerializeField]
        private InteractionState state = null;

        public InteractionState State
        {
            get => state;
            set => state = value;
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
