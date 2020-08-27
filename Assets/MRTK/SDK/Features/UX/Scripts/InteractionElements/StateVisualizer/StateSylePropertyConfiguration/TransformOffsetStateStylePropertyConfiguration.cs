// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The configuration for the Transform Offset state style property.  This configuration is used in the State 
    /// Visualizer component. 
    /// </summary>
    [CreateAssetMenu(fileName = "TransformOffsetStateStylePropertyConfiguration", menuName = "Mixed Reality Toolkit/State Visualizer/State Style Property Configurations/Transform Offset")]
    public class TransformOffsetStateStylePropertyConfiguration : StateStylePropertyConfiguration
    {
        public override string StylePropertyName => "Transform Offset";

        [SerializeField]
        [Tooltip("")]
        private Vector3 position = Vector3.zero;

        /// <summary>
        /// The position offset to add to the initial position of the Target game object.  For example, if 
        /// the initial position is (0, 0, 0.5) and this position is (0, 0, 1), the new position of the Target
        /// game object will be (0, 0, 1.5).
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
        /// The rotation offset to add to the initial rotation of the Target game object.
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
        /// The scale offset to add to the initial scale of the Target game object. For example, if the 
        /// initial scale of the target game object is (1, 1, 1) and this scale is (0.5, 0.5, 0.5), the new
        /// scale of the object will be (1.5, 1.5, 1.5).
        /// </summary>
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        public override void CreateRuntimeInstance()
        {
            TransformOffsetStateStyleProperty stateStyleProperty = new TransformOffsetStateStyleProperty(this);

            StateStyleProperty = stateStyleProperty;
        }

    }
}
