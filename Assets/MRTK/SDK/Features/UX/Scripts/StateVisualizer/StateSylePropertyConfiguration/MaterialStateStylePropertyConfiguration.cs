using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Microsoft.MixedReality.Toolkit.UI
{
    [System.Serializable]   
    public class MaterialStateStylePropertyConfiguration : StateStylePropertyConfiguration
    {
        public string StateStylePropertyName = "Material";

        public Material Material;

        public Color Color;

        public MaterialStateStyleProperty MaterialStateStyleProperty => CreateRuntimeInstance();

        public MaterialStateStyleProperty CreateRuntimeInstance()
        {
            //if (MaterialStateStyleProperty == null)
            MaterialStateStyleProperty stateStyleProperty = new MaterialStateStyleProperty(this);

            stateStyleProperty.SetStyleProperty();

            return stateStyleProperty;
        }

        

    }
}
