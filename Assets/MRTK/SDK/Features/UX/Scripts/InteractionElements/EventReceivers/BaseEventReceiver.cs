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

        /// <summary>
        /// Defines whether Unity Events should be hidden in inspector for this type of EventReceiver
        /// </summary>
        public virtual bool HideUnityEvents => false;

        protected BaseInteractableEvent unityEvent;
        /// <summary>
        /// Each Receiver has a base Event it raises, (in addition to others).
        /// </summary>
        public BaseInteractableEvent Event { get => unityEvent; set => unityEvent = value; }

        /// <summary>
        /// Targeted component for Event Receiver at runtime
        /// </summary>
        public MonoBehaviour Host { get; set; }

        /// <summary>
        /// Constructs an interaction receiver that will raise unity event when triggered.
        /// </summary>
        /// <param name="ev">Unity event to invoke. Add more events in deriving class.</param>
        /// <param name="name">Name of the unity event that will get invoked (visible in editor).</param>
        protected BaseEventReceiver(BaseInteractableEvent ev, string name)
        {
            unityEvent = ev;
            Name = name;
        }

        /// <summary>
        /// The state has changed
        /// </summary>
        public abstract void OnUpdate(StateManager state, BaseInteractable source, BaseEventData eventData);

    }
}
