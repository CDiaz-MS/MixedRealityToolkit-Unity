using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class BaseInteractable :
        MonoBehaviour,
        IMixedRealityFocusChangedHandler,
        IMixedRealityFocusHandler
    {

        /// <summary>
        /// Pointers that are focusing the interactable
        /// </summary>
        public List<IMixedRealityPointer> FocusingPointers => focusingPointers;
        protected readonly List<IMixedRealityPointer> focusingPointers = new List<IMixedRealityPointer>();

        [SerializeField]
        [Tooltip("ScriptableObject to reference for basic state logic to follow when interacting and transitioning between states. Should generally be \"DefaultInteractableStates\" object")]
        private ActiveStates states;

        /// <summary>
        /// ScriptableObject to reference for basic state logic to follow when interacting and transitioning between states. Should generally be "DefaultInteractableStates" object
        /// </summary>
        public ActiveStates States
        {
            get => states;
            set
            {
                states = value;
            }
        }

        // The StateManager stores all the values for the states
        public StateManager StateManager = new StateManager();

        // The StateManager stores all the values for the states
        public EventReceiverManager EventReceiverManager = new EventReceiverManager();


        [SerializeField]
        private List<BaseInteractableEvent> events = new List<BaseInteractableEvent>();

        /// <summary>
        /// ScriptableObject to reference for basic state logic to follow when interacting and transitioning between states. Should generally be "DefaultInteractableStates" object
        /// </summary>
        public List<BaseInteractableEvent> Events
        {
            get => events;
            set
            {
                events = value;
            }
        }

        
        public FocusUnityEvent focusEvent = new FocusUnityEvent();


        public virtual void Start()
        {
            //StateManager = new StateManager(activeStates);
            InitializeStateManager();
            


            // Set the defalut state on start
            StateManager.SetState("Default", 1);
        }


        /// <summary>
        /// Take the states defined in the scriptable object and add them to the active states
        /// list.
        /// </summary>
        private void InitializeStateManager()
        {
            StateManager.ActiveStates = States.StateList;

            EventReceiverManager.EventReceiverList.Add(new FocusReceiver());


        }


        private void OnValidate()
        {
            if (Events.Count != 0)
            {


                foreach (BaseEventReceiver receiver in EventReceiverManager.EventReceiverList)
                {
                    //receiver.GetType
                    //EventReceiverManager.Events.Add(receiver.Event);
                }

                Events = EventReceiverManager.Events;
            }
        }


        private void Update()
        {
            // Update the active states in the StateManager if Active States length has changed

            foreach (BaseEventReceiver receiver in EventReceiverManager.EventReceiverList)
            {
                // Add the events in each receiver to the event list 
            }


            // If no other states are active, then we are in the Default state
        }


        private void UpdateStates()
        {
            
        }



        #region Focus

        public void OnBeforeFocusChange(FocusEventData eventData)
        {
            // If we went from focus on an object to not focusing on an object, remove it from the list
            if (eventData.NewFocusedObject == null)
            {
                focusingPointers.Remove(eventData.Pointer); 
            }
            // If the old focused object is a child of this gameobject
            else if (eventData.OldFocusedObject != null
                && eventData.OldFocusedObject.transform.IsChildOf(gameObject.transform))
            {
                focusingPointers.Remove(eventData.Pointer);
            }
            // If we go from no focus to a child of an interactable then add it
            else if (eventData.NewFocusedObject.transform.IsChildOf(gameObject.transform))
            {
                // Technically this gameobject is a child of itself, so this covers both adding
                // a focused pointer if the pointer is focused on this gameobject or a child 
                if (!focusingPointers.Contains(eventData.Pointer))
                {
                    focusingPointers.Add(eventData.Pointer);
                }
            }
        }

        public void OnFocusEnter(FocusEventData eventData)
        {
            // Change focus state
            // Add event receiver 
            onFocusEnter(eventData);
            focusEvent.Invoke(eventData);
        }


        public virtual void onFocusEnter(FocusEventData eventData)
        {
            StateManager.SetState("Focus", 1);

            Debug.Log(gameObject.name + " Focus Enter" + StateManager.GetState("Focus").Value);

            EventReceiverManager.Invoke("Focus", 1, eventData, StateManager, this);
            


            StateManager.SetState("Default", 0);
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            // Change focus state
            // Add event receiver 
            StateManager.SetState("Focus", 0);
            Debug.Log(gameObject.name + " Focus Exit" + StateManager.GetState("Focus").Value);
        }

        public void OnFocusChanged(FocusEventData eventData){ }

        #endregion

    }
}