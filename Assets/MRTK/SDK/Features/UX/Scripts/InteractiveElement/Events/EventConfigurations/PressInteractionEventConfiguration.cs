// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The event configuration for the Press state.
    /// </summary>
    [CreateAssetMenu(fileName = "PressInteractionEventConfiguration", menuName = "Mixed Reality Toolkit/Interactive Element/Event Configurations/Press Event Configuration")]
    public class PressInteractionEventConfiguration : BaseInteractionEventConfiguration
    {
        public override string StateName => "Press";

        /// <summary>
        /// A Unity event for when a button is pressed.  This event is fired when a button is pressed via InteractivePressableButton.
        /// </summary>
        public UnityEvent OnPress = new UnityEvent();

        /// <summary>
        /// A Unity event for when a button is pressed.  This event is fired when a button press is released via InteractivePressableButton.
        /// </summary>
        public UnityEvent OnPressRelease = new UnityEvent();

        ///<inheritdoc/>
        public override BaseEventReceiver InitializeRuntimeEventReceiver()
        {
            PressReceiver PressReceiver = new PressReceiver(this);

            EventReceiver = PressReceiver;

            return PressReceiver;
        }
    }
}
