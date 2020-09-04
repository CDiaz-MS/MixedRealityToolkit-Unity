// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    public class InteractiveButton : BaseInteractiveElement,
        IMixedRealityPointerHandler
    {

        private void Start()
        {
            if (IsStateTracking("Toggle"))
            {
                var toggleEventConfig = EventReceiverManager.GetEventConfiguration("Toggle") as ToggleInteractionEventConfiguration;
                
                if (toggleEventConfig.IsSelected)
                {
                    SetStateOn(CoreInteractionState.Toggle);

                    EventReceiverManager.InvokeStateEvent("Toggle", null);
                }
                else
                {
                    SetStateOff(CoreInteractionState.Toggle);
             
                    EventReceiverManager.InvokeStateEvent("Toggle", null);
                }
            }
        }

        #region Click and Toggle

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (IsStateTracking("Click"))
            {
                SetStateOn(CoreInteractionState.Click);

                EventReceiverManager.InvokeStateEvent("Click", eventData);

                SetStateOff(CoreInteractionState.Click);
            }

            if (IsStateTracking("Toggle"))
            {
                if (IsStateActive(CoreInteractionState.Toggle))
                {
                    SetStateOff(CoreInteractionState.Toggle);

                    EventReceiverManager.InvokeStateEvent("Toggle", eventData);
                }
                else
                {
                    SetStateOn(CoreInteractionState.Toggle);

                    EventReceiverManager.InvokeStateEvent("Toggle", eventData);
                }
            }
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            
        }

        #endregion

    }
}