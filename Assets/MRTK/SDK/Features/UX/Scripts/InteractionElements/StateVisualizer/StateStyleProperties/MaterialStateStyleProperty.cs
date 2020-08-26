// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public class MaterialStateStyleProperty : StateStyleProperty
    {
        public MaterialStateStyleProperty(MaterialStateStylePropertyConfiguration stateStylePropertyConfiguration) : base(stateStylePropertyConfiguration, "Material")
        {
            materialStateStylePropertyConfiguration = stateStylePropertyConfiguration;

            meshRenderer = Target.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
            {
                Debug.LogError($"The {Target.name} Game Object for the Material State Style Property is missing a MeshRenderer component.");
            }
        }

        private MaterialStateStylePropertyConfiguration materialStateStylePropertyConfiguration = null;

        private Material Material => materialStateStylePropertyConfiguration.Material;

        private MeshRenderer meshRenderer = null;

        public override void SetStyleProperty()
        {
            if (meshRenderer != null)
            {
                if (!Material.IsNull())
                {
                    meshRenderer.material = Material;
                }
                else
                {
                    Debug.LogError($"A Material Property on the {Target.name} game object for the {StateName} state in the StateVisualizer has not been set.");
                }
            } 
        }
    }
}
