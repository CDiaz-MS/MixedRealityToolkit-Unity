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
    /// 
    /// </summary>
    public abstract class BaseEventReceiver
    {

        public string Name { get; protected set; }

        public BaseInteractionEventConfiguration EventConfiguration {get; protected set;}

        protected BaseEventReceiver(BaseInteractionEventConfiguration eventConfiguration, string name)
        {
            EventConfiguration = eventConfiguration;
            Name = name;
        }

        /// <summary>
        /// Update an event receiver 
        /// </summary>
        public abstract void OnUpdate(StateManager state, BaseEventData eventData);

    }
}
