using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Microsoft.MixedReality.Toolkit.UI
{
    [CreateAssetMenu]
    public class MaterialStateStylePropertyConfiguration : StateStylePropertyConfiguration
    {
        public string StateStylePropertyName = "Material";

        public Material Material;

        public Color Color;

        public MaterialStateStyleProperty MaterialStateStyleProperty;

        public MaterialStateStyleProperty CreateRuntimeInstance()
        {
            //if (MaterialStateStyleProperty == null)
            MaterialStateStyleProperty stateStyleProperty = new MaterialStateStyleProperty(this);

            MaterialStateStyleProperty = stateStyleProperty;

            return stateStyleProperty;
        }

    }
}
