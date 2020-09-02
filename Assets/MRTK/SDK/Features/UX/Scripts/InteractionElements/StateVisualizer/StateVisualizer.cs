// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [RequireComponent(typeof(InteractiveElement))]
    public class StateVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("")]
        private StateVisualizerDefinition stateVisualizerDefinition;

        /// <summary>
        /// The stateVisualizerDefinition is a scriptable object container for all the state style properties
        /// for each state that is tracked.
        /// </summary>
        public StateVisualizerDefinition StateVisualizerDefinition
        {
            get => stateVisualizerDefinition;
            set => stateVisualizerDefinition = value;
        }

        // The Interactive Element that contains the states being tracked
        private BaseInteractiveElement interactiveElement => GetComponent<BaseInteractiveElement>();

        // The states being tracked within an Interactive Element 
        private TrackedStates trackedStates => interactiveElement.TrackedStates;


        // The state manager within the Interactive Element
        private StateManager stateManager = null;

        /// <summary>
        /// Manages the transitions between states
        /// </summary>
        public StateTransitionManager StateTransitionManager { get; protected set; } = new StateTransitionManager();

        public void OnValidate()
        {
            if (StateVisualizerDefinition != null)
            {
                // Initialize the StateStyleContainers with the states in Tracked States
                if (StateVisualizerDefinition.StateContainers.Count == 0)
                {
                    Debug.Log("Initialize state definition via inspector");
                    
                    // If the user has not set an already defined tracked states object, then set it to tracked states in the BaseInteractiveElement
                    if (StateVisualizerDefinition.TrackedStates == null)
                    {
                        StateVisualizerDefinition.TrackedStates = trackedStates;
                    }
                    

                    // Set the default target object for the state definition
                    StateVisualizerDefinition.DefaultTarget = gameObject;

                    //UpdateStateVisualizerDefinitionStates();

                    //StateVisualizerDefinition.StateContainers.ForEach((x) => Debug.Log(x.name));

                    StateVisualizerDefinition.InitializeStateContainers(gameObject);

                    AddStateStylePropertyToAState(CoreStyleProperty.Material, "Focus");
                    AddStateStylePropertyToAState(CoreStyleProperty.TransformOffset, "Focus");                   
                }
            }
        }

        /// <summary>
        /// The states in the StateVisualizer definition mirror the states in an InteractionElement's Tracked States.
        /// If the states are updated, i.e. a new state is added or removed, the states in the State Visualizer need
        /// to be updated to match the Tracked States in the Interaction Element.
        /// </summary>
        public void UpdateStateVisualizerDefinitionStates()
        {
            StateVisualizerDefinition.UpdateStateStyleContainers();
        }

        public StateStylePropertyConfiguration AddStateStylePropertyToAState(CoreStyleProperty styleProperty, string stateName, GameObject target = null)
        {
            // If a state property is added during runtime, make sure the state containers match the tracked states
            // If the state containers and the tracked states are in sync, nothing will happen when this is called
            UpdateStateVisualizerDefinitionStates();

            // Find the state container for the state entered
            StateContainer stateStyleContainer = StateVisualizerDefinition.GetStateContainer(stateName);

            if (stateStyleContainer != null)
            {
                StateStylePropertyConfiguration newStyleProperty;

                if (target == null)
                {
                    newStyleProperty = stateStyleContainer.AddStateStyleProperty(styleProperty, gameObject);
                }
                else
                {
                    newStyleProperty = stateStyleContainer.AddStateStyleProperty(styleProperty, target);
                }

                return newStyleProperty;
            }
            else
            {
                return null;
            }

            
        }

        public StateStylePropertyConfiguration AddStateStylePropertyToAState(CoreStyleProperty styleProperty, CoreInteractionState state, GameObject target = null)
        {
            // If a state property is added during runtime, make sure the state containers match the tracked states
            // If the state containers and the tracked states are in sync, nothing will happen when this is called
            UpdateStateVisualizerDefinitionStates();

            // Find the state container for the state entered
            StateContainer stateStyleContainer = StateVisualizerDefinition.GetStateContainer(state.ToString());

            if (stateStyleContainer != null)
            {
                StateStylePropertyConfiguration newStyleProperty;

                if (target == null)
                {
                    newStyleProperty = stateStyleContainer.AddStateStyleProperty(styleProperty, gameObject);
                }
                else
                {
                    newStyleProperty = stateStyleContainer.AddStateStyleProperty(styleProperty, target);
                }
                
                return newStyleProperty;
            }
            else
            {
                return null;
            }
        }


        private void ActivateStateStyleProperty(string stateName)
        {
            StateContainer stateStyleProperties = StateVisualizerDefinition.StateContainers.Find((x) => x.StateName == stateName);

            foreach (StateStylePropertyConfiguration stylePropertyConfig in stateStyleProperties.StateStyleProperties)
            {
                // If the property is added during runtime, create an instance of the runtime class
                if (stylePropertyConfig.StateStyleProperty == null)
                {
                    stylePropertyConfig.CreateRuntimeInstance();
                }

                stylePropertyConfig.StateStyleProperty.SetStyleProperty();
            }
        }

        private void InitializeStyleProperties()
        { 
            foreach (StateContainer container in StateVisualizerDefinition.StateContainers)
            {
                foreach (StateStylePropertyConfiguration stylePropertyConfig in container.StateStyleProperties)
                {
                    if (stylePropertyConfig.StateStyleProperty == null)
                    {
                        stylePropertyConfig.CreateRuntimeInstance();
                    }
                }
            }
        }

        private void Start()
        {
            stateManager = interactiveElement.StateManager;

            StateTransitionManager.SaveDefaultStates(gameObject);

            InitializeStyleProperties();

            stateManager.OnStateActivated.AddListener(
                (state) =>
                {
                    StateTransitionManager.StateTransitionsQueue.Enqueue(state.Name);

                    if (state.Name == "Default" && state.Value == 1)
                    {
                        StateTransitionManager.SetDefaults(gameObject);
                    }
                    else
                    {
                        ActivateStateStyleProperty(state.Name);
                    }
                    
                });

            stateManager.OnStateDeactivated.AddListener(
                (stateTurnedOff, currentStateOn) =>
                {
                    //Debug.Log("StateTurnedOff: " + stateTurnedOff.Name);
                    //Debug.Log("CurrentStateOn: " + currentStateOn.Name);
                });
            
        }


        private void Awake()
        {
            InitializeStateVisualizerDefinition();
        }


        private void InitializeStateVisualizerDefinition()
        {
            if (StateVisualizerDefinition == null)
            {
                StateVisualizerDefinition = ScriptableObject.CreateInstance<StateVisualizerDefinition>();
                
                StateVisualizerDefinition.TrackedStates = trackedStates;

                // Set the default target object for the state definition
                StateVisualizerDefinition.DefaultTarget = gameObject;

                StateVisualizerDefinition.InitializeStateContainers(gameObject);
            }
        }
    }
}
