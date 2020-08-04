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
    public class FocusUnityEvent : UnityEvent<FocusEventData> { }

    public class FocusEvents
    {
        public FocusUnityEvent OnFocusOn = new FocusUnityEvent();

        public FocusUnityEvent OnFocusOff = new FocusUnityEvent();
    }
}