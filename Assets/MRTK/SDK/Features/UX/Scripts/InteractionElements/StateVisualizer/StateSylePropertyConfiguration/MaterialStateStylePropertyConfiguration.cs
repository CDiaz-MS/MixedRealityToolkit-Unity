// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu(fileName = "MaterialStateStylePropertyConfiguration", menuName = "Mixed Reality Toolkit/State Visualizer/State Style Property Configurations/Material")]
    public class MaterialStateStylePropertyConfiguration : StateStylePropertyConfiguration
    {
        public override string StylePropertyName => "Material";

        [SerializeField]
        [Tooltip("")]
        private Material material = null;

        /// <summary>
        /// 
        /// </summary>
        public Material Material
        {
            get => material;
            set => material = value;
        }


        public override StateStyleProperty CreateRuntimeInstance()
        {
            MaterialStateStyleProperty stateStyleProperty = new MaterialStateStyleProperty(this);

            StateStyleProperty = stateStyleProperty;

            return stateStyleProperty;
        }
    }
}
