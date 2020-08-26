// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [RequireComponent(typeof(BasicButton))]
    public class StateVisualizer : MonoBehaviour
    {
        [SerializeField]
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
        private StateManager stateManager;

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
                    // Set the default target object for the state definition
                    StateVisualizerDefinition.DefaultTarget = gameObject;

                    StateVisualizerDefinition.InitializeStateContainers(trackedStates, gameObject);

                    //AddStateStylePropertyToAState<MaterialStateStylePropertyConfiguration>("Focus");
                    //AddStateStylePropertyToAState<TransformOffsetStateStylePropertyConfiguration>("Focus");                   
                }
            }
        }

        /// <summary>
        /// The states in the StateVisualizer definition mirror the states in an InteractionElement's Tracked States.
        /// If the TrackedStates are updated, i.e. a new state is added or removed, the states in the State Visualizer need
        /// to be updated to match the Tracked States in the Interaction Element.
        /// </summary>
        public void UpdateStateVisualizerDefinitionStates()
        {
            StateVisualizerDefinition.UpdateStateStyleContainers(trackedStates);
        }


        public StateStylePropertyConfiguration AddStateStylePropertyToAState(CoreStyleProperty styleProperty, string stateName)
        {
            // Find the state container for the state entered
            StateContainer stateStyleContainer = StateVisualizerDefinition.StateContainers.Find((container) => (container.StateName == stateName));

            if (!stateStyleContainer.IsNull())
            {
                StateStylePropertyConfiguration newStyleProperty = stateStyleContainer.AddStateStyleProperty(styleProperty);

                return newStyleProperty;
            }
            else
            {
                Debug.LogError($"The {stateName} state does not have an existing state container for state style properties");
                return null;
            }
        }

        public StateStylePropertyConfiguration AddStateStylePropertyToAState(CoreStyleProperty styleProperty, CoreInteractionState state)
        {
            // Find the state container for the state entered
            StateContainer stateStyleContainer = StateVisualizerDefinition.StateContainers.Find((container) => (container.StateName == state.ToString()));

            if (!stateStyleContainer.IsNull())
            {
                StateStylePropertyConfiguration newStyleProperty = stateStyleContainer.AddStateStyleProperty(styleProperty);

                return newStyleProperty;
            }
            else
            {
                Debug.LogError($"The {state} state does not have an existing state container for state style properties");
                return null;
            }
        }


        public void ActivateStateStyleProperty(string stateName)
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
            if (StateVisualizerDefinition == null)
            {
                StateVisualizerDefinition = ScriptableObject.CreateInstance<StateVisualizerDefinition>();

                // Set the default target object for the state definition
                StateVisualizerDefinition.DefaultTarget = gameObject;

                StateVisualizerDefinition.InitializeStateContainers(trackedStates, gameObject);
            }

            stateManager = interactiveElement.StateManager;

            StateTransitionManager.SaveDefaultStates(gameObject);

            InitializeStyleProperties();

            stateManager.OnStateActivated.AddListener(
                (state) =>
                {
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
                    Debug.Log("StateTurnedOff: " + stateTurnedOff.Name);
                    Debug.Log("CurrentStateOn: " + currentStateOn.Name);
                });
            
        }


        private void Update()
        {
           
        }
    }
}
