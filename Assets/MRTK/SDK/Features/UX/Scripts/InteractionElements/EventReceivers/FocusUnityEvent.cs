using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI
{

    [System.Serializable]
    public class FocusUnityEvent : UnityEvent<FocusEventData>{}

    [System.Serializable]
    public class FocusReceiverEvents: IInteractableUnityEvent
    {
        [SerializeField]
        public FocusUnityEvent OnFocusOn = new FocusUnityEvent();
        
        public FocusUnityEvent OnFocusOff = new FocusUnityEvent();

        public SystemType EventType => typeof(FocusUnityEvent);

        [SerializeField]
        private string name = "Focus";

        
        public string Name => name;

        [SerializeField]
        public string focusName = "Focus";

        public void CreateUnityEvents()
        {
            //throw new NotImplementedException();
        }
    }
}