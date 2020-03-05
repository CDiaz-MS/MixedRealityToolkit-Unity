using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    public class LeapMotionArticulatedHandDefinition : ArticulatedHandDefinition
    {
        public LeapMotionArticulatedHandDefinition(IMixedRealityInputSource source, Handedness handedness) : base(source, handedness)
        {
            Debug.Log("Create Hand Definition");
        }

    }
}
