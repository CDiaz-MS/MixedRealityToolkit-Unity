// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public class TransformOffsetStateStyleProperty : StateStyleProperty
    {
        public TransformOffsetStateStyleProperty(TransformOffsetStateStylePropertyConfiguration stateStylePropertyConfiguration) : base(stateStylePropertyConfiguration, "TransformOffset")
        {
            transformOffsetStateStylePropertyConfiguration = stateStylePropertyConfiguration;

            initialTransform = Target.GetComponent<Transform>();
        }

        private TransformOffsetStateStylePropertyConfiguration transformOffsetStateStylePropertyConfiguration = null;

        private Vector3 position => transformOffsetStateStylePropertyConfiguration.Position;

        private Vector3 rotation => transformOffsetStateStylePropertyConfiguration.Rotation;

        private Vector3 scale => transformOffsetStateStylePropertyConfiguration.Scale;

        private Transform currentTransform => Target.GetComponent<Transform>();

        private Transform initialTransform;

        public override void SetStyleProperty()
        {
            currentTransform.Translate(position);
            currentTransform.Rotate(rotation);
            currentTransform.localScale += scale;
        }
    }
}
