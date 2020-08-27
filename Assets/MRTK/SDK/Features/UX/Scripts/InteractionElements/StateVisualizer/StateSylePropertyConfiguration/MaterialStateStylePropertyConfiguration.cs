// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The configuration for the Material state style property.  This configuration is used in the State 
    /// Visualizer component. 
    /// </summary>
    [CreateAssetMenu(fileName = "MaterialStateStylePropertyConfiguration", menuName = "Mixed Reality Toolkit/State Visualizer/State Style Property Configurations/Material")]
    public class MaterialStateStylePropertyConfiguration : StateStylePropertyConfiguration
    {
        public override string StylePropertyName => "Material";

        [SerializeField]
        [Tooltip("The material to set for the Target game object, i.e. the material of the Target game object will" +
            " be set to this material.")]
        private Material material = null;

        /// <summary>
        /// The material to set for the Target game object, i.e. the material of the Target game object will 
        /// be set to this material.
        /// </summary>
        public Material Material
        {
            get => material;
            set => material = value;
        }

        public override void CreateRuntimeInstance()
        {
            MaterialStateStyleProperty stateStyleProperty = new MaterialStateStyleProperty(this);

            StateStyleProperty = stateStyleProperty;
        }
    }
}
