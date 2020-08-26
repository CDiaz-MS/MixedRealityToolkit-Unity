// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Class for managing the events with InteractionStates.
    /// </summary>
    public class EventReceiverManager
    {
        public EventReceiverManager(StateManager stateManager)
        {
            StateManager = stateManager;
        }

        public StateManager StateManager { get; protected set; }

        public List<BaseEventReceiver> EventReceiverList { get; protected set; } = new List<BaseEventReceiver>();

        private Dictionary<BaseInteractionEventConfiguration, BaseEventReceiver> stateEvents = new Dictionary<BaseInteractionEventConfiguration, BaseEventReceiver>();

        public void InvokeStateEvent(BaseEventData eventData)
        {
            foreach (BaseEventReceiver receiver in EventReceiverList)
            {
                if (!receiver.IsNull())
                {
                    receiver.OnUpdate(StateManager, eventData);
                }               
            }
        }

        public BaseEventReceiver InitializeEventReceiver(string stateName)
        {
            if (IsEventConfigurationValid(stateName))
            {
                BaseEventReceiver receiver = StateManager.GetState(stateName).EventConfiguration.InitializeRuntimeEventReceiver();
                
                return receiver;
            }

            return null;
            
        }

        public T GetEventConfiguration<T>(string stateName) where T : BaseInteractionEventConfiguration
        {
            T receiver = StateManager.GetState(stateName).EventConfiguration as T;

            return receiver;
        }

        public BaseInteractionEventConfiguration GetEventConfiguration(string stateName)
        {
            // find the event receiver that has the state name in it and return the configuration
            BaseEventReceiver eventReceiver = EventReceiverList.Find((receiver) => receiver.Name.StartsWith(stateName));

            if (eventReceiver == null)
            {
                Debug.LogError($"An event configuration for the {stateName} state does not exist");
            }

            return eventReceiver.EventConfiguration;

        }

        // Add method for creating instances of the scriptable object based on the name 

        public BaseInteractionEventConfiguration SetEventConfiguration(string stateName)
        {
           if (IsEventConfigurationValid(stateName))
           {
                var eventConfiguration = (BaseInteractionEventConfiguration)ScriptableObject.CreateInstance(stateName + "InteractionEventConfiguration");
                return eventConfiguration;
           }
            else
            {
                //Debug.LogError($"An event configuration for the {stateName} state does not exist");
                return null;
            }
        }


        private bool IsEventConfigurationValid(string stateName)
        {
            var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
            var eventConfigType = eventConfigurationTypes.Find(t => t.Name.StartsWith(stateName));

            if (eventConfigType.IsNull())
            {
                return false;
            }

            return true;
        }

        // Add method for non-manual updates for events
    }
}
