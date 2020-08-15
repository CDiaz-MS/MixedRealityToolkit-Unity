using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class FocusInteractionEventConfiguration : BaseInteractionEventConfiguration
    {
        public FocusUnityEvent OnFocusOn = new FocusUnityEvent();

        public FocusUnityEvent OnFocusOff = new FocusUnityEvent();

        public override BaseEventReceiver CreateRuntimeInstance()
        {
            Name = "Focus";

            FocusReceiver focusReceiver = new FocusReceiver(this);

            EventReceiver = focusReceiver;

            return focusReceiver;
        }
    }
}
