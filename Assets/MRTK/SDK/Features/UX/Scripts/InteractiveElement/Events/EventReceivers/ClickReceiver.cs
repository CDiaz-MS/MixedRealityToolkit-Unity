// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The internal event receiver for the events defined in the Click Interaction Event Configuration
    /// </summary>
    internal class ClickReceiver : BaseEventReceiver
    {
        public ClickReceiver(ClickInteractionEventConfiguration clickEventConfiguration) : base(clickEventConfiguration, "ClickReceiver")
        {
            EventConfiguration = clickEventConfiguration;

            clickEventConfig = clickEventConfiguration;
        }

        private readonly ClickInteractionEventConfiguration clickEventConfig;

        private ClickInteractionEvent onClick => clickEventConfig.OnClick;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool click = stateManager.GetState(CoreInteractionState.Click).Value > 0;

            if (click)
            {
                Debug.Log("OnClick");

                onClick.Invoke(eventData as MixedRealityPointerEventData);
            }
        }

    }
}
