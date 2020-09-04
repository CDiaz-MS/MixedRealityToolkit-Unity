// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The event configuration for the Touch state.
    /// </summary>
    [CreateAssetMenu(fileName = "TouchInteractionEventConfiguration", menuName = "Mixed Reality Toolkit/Interactive Element/Event Configurations/Touch Event Configuration")]
    public class TouchInteractionEventConfiguration : BaseInteractionEventConfiguration
    {
        public override string StateName => "Touch";

        /// <summary>
        /// A Unity event with HandTrackingInputEventData.  This event is fired when touch starts on an object.
        /// </summary>
        public TouchInteractionEvent OnTouchStarted = new TouchInteractionEvent();

        /// <summary>
        /// A Unity event with HandTrackingInputEventData.  This event is fired when touch starts on object.
        /// </summary>
        public TouchInteractionEvent OnTouchCompleted = new TouchInteractionEvent();

        /// <summary>
        /// A Unity event with HandTrackingInputEventData.  This event is fired when touch is updated on object.
        /// </summary>
        public TouchInteractionEvent OnTouchUpdated = new TouchInteractionEvent();

        ///<inheritdoc/>
        public override BaseEventReceiver InitializeRuntimeEventReceiver()
        {
            TouchReceiver touchReceiver = new TouchReceiver(this);

            EventReceiver = touchReceiver;

            return touchReceiver;
        }
    }
}
