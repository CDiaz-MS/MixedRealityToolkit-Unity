using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class MaterialStateStylePropertyConfiguration : StateStylePropertyConfiguration
    {
        public Material Material;

        public override StateStyleProperty CreateRuntimeInstance()
        {
            MaterialStateStyleProperty stateStyleProperty = new MaterialStateStyleProperty(this);

            StateStyleProperty = stateStyleProperty;

            return stateStyleProperty;
        }

    }
}
