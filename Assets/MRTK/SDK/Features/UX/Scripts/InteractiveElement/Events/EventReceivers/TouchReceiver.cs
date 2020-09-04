// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The internal event receiver for the events defined in the Touch Interaction Event Configuration
    /// </summary>
    internal class TouchReceiver : BaseEventReceiver
    {
        public TouchReceiver(TouchInteractionEventConfiguration touchEventConfiguration) : base(touchEventConfiguration, "TouchReceiver")
        {
            EventConfiguration = touchEventConfiguration;

            touchEventConfig = touchEventConfiguration;
        }

        private readonly TouchInteractionEventConfiguration touchEventConfig;

        private TouchInteractionEvent onTouchStarted => touchEventConfig.OnTouchStarted;

        private TouchInteractionEvent onTouchCompleted => touchEventConfig.OnTouchCompleted;

        private TouchInteractionEvent onTouchUpdated => touchEventConfig.OnTouchUpdated;

        private bool hadTouch;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool hasTouch = stateManager.GetState(CoreInteractionState.Touch).Value > 0;

            if (hadTouch != hasTouch)
            {
                if (hasTouch)
                {
                    onTouchStarted.Invoke(eventData as HandTrackingInputEventData);
                }
                else
                {
                    onTouchCompleted.Invoke(eventData as HandTrackingInputEventData);
                }
            }
            else if (hasTouch)
            {
                onTouchUpdated.Invoke(eventData as HandTrackingInputEventData);
            }

            hadTouch = hasTouch;
        }
    }
}
