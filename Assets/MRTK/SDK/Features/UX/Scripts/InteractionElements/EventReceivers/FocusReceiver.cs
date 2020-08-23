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
        public FocusReceiver(FocusInteractionEventConfiguration focusEventConfiguration) : base(focusEventConfiguration, "FocusReceiver") 
        {
            EventConfiguration = focusEventConfiguration;

            focusEventConfig = focusEventConfiguration;
        }

        private readonly FocusInteractionEventConfiguration focusEventConfig;

        private FocusInteractionEvent onFocusOn => focusEventConfig.OnFocusOn;

        private FocusInteractionEvent onFocusOff => focusEventConfig.OnFocusOff;

        private bool hadFocus;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool hasFocus = stateManager.GetState(CoreInteractionState.Focus).Value > 0;

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
