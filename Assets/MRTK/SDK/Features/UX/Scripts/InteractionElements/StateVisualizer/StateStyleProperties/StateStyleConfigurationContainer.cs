using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class StateStyleConfigurationContainer : ScriptableObject
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


        // Read only 
        [SerializeField]
        private InteractionState state = null;

        /// <summary>
        /// 
        /// </summary>
        public InteractionState State
        {
            get => state;
            set => state = value;
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
        private List<StateStylePropertyConfiguration> stateStyleProperties = new List<StateStylePropertyConfiguration>();

        /// <summary>
        /// 
        /// </summary>
        public List<StateStylePropertyConfiguration> StateStyleProperties
        {
            get => stateStyleProperties;
            set => stateStyleProperties = value;
        }


        // For the inspector 
        public StateStylePropertyConfiguration AddStateStyleProperty(Type propertyType)
        {
            StateStylePropertyConfiguration stateStylePropertyInstance = (StateStylePropertyConfiguration)CreateInstance(propertyType.Name.ToString());

            StateStyleProperties.Add(stateStylePropertyInstance);

            return stateStylePropertyInstance;
        }




    }
}