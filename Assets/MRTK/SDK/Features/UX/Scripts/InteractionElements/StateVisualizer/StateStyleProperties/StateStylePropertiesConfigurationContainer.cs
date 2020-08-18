using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class StateStylePropertiesConfigurationContainer : ScriptableObject
    {
        [SerializeField]
        private string stateName = "";

        /// <summary>
        /// 
        /// </summary>
        public string StateName
        {
            get => stateName;
            set => stateName = value;
        }

        [SerializeField]
        private GameObject target = null;

        /// <summary>
        /// 
        /// </summary>
        public GameObject Target
        {
            get => target;
            set => target = value;
        }

        [SerializeField]
        private List<StateStylePropertyConfiguration> stateStylePropList = new List<StateStylePropertyConfiguration>();


        public List<StateStylePropertyConfiguration> StateStylePropList
        {
            get => stateStylePropList;
            set => stateStylePropList = value;
        }

    }
}