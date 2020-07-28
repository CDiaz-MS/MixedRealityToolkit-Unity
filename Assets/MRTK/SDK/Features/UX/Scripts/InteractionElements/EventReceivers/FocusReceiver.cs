using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class FocusReceiver : BaseEventReceiver
    {
        /// <summary>
        /// Creates receiver that raises focus enter and exit unity events
        /// </summary>
        public FocusReceiver() : this(new BaseInteractableEvent()) { }

        /// <summary>
        /// Creates receiver that raises focus enter and exit unity events
        /// </summary>
        public FocusReceiver(BaseInteractableEvent ev) : base(ev, "FocusReceiver") { }

        /// <summary>
        /// Raised when focus has left the object
        /// </summary>
        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Focus Off", Tooltip = "Focus has left the object")]
        public BaseInteractableEvent OnFocusOff = new BaseInteractableEvent();

        /// <summary>
        /// Raised when focus has entered the object
        /// </summary>
        public BaseInteractableEvent OnFocusOn => unityEvent;

        private bool hadFocus;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseInteractable source, BaseEventData eventData)
        {
            bool hasFocus = stateManager.GetState("Focus").Value > 0;

            if (hadFocus != hasFocus)
            {
                if (hasFocus)
                {
                    unityEvent.Invoke(eventData);
                    Debug.Log("Focus On Event Receiver");
                }
                else
                {
                    OnFocusOff.Invoke(eventData);
                    Debug.Log("Focus Off Event Receiver");
                }
            }

            hadFocus = hasFocus;
        }

    }
}
