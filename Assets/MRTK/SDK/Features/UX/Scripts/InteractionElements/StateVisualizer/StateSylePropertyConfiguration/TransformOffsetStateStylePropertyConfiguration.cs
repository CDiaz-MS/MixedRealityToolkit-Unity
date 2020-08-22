using Microsoft.MixedReality.Toolkit.UI.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CreateAssetMenu]
    public class TransformOffsetStateStylePropertyConfiguration : StateStylePropertyConfiguration
    {
        public Vector3 Position;

        public Vector3 Rotation;

        public Vector3 Scale;

        public TransformOffsetStateStylePropertyConfiguration()
        {
            StylePropertyName = "Transform Offset";
        }

        public override StateStyleProperty CreateRuntimeInstance()
        {
            Debug.Log("Transform Create");

            TransformOffsetStateStyleProperty stateStyleProperty = new TransformOffsetStateStyleProperty(this);

            StateStyleProperty = stateStyleProperty;

            return stateStyleProperty;
        }

    }
}
