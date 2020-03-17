using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Leap.Unity.Attachments;
using Leap;
using Leap.Unity;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Leap.Unity.Geometry;

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    [MixedRealityController(
    SupportedControllerType.ArticulatedHand,
    new[] { Handedness.Left, Handedness.Right })]
    public class LeapMotionArticulatedHand : BaseHand
    {
        public LeapMotionArticulatedHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null) 
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            handDefinition = new LeapMotionArticulatedHandDefinition(inputSource, controllerHandedness);
            SetAttachmentHands();

        }

        private readonly LeapMotionArticulatedHandDefinition handDefinition;
        
        // Leap Hands
        private AttachmentHands attachmentHands = null;

        // Attachemnt hand needed for this hand, left or right hand
        private AttachmentHand attachmentHand = null;

        // Default interactions not used
        // public override MixedRealityInteractionMapping[] DefaultInteractions => handDefinition?.DefaultInteractions;

        // Number of joints in an MRTK Tracked Hand
        protected static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        // Joint poses of the MRTK hand based on the leap hand data
        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        public bool IsPinching {
            get
            {
                Vector3 thumbTipPosition = jointPoses[TrackedHandJoint.ThumbTip].Position;
                Vector3 indexTipPosition = jointPoses[TrackedHandJoint.IndexTip].Position;

                // Found this distance with tests but there could be a better range
                if (Vector3.Distance(thumbTipPosition, indexTipPosition) < 0.015)
                {
                    return true;
                    
                }

                return false;
            }
        }

        /// <summary>
        /// From the hand definition, not sure how to use this
        /// </summary>
        public override bool IsInPointingPose => handDefinition.IsInPointingPose;

        private void SetAttachmentHands()
        {
            // Only get the attachment hands if the application is playing
            // The reason this is not in the constructor is that in editmode, the leap hand constructor is called and the attachment hands
            // are null.  The attachment hands are added in play mode and do not exist in edit mode.
            if (Application.isPlaying)
            {
                attachmentHands = GameObject.Find("LeapHands").GetComponent<AttachmentHands>();

                // Get the attachment hand data based on controller handedness
                attachmentHand = Array.Find(attachmentHands.attachmentHands, (hand => HandChilarityToHandedness(hand.chirality) == this.ControllerHandedness));

                // Set all attachment point flags in the hand, the attachment point flags in the attachment hands are joints
                // By default, the attachment points on an attachment hand set to the wrist and the palm
                // Add all joints to the attachmentpoints
                foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
                {
                    attachmentHands.attachmentPoints |= GetLeapAttachmentFlagFromTrackedHandJoint(joint);
                }            attachmentHands = GameObject.Find("LeapHands").GetComponent<AttachmentHands>();

            }

        }

        // Convert leap hand chilarity to mrtk handedness
        private Handedness HandChilarityToHandedness(Chirality chirality)
        {
            return (chirality == Chirality.Left) ? Handedness.Left : Handedness.Right;
        }

        public override void SetupDefaultInteractions()
        {
            // Get input actions from the active profile
            // This is the method used in Julias fake input example
            var inputActions = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions;

            MixedRealityInteractionMapping[] interactions = new[]
            {
                new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, Array.Find(inputActions, x => x.Description.Equals("Pointer Pose"))),
                new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, Array.Find(inputActions, x => x.Description.Equals("Grip Pose"))),
                new MixedRealityInteractionMapping(2, "Select", AxisType.Digital, DeviceInputType.Select, Array.Find(inputActions, x => x.Description.Equals("Select"))),
                new MixedRealityInteractionMapping(3, "Grab", AxisType.SingleAxis, DeviceInputType.TriggerPress, Array.Find(inputActions, x => x.Description.Equals("Grip Press"))),
                new MixedRealityInteractionMapping(4, "Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger, Array.Find(inputActions, x => x.Description.Equals("Index Finger Pose")))
            };

            AssignControllerMappings(interactions);
        }

        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            if (attachmentHand != null)
            {
                AttachmentPointFlags leapAttachmentFlag = GetLeapAttachmentFlagFromTrackedHandJoint(joint);
                AttachmentPointBehaviour leapJoint = attachmentHand.GetBehaviourForPoint(leapAttachmentFlag);

                // Set the pose calculated by the leap motion to a mixed reality pose
                pose = new MixedRealityPose(leapJoint.transform.position, leapJoint.transform.rotation);
                return true;
            }

            Debug.Log("Leap Motion Attachment Hand is null");
            pose = MixedRealityPose.ZeroIdentity;
            return false;
        }

        private AttachmentPointFlags GetLeapAttachmentFlagFromTrackedHandJoint(TrackedHandJoint joint)
        {
            switch (joint)
            {
                case TrackedHandJoint.Palm: return AttachmentPointFlags.Palm;

                case TrackedHandJoint.Wrist: return AttachmentPointFlags.Wrist;

                case TrackedHandJoint.ThumbMetacarpalJoint:
                    //Debug.Log("Leap Motion doesn't support thumb metacarpal tracking. Falling back to the palm.");
                    return AttachmentPointFlags.Palm;

                case TrackedHandJoint.ThumbProximalJoint: return AttachmentPointFlags.ThumbProximalJoint;
                case TrackedHandJoint.ThumbDistalJoint: return AttachmentPointFlags.ThumbDistalJoint;
                case TrackedHandJoint.ThumbTip: return AttachmentPointFlags.ThumbTip;

                case TrackedHandJoint.IndexKnuckle: return AttachmentPointFlags.IndexKnuckle;
                case TrackedHandJoint.IndexMiddleJoint: return AttachmentPointFlags.IndexMiddleJoint;
                case TrackedHandJoint.IndexDistalJoint: return AttachmentPointFlags.IndexDistalJoint;
                case TrackedHandJoint.IndexTip: return AttachmentPointFlags.IndexTip;

                case TrackedHandJoint.MiddleKnuckle: return AttachmentPointFlags.MiddleKnuckle;
                case TrackedHandJoint.MiddleMiddleJoint: return AttachmentPointFlags.MiddleMiddleJoint;
                case TrackedHandJoint.MiddleDistalJoint: return AttachmentPointFlags.MiddleDistalJoint;
                case TrackedHandJoint.MiddleTip: return AttachmentPointFlags.MiddleTip;

                case TrackedHandJoint.RingKnuckle: return AttachmentPointFlags.RingKnuckle;
                case TrackedHandJoint.RingMiddleJoint: return AttachmentPointFlags.RingMiddleJoint;
                case TrackedHandJoint.RingDistalJoint: return AttachmentPointFlags.RingDistalJoint;
                case TrackedHandJoint.RingTip: return AttachmentPointFlags.RingTip;

                case TrackedHandJoint.PinkyKnuckle: return AttachmentPointFlags.PinkyKnuckle;
                case TrackedHandJoint.PinkyMiddleJoint: return AttachmentPointFlags.PinkyMiddleJoint;
                case TrackedHandJoint.PinkyDistalJoint: return AttachmentPointFlags.PinkyDistalJoint;
                case TrackedHandJoint.PinkyTip: return AttachmentPointFlags.PinkyTip;
                default: return AttachmentPointFlags.Wrist;
            }
        }

        public void UpdateState()
        {
            for (int i = 0; i < jointCount; i++)
            {
                TrackedHandJoint handJoint = (TrackedHandJoint)i;
                TryGetJoint(handJoint, out MixedRealityPose pose);

                // Add joint poses from leap to mrtk
                if (!jointPoses.ContainsKey(handJoint))
                {
                    jointPoses.Add(handJoint, pose);
                }
                else
                {
                    jointPoses[handJoint] = pose;
                }
            }

            // Hand definition integration?
            handDefinition?.UpdateHandJoints(jointPoses);

            UpdateVelocity();

            UpdateInteractions();
        }

        protected void UpdateInteractions()
        {
            MixedRealityPose pointerPose = jointPoses[TrackedHandJoint.IndexTip];
            MixedRealityPose gripPose = jointPoses[TrackedHandJoint.Palm];
            MixedRealityPose indexPose = jointPoses[TrackedHandJoint.IndexTip];

            // Update hand Ray with the origin at the index tip
            HandRay.Update(pointerPose.Position, GetPalmNormal(), CameraCache.Main.transform, ControllerHandedness);
            Ray ray = HandRay.Ray;

            pointerPose.Position = ray.origin;
            pointerPose.Rotation = Quaternion.LookRotation(ray.direction);

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        Interactions[i].PoseData = pointerPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, pointerPose);
                        }
                        break;
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = gripPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, gripPose);
                        }
                        break;
                    case DeviceInputType.Select:
                        Interactions[i].BoolData = IsPinching;

                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                    case DeviceInputType.TriggerPress:
                        Interactions[i].BoolData = IsPinching;

                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                    case DeviceInputType.IndexFinger:
                        Interactions[i].PoseData = indexPose;
                        if (Interactions[i].Changed)
                        {
                            // Update the index pose in hand definition 
                            // Does this need to be updated?
                            //handDefinition?.UpdateCurrentIndexPose(Interactions[i]);
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, indexPose);
                        }
                        break;
                }
            }
        }




    }
    
}
