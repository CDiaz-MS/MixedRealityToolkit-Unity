using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class BasicButton : BaseInteractable
    {

        public override void Start()
        {
            base.Start();

            StateManager.ActiveStates.Add(new State() { Index = 1, Name = "Press", ActiveIndex = -1, Bit = 0, Value = 0 });
        }

    }

}