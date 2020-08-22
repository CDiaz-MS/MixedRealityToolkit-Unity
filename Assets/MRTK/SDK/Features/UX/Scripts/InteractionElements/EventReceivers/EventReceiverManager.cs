using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
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
                receiver.OnUpdate(StateManager, eventData);

            }
        }

        public BaseEventReceiver InitializeEventReceiver(string stateName)
        {
            BaseEventReceiver receiver = StateManager.GetState(stateName).EventConfiguration.CreateRuntimeInstance();

            return receiver;
        }



        public T GetEventConfiguration<T>(string stateName) where T : BaseInteractionEventConfiguration
        {
            T receiver = StateManager.GetState(stateName).EventConfiguration as T;

            return receiver;
        }



        public BaseInteractionEventConfiguration GetEventConfiguration(string stateName)
        {
            // find the event receiver that has the state name in it and return the configuration
            BaseEventReceiver eventReceiver = EventReceiverList.Find((receiver) => receiver.Name.Contains(stateName));

            if (eventReceiver == null)
            {
                Debug.LogError($"An event configuration for the {stateName} state does not exist");
            }

            return eventReceiver.EventConfiguration;

        }

        // Add method for creating instances of the scriptable object based on the name 

        public BaseInteractionEventConfiguration SetEventConfiguration(string stateName)
        {
            var eventConfiguration = (BaseInteractionEventConfiguration)ScriptableObject.CreateInstance(stateName + "InteractionEventConfiguration");
            
            if (eventConfiguration == null)
            {
                Debug.LogError($"An event configuration for the {stateName} state does not exist");
            }
            
            return eventConfiguration;
        }



        // Add method for non-manual updates for events
    }
}
