// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The event configuration for the Click state.
    /// </summary>
    [CreateAssetMenu(fileName = "ClickInteractionEventConfiguration", menuName = "Mixed Reality Toolkit/Interactive Element/Event Configurations/Click Event Configuration")]
    public class ClickInteractionEventConfiguration : BaseInteractionEventConfiguration
    {
        public override string StateName => "Click";

        /// <summary>
        /// A Unity event with PointerEventData.  This event is fired when Click enters an object.
        /// </summary>
        public ClickInteractionEvent OnClick = new ClickInteractionEvent();

        ///<inheritdoc/>
        public override BaseEventReceiver InitializeRuntimeEventReceiver()
        {
            ClickReceiver ClickReceiver = new ClickReceiver(this);

            EventReceiver = ClickReceiver;

            return ClickReceiver;
        }
    }
}
