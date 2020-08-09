using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{

    public abstract class StateStylePropertyConfiguration: ScriptableObject
    {

        public State State;

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




    }
}
