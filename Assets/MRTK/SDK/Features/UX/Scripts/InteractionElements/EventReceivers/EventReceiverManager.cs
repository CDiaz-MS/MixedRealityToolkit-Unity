using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;


namespace Microsoft.MixedReality.Toolkit.UI
{
    public class EventReceiverManager
    {
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


        [SerializeField]
        private List<BaseInteractableEvent> events = new List<BaseInteractableEvent>();

        /// <summary>
        /// List of available states defined by asset
        /// </summary>
        public List<BaseInteractableEvent> Events
        {
            get { return events; }
            set { events = value; }
        }


        public void Invoke(string name, int value, BaseEventData eventData, StateManager stateManager, BaseInteractable baseInteractable)
        {
            State state = stateManager.GetState(name);

            EventReceiverList[0].OnUpdate(stateManager, baseInteractable, eventData);

            // Only call the onupdates if it has been called from interactable

            Debug.Log(EventReceiverList[0].Name + "!!!!!!!!!!!!!!!!!!!!");

        }


        // An event receiver can only be added if the state is also in there
    }
}
