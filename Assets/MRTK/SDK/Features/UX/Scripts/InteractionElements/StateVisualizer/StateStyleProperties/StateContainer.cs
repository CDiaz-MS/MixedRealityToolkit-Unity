// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class StateContainer : ScriptableObject
    {
        
        [SerializeField]
        private string stateName = null;

        /// <summary>
        /// The name of the state this container is tracking 
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
        /// The InteractionState associated with this container
        /// </summary>
        public InteractionState State
        {
            get => state;
            set => state = value;
        }

        [SerializeField]
        private List<StateStylePropertyConfiguration> stateStyleProperties = new List<StateStylePropertyConfiguration>();

        /// <summary>
        /// 
        /// </summary>
        public List<StateStylePropertyConfiguration> StateStyleProperties
        {
            get => stateStyleProperties;
            protected set => stateStyleProperties = value;
        }

        public GameObject DefaultTarget { get; set; }

        private string[] coreStyleProperties = Enum.GetNames(typeof(CoreStyleProperty)).ToArray();

        // For the inspector 
        public StateStylePropertyConfiguration AddStateStyleProperty(CoreStyleProperty styleProperty)
        {

            StateStylePropertyConfiguration stateStylePropertyInstance = (StateStylePropertyConfiguration)CreateInstance(styleProperty + "StateStylePropertyConfiguration");
            stateStylePropertyInstance.StateName = StateName;

            if (!DefaultTarget.IsNull())
            {
                stateStylePropertyInstance.Target = DefaultTarget;
            }
            


            StateStyleProperties.Add(stateStylePropertyInstance);

            return stateStylePropertyInstance;
           
        }


        public StateStylePropertyConfiguration AddStateStyleProperty(string styleProperty)
        {
            if (coreStyleProperties.Contains(styleProperty))
            {
                StateStylePropertyConfiguration stateStylePropertyInstance = (StateStylePropertyConfiguration)CreateInstance(styleProperty + "StateStylePropertyConfiguration");
                stateStylePropertyInstance.StateName = StateName;
                stateStylePropertyInstance.name = styleProperty + StateName;

                if (!DefaultTarget.IsNull())
                {
                    stateStylePropertyInstance.Target = DefaultTarget;
                }

                StateStyleProperties.Add(stateStylePropertyInstance);

                return stateStylePropertyInstance;
            }
            else
            {
                Debug.LogError($"The {styleProperty} style property could not be added becasue a configuration does not exist for the property.");
                return null;
            }
        }
    }
}