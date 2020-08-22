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
    public class BasicButton : BaseInteractable
    {
        public override void Start()
        {
            base.Start();

            // Handle cases for changing list sizes
            //StateManager.TrackedStates.Add(new InteractionState("Press"));


            //AddNewState("Focus");

        }

    }

}