using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{

    [CreateAssetMenu]
    public abstract class BaseInteractionEventConfiguration : ScriptableObject
    {
        [NonSerialized]
        public string Name;

        public BaseEventReceiver EventReceiver;

        public abstract BaseEventReceiver CreateRuntimeInstance();
    }
}
