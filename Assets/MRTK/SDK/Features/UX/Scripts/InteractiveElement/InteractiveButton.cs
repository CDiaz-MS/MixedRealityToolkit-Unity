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

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    public class InteractiveButton : BaseInteractiveElement,
        IMixedRealityPointerHandler
    {
        #region Click and Toggle

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (IsStateTracking("Click"))
            {
                SetStateOn(CoreInteractionState.Click);

                EventReceiverManager.InvokeStateEvent("Click", eventData);

                SetStateOff(CoreInteractionState.Click);
            }  


            //if (IsStateTracking("Toggle"))
            //{
            //    if (IsStateActive(CoreInteractionState.Toggle))
            //    {
            //        SetStateOff(CoreInteractionState.Toggle);
            //        Debug.Log("Toggle off: " + GetState(CoreInteractionState.Toggle).Value);
            //    }
            //    else
            //    {
            //        SetStateOn(CoreInteractionState.Toggle);
            //        Debug.Log("Toggle on: " + GetState(CoreInteractionState.Toggle).Value);
            //    }
            //}

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