// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The internal event receiver for the events defined in the Toggle Interaction Event Configuration
    /// </summary>
    internal class ToggleReceiver : BaseEventReceiver
    {
        public ToggleReceiver(ToggleInteractionEventConfiguration toggleEventConfiguration) : base(toggleEventConfiguration, "ToggleReceiver")
        {
            EventConfiguration = toggleEventConfiguration;

            toggleEventConfig = toggleEventConfiguration;
        }

        private readonly ToggleInteractionEventConfiguration toggleEventConfig;

        private ClickInteractionEvent onToggleSelect => toggleEventConfig.OnToggleSelect;
        private ClickInteractionEvent onToggleDeselect => toggleEventConfig.OnToggleDeselect;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool isToggleOn = stateManager.GetState(CoreInteractionState.Toggle).Value > 0;

            if (isToggleOn)
            {
                Debug.Log("Toggle On");

                onToggleSelect.Invoke(eventData as MixedRealityPointerEventData);
            }
            else
            {
                Debug.Log("Toggle Off");
                onToggleDeselect.Invoke(eventData as MixedRealityPointerEventData);
            }
        }
    }
}
