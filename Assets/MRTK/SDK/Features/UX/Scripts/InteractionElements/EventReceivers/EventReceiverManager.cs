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

        public StateManager StateManager;


        [SerializeField]
        private List<BaseEventReceiver> eventReceiverList = new List<BaseEventReceiver>();

        /// <summary>
        /// List of available states defined by asset
        /// </summary>
        public List<BaseEventReceiver> EventReceiverList
        {
            get { return eventReceiverList; }
            set { eventReceiverList = value; }
        }

        public void InvokeStateEvent(BaseInteractable baseInteractable, BaseEventData eventData)
        {
            foreach (BaseEventReceiver receiver in EventReceiverList)
            {
                receiver.OnUpdate(StateManager, baseInteractable, eventData);

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



        public BaseInteractionEventConfiguration GetStateEvents(string stateName)
        {
            // find the event receiver that has the state name in it and return the configuration
            BaseEventReceiver eventReceiver = eventReceiverList.Find((receiver) => receiver.Name.Contains(stateName));

            return eventReceiver.baseEventConfiguration;

        }



        // Add method for non-manual updates for events
    }
}
