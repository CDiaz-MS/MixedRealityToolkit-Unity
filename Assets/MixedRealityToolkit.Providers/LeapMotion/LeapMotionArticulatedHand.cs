using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    [MixedRealityController(
    SupportedControllerType.ArticulatedHand,
    new[] { Handedness.Left, Handedness.Right })]
    class LeapMotionArticulatedHand : BaseHand
    {
        private readonly LeapMotionArticulatedHandDefinition handDefinition;

        public LeapMotionArticulatedHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null) : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            Debug.Log("New leap hand created");
            handDefinition = new LeapMotionArticulatedHandDefinition(inputSource, controllerHandedness);

        }

        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Select", AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Grab", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger, MixedRealityInputAction.None),
        };

        public override void SetupDefaultInteractions()
        {
            AssignControllerMappings(DefaultInteractions);
        }

        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            // Is the hand active?
            // Get the joint and return a new MixedRealityPose
            Debug.Log("Try Get Joint");
            
            pose = MixedRealityPose.ZeroIdentity;
            return false;
        }

    }
}
