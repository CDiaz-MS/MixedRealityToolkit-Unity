// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The base class for all receivers that attach to Interactables
    /// </summary>
    public abstract class BaseEventReceiver
    {
        /// <summary>
        /// Name of Event Receiver
        /// </summary>
        public string Name { get; protected set; }

        public BaseInteractionEventConfiguration baseEventConfiguration;

        /// <summary>
        /// Targeted component for Event Receiver at runtime
        /// </summary>
        public MonoBehaviour Host { get; set; }

        /// <summary>
        /// Constructs an interaction receiver that will raise unity event when triggered.
        /// </summary>
        /// <param name="ev">Unity event to invoke. Add more events in deriving class.</param>
        /// <param name="name">Name of the unity event that will get invoked (visible in editor).</param>
        protected BaseEventReceiver(BaseInteractionEventConfiguration interactionEventConfiguration, string name)
        {
            baseEventConfiguration = interactionEventConfiguration;
            Name = name;
        }

        /// <summary>
        /// The state has changed
        /// </summary>
        public abstract void OnUpdate(StateManager state, BaseInteractable source, BaseEventData eventData);

    }
}
