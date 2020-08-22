using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    public class MaterialStateStyleProperty : StateStyleProperty
    {
        public MaterialStateStyleProperty(MaterialStateStylePropertyConfiguration stateStylePropertyConfiguration) : base(stateStylePropertyConfiguration, "Material")
        {
            MaterialStateStylePropertyConfiguration = stateStylePropertyConfiguration;

            meshRenderer = Target.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
            {
                Debug.LogError("The Target Game Object for the Material State Style Property is missing a MeshRenderer component");
            }
        }

        public MaterialStateStylePropertyConfiguration MaterialStateStylePropertyConfiguration;

        private Material Material => MaterialStateStylePropertyConfiguration.Material;

        private MeshRenderer meshRenderer;

        public override void SetStyleProperty()
        {
            if (meshRenderer != null)
            {
                if (Material != null)
                {
                    meshRenderer.material = Material;
                }
            } 
        }


    }
}
