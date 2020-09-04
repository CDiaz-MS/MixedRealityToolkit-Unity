// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The event configuration for the Toggle state.
    /// </summary>
    [CreateAssetMenu(fileName = "ToggleInteractionEventConfiguration", menuName = "Mixed Reality Toolkit/Interactive Element/Event Configurations/Toggle Event Configuration")]
    public class ToggleInteractionEventConfiguration : BaseInteractionEventConfiguration
    {
        public override string StateName => "Toggle";

        /// <summary>
        /// Is the toggle selected when the application starts?
        /// </summary>
        public bool IsSelected = false;

        /// <summary>
        /// A Unity event with PointerEventData.  This event is fired when Toggle enters an object.
        /// </summary>
        public ClickInteractionEvent OnToggleSelect = new ClickInteractionEvent();

        /// <summary>
        /// A Unity event with PointerEventData.  This event is fired when Toggle enters an object.
        /// </summary>
        public ClickInteractionEvent OnToggleDeselect = new ClickInteractionEvent();

        ///<inheritdoc/>
        public override BaseEventReceiver InitializeRuntimeEventReceiver()
        {
            ToggleReceiver ToggleReceiver = new ToggleReceiver(this);

            EventReceiver = ToggleReceiver;

            return ToggleReceiver;
        }
    }
}
