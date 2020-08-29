﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    public abstract class BaseInteractiveElement :
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
            set => trackedStates = value;
        }

        /// <summary>
        /// Entry point for state management 
        /// </summary>
        public StateManager StateManager { get; protected set; }

        /// <summary>
        /// Entry point for event management 
        /// </summary>
        public EventReceiverManager EventReceiverManager { get; protected set; }


        public bool IsInDefaultState
        {
            get
            {
                int defaultStateValue = GetState(CoreInteractionState.Default).Value;

                if (defaultStateValue == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }


        // Initialize the State Manager and the Event Manager in Awake because 
        // the States Visualizer depends on the initialization of these elements
        private void Awake()
        {
            InitializeStateManager();

            InitializeEventReceiverManager();

            SetStateOn(CoreInteractionState.Default);
        }

        /// <summary>
        /// Initializes the StateList in the StateManager with the states defined in the states scriptable object.
        /// </summary>
        private void InitializeStateManager()
        {
            // Create an instance of the Tracked States scriptable object if this class is initialized via code
            // instead of the inspector 
            if (TrackedStates == null)
            {
                TrackedStates = ScriptableObject.CreateInstance<TrackedStates>();
            }

            StateManager = new StateManager(TrackedStates);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeEventReceiverManager()
        {
            EventReceiverManager = new EventReceiverManager(StateManager);

            foreach (InteractionState state in TrackedStates.States)
            {
                // The Defalut state does not have an event configuration 
                if (state.Name != "Default")
                {
                    // Initialize Event Configurations for each state if the configuration exists
                    // This case if for if a component is created via script instead of initialized in the inspector
                    if (state.EventConfiguration == null )
                    {
                        state.EventConfiguration = EventReceiverManager.SetEventConfiguration(state.Name);
                    }

                    // Initialize runtime event receiver classes for states that have an event configuration 
                    BaseEventReceiver eventReceiver = EventReceiverManager.InitializeEventReceiver(state.Name);

                    EventReceiverManager.EventReceiverList.Add(eventReceiver);
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
            if (IsStateTracking("Focus"))
            {
                SetStateOn(CoreInteractionState.Focus);

                // This is for updating manually
                EventReceiverManager.InvokeStateEvent(eventData);

                //SetStateOff(CoreInteractionState.Default);
            }
        }


        public void OnFocusExit(FocusEventData eventData)
        {
            if (IsStateTracking("Focus"))
            {
                // ORDER MATTERS, make it not matter
                //SetStateOn(CoreInteractionState.Default);

                SetStateOff(CoreInteractionState.Focus);

                EventReceiverManager.InvokeStateEvent(eventData);

                
            }
        }

        public void OnFocusChanged(FocusEventData eventData) { }

        #endregion



        #region State Setting Utilities

        public void SetStateOn(string stateName)
        {
            StateManager.SetStateOn(stateName);
        }

        public void SetStateOn(CoreInteractionState coreState)
        {
            StateManager.SetStateOn(coreState);
        }

        public void SetStateOff(string stateName)
        {
            StateManager.SetStateOff(stateName);
        }        
        
        public void SetStateOff(CoreInteractionState coreState)
        {
            StateManager.SetStateOff(coreState);
        }
        #endregion

        #region State Getting Utilities

        public InteractionState GetState(string stateName)
        {
            return StateManager.GetState(stateName);
        }

        public InteractionState GetState(CoreInteractionState coreState)
        {
            return StateManager.GetState(coreState);
        }


        #endregion


        public void ResetAllStates()
        {
            foreach (InteractionState state in StateManager.States)
            {
                StateManager.SetState(state.Name, 0);
            }
        }

        public bool IsStateTracking(string stateName)
        {
           if (StateManager.GetState(stateName) == null)
           {
                return false;
           }
           
           return true;
        }

        public InteractionState AddCoreState(CoreInteractionState state)
        {
            return StateManager.AddCoreState(state);
        }

        public InteractionState AddNewState(string stateName)
        {
            return StateManager.AddNewState(stateName);
        }

        public void RemoveCoreState(CoreInteractionState state)
        {
            StateManager.RemoveCoreState(state);
        }

        public void RemoveState(string stateName)
        {
            StateManager.RemoveState(stateName);
        }

        public void AddNewStateWithEventConfiguration(string stateName, BaseInteractionEventConfiguration eventConfiguration)
        {
            StateManager.AddNewStateWithEventConfiguration(stateName, eventConfiguration);
        }

        private void Update()
        {


        }

    }
}