using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI
{
    [System.Serializable]
    public class StateStyleProperties
    {
        [SerializeField]
        public string StateName;

        [SerializeField]
        public GameObject Target;

        [SerializeField]
        public List<StateStylePropertyConfiguration> StateStylePropList;


        public StateStyleProperties(string stateName, GameObject target, List<StateStylePropertyConfiguration> stateStylePropList)
        {
            StateName = stateName;
            Target = target;
            StateStylePropList = stateStylePropList;
        }

    }
}