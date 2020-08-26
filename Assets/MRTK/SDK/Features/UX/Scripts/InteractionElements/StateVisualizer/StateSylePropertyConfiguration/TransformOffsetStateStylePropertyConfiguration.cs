// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu(fileName = "TransformOffsetStateStylePropertyConfiguration", menuName = "Mixed Reality Toolkit/State Visualizer/State Style Property Configurations/Transform Offset")]
    public class TransformOffsetStateStylePropertyConfiguration : StateStylePropertyConfiguration
    {
        public override string StylePropertyName => "Transform Offset";

        [SerializeField]
        [Tooltip("")]
        private Vector3 position = Vector3.zero;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        [SerializeField]
        [Tooltip("")]
        private Vector3 rotation = Vector3.zero;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        [SerializeField]
        [Tooltip("")]
        private Vector3 scale = Vector3.zero;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        public override StateStyleProperty CreateRuntimeInstance()
        {
            TransformOffsetStateStyleProperty stateStyleProperty = new TransformOffsetStateStyleProperty(this);

            StateStyleProperty = stateStyleProperty;

            return stateStyleProperty;
        }

    }
}
