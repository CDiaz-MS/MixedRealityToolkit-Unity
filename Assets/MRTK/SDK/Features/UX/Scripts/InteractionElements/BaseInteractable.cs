using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        /// <summary>
        /// Entry point for state management 
        /// </summary>
        public StateManager StateManager { get; protected set; }


        public readonly string[] availableStates = new string[] { "Default", "Focus" };


        /// <summary>
        /// Entry point for event management 
        /// </summary>
        public EventReceiverManager EventReceiverManager { get; protected set; }


        // Initialize in Awake because the States Visualizer depends on the initialization of these elements
        private void Awake()
        {
            InitializeStateManager();

            InitializeEventReceiverManager();

            SetStateOn("Default");
        }


        public virtual void Start()
        {

        }

        /// <summary>
        /// Initializes the StateList in the StateManager with the states defined in the TrackedStates scriptable object.
        /// </summary>
        private void InitializeStateManager()
        {
            StateManager = new StateManager
            {
                // Add the states defined in the scriptable object
                TrackedStates = TrackedStates.StateList
            };
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
            if (CheckStateTrackingStatus("Focus"))
            {
                SetStateOn("Focus");

                // This is for updating manually
                EventReceiverManager.InvokeStateEvent(this, eventData);

                StateManager.SetState("Default", 0);

            }
        }


        public void OnFocusExit(FocusEventData eventData)
        {
            if (CheckStateTrackingStatus("Focus"))
            {
                SetStateOff("Focus");

                EventReceiverManager.InvokeStateEvent(this, eventData);

                StateManager.SetState("Default", 1);
            }
        }

        public void OnFocusChanged(FocusEventData eventData) { }

        #endregion



        #region State Setting Utilities

        public void SetStateOn(string stateName)
        {
            StateManager.SetState(stateName, 1);
        }

        public void SetStateOff(string stateName)
        {
            StateManager.SetState(stateName, 0);
        }

        public void ResetAllStates()
        {
            foreach (InteractionState state in StateManager.TrackedStates)
            {
                StateManager.SetState(state.Name, 0);
            }
        }

        public bool CheckStateTrackingStatus(string stateName)
        {
           if (StateManager.GetState(stateName) == null)
           {
                return false;
           }
           
           return true;
        }

        public void AddNewState(string stateName)
        {
            if (availableStates.Contains(stateName))
            {
                InteractionState newState = new InteractionState(stateName);

                var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
                var eventConfigType = eventConfigurationTypes.Find(t => t.Name.Contains(stateName));

                // Check if the state has a custom event configuration
                if (eventConfigType != null)
                {
                    string className = eventConfigType.Name;

                    newState.EventConfiguration = (BaseInteractionEventConfiguration)ScriptableObject.CreateInstance(className);
                    newState.EventConfiguration.Name = stateName + "EventConfiguration";   
                }

                StateManager.TrackedStates.Add(newState);
            }
        }

        public void CreateNewState(string stateName)
        {
            if (!availableStates.Contains(stateName))
            {
                InteractionState newState = new InteractionState(stateName);
                StateManager.TrackedStates.Add(newState);
            }
            else
            {
                Debug.Log("The state name " + stateName + " is a defined core state, please use AddNewState(" + stateName + ") to add to Tracked States.");
            }
        }

        public void CreateNewState(string stateName, BaseInteractionEventConfiguration eventConfiguration)
        {
            if (!availableStates.Contains(stateName))
            {
                InteractionState newState = new InteractionState(stateName);
                newState.EventConfiguration = eventConfiguration;
                StateManager.TrackedStates.Add(newState);
            }
            else
            {
                Debug.Log("The state name " + stateName + " is a defined core state, please use AddNewState(" + stateName + ") to add to Tracked States.");
            }
        }

        public bool IsInDefaultState()
        {
            foreach (InteractionState state in StateManager.TrackedStates)
            {
                int value = StateManager.GetState(state.Name).Value;

                if ((value > 0) && (state.Name != "Default"))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion


        private void Update()
        {
            // If any of the other states are active, set the default state

        }



    }
}