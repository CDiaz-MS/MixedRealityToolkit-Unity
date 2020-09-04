// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The internal event receiver for the events defined in the Press Interaction Event Configuration
    /// </summary>
    internal class PressReceiver : BaseEventReceiver
    {
        public PressReceiver(PressInteractionEventConfiguration pressEventConfiguration) : base(pressEventConfiguration, "PressReceiver")
        {
            EventConfiguration = pressEventConfiguration;

            pressEventConfig = pressEventConfiguration;
        }

        private readonly PressInteractionEventConfiguration pressEventConfig;

        private UnityEvent onPress => pressEventConfig.OnPress;

        private UnityEvent onPressRelease => pressEventConfig.OnPressRelease;

        private bool hadPress;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool hasPress = stateManager.GetState(CoreInteractionState.Press).Value > 0;

            if (hadPress != hasPress)
            {
                if (hasPress)
                {
                    Debug.Log("On Press");
                    onPress.Invoke();
                }
                else
                {
                    Debug.Log("On Press Release");
                    onPressRelease.Invoke();
                }
            }

            hadPress = hasPress;
        }

    }
}
