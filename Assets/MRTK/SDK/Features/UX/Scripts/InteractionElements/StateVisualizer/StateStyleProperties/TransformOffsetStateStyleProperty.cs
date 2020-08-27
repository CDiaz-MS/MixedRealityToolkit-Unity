// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The internal class for the Transform Offset style property.
    /// </summary>
    internal class TransformOffsetStateStyleProperty : StateStyleProperty
    {
        public TransformOffsetStateStyleProperty(TransformOffsetStateStylePropertyConfiguration stateStylePropertyConfiguration) : base(stateStylePropertyConfiguration, "TransformOffset")
        {
            transformOffsetStateStylePropertyConfiguration = stateStylePropertyConfiguration;

            initialTransform = Target.GetComponent<Transform>();
        }

        private TransformOffsetStateStylePropertyConfiguration transformOffsetStateStylePropertyConfiguration = null;

        // The Position defined in the StateStylePropertyConfiguration
        private Vector3 position => transformOffsetStateStylePropertyConfiguration.Position;

        // The Rotation defined in the StateStylePropertyConfiguration
        private Vector3 rotation => transformOffsetStateStylePropertyConfiguration.Rotation;

        // The Scale defined in the StateStylePropertyConfiguration
        private Vector3 scale => transformOffsetStateStylePropertyConfiguration.Scale;

        private Transform currentTransform => Target.GetComponent<Transform>();

        private Transform initialTransform;

        public override void SetStyleProperty()
        {
            currentTransform.position += position;

            // Make sure to add conversion for incrementing the rotation instead of just setting it
            currentTransform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        
            currentTransform.localScale += scale;
        }
    }
}
