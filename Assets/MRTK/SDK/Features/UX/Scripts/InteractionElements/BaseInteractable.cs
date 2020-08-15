using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    public abstract class BaseInteractable :
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
        private TrackedStates trackedStates;

        /// <summary>
        /// ScriptableObject to reference for basic state logic to follow when interacting and transitioning between states. Should generally be "DefaultInteractableStates" object
        /// </summary>
        public TrackedStates TrackedStates
        {
            get => trackedStates;
            set
            {
                trackedStates = value;
            }
        }

        // The StateManager stores all the values for the states
        public StateManager StateManager { get; protected set; }

        public EventReceiverManager EventReceiverManager { get; protected set; }


        public virtual void Start()
        {
            InitializeStateManager();

            InitializeEventReceiverManager();
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitializeStateManager()
        {
            StateManager = new StateManager
            {
                // Add the states defined in the scriptable object
                TrackedStates = TrackedStates.StateList
            };

            // Set the defalut state on start
            StateManager.SetState("Default", 1);
            StateManager.SetState("Focus", 0);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeEventReceiverManager()
        {
            EventReceiverManager = new EventReceiverManager(StateManager);

            foreach (InteractionState state in TrackedStates.StateList)
            {
                // Make sure the default state does not get event receivers
                if (state.EventConfiguration != null)
                {
                    BaseEventReceiver receiver = EventReceiverManager.InitializeEventReceiver(state.Name);

                    EventReceiverManager.EventReceiverList.Add(receiver);
                }

            } 
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
            StateManager.SetState("Focus", 1);

            // This is for updating manually
            EventReceiverManager.InvokeStateEvent(this, eventData);

            StateManager.SetState("Default", 0);
        }


        public void OnFocusExit(FocusEventData eventData)
        {
            StateManager.SetState("Focus", 0);

            EventReceiverManager.InvokeStateEvent(this, eventData);

            StateManager.SetState("Default", 1);

        }

        public void OnFocusChanged(FocusEventData eventData){ }

        #endregion

    }
}