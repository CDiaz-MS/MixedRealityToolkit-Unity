using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class MaterialStateStyleProperty : StateStyleProperty
    {
        public MaterialStateStyleProperty(MaterialStateStylePropertyConfiguration stateStylePropertyConfiguration) : base(stateStylePropertyConfiguration)
        {
            MaterialStateStylePropertyConfiguration = stateStylePropertyConfiguration;
        }

        public MaterialStateStylePropertyConfiguration MaterialStateStylePropertyConfiguration;

        //private Material Material => MaterialStateStylePropertyConfiguration.Material;

        private Color Color => MaterialStateStylePropertyConfiguration.Color;

        public override void SetStyleProperty()
        {
            Target.GetComponent<MeshRenderer>().material.color = Color;
        }


    }
}
