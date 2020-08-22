using Microsoft.MixedReality.Toolkit.UI.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    public class TransformOffsetStateStyleProperty : StateStyleProperty
    {
        public TransformOffsetStateStyleProperty(TransformOffsetStateStylePropertyConfiguration stateStylePropertyConfiguration) : base(stateStylePropertyConfiguration, "TransformOffset")
        {
            TransformOffsetStateStylePropertyConfiguration = stateStylePropertyConfiguration;

            initialTransform = Target.GetComponent<Transform>();
        }

        public TransformOffsetStateStylePropertyConfiguration TransformOffsetStateStylePropertyConfiguration;

        private Vector3 position => TransformOffsetStateStylePropertyConfiguration.Position;
        private Vector3 rotation => TransformOffsetStateStylePropertyConfiguration.Rotation;
        private Vector3 scale => TransformOffsetStateStylePropertyConfiguration.Scale;

        private Transform currentTransform => Target.GetComponent<Transform>();

        private Transform initialTransform;

        public override void SetStyleProperty()
        {
            Debug.Log("Set transform");

            currentTransform.Translate(position);
            currentTransform.Rotate(rotation);
            currentTransform.localScale += scale;
        }
    }
}
