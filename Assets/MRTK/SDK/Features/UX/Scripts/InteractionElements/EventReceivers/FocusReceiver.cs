using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    public class FocusReceiver : BaseEventReceiver
    {
        public FocusReceiver() : this(ScriptableObject.CreateInstance<FocusInteractionEventConfiguration>()) { }

        public FocusReceiver(FocusInteractionEventConfiguration interactionEventConfiguration) : base(interactionEventConfiguration, "FocusReceiver") 
        {
            focusEventConfiguration = interactionEventConfiguration;

            baseEventConfiguration = focusEventConfiguration;
        }

        public FocusInteractionEventConfiguration focusEventConfiguration;

        private FocusUnityEvent onFocusOn => focusEventConfiguration.OnFocusOn;

        private FocusUnityEvent onFocusOff => focusEventConfiguration.OnFocusOff;

        private bool hadFocus;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseInteractable source, BaseEventData eventData)
        {
            bool hasFocus = stateManager.GetState("Focus").Value > 0;

            if (hadFocus != hasFocus)
            {
                if (hasFocus)
                {
                    onFocusOn.Invoke(eventData as FocusEventData);
                    Debug.Log("Focus On Event Receiver");
                }
                else
                {
                    onFocusOff.Invoke(eventData as FocusEventData);
                    Debug.Log("Focus Off Event Receiver");
                }
            }

            hadFocus = hasFocus;
        }

    }
}
