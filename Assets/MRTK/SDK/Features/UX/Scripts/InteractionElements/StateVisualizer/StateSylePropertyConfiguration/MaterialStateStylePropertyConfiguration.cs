using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class MaterialStateStylePropertyConfiguration : StateStylePropertyConfiguration
    {
        public Material Material;

        public Color Color;

        public MaterialStateStyleProperty MaterialStateStyleProperty;

        public override StateStyleProperty CreateRuntimeInstance()
        {
            //if (MaterialStateStyleProperty == null)
            MaterialStateStyleProperty stateStyleProperty = new MaterialStateStyleProperty(this);

            MaterialStateStyleProperty = stateStyleProperty;

            return stateStyleProperty;
        }

    }
}
