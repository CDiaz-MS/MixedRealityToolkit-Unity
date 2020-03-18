
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using Leap.Unity.Attachments;


// Add if def
#if UNITY_EDITOR || UNITY_STANDALONE_WINDOWS
using Leap;
using Leap.Unity;
#endif



namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsEditor,
        "Leap Motion Device Manager")]
    // Leap Motion SDK does not support Mac
    public class LeapMotionDeviceManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public LeapMotionDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile)
        {
        }

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            // Leap Motion only supports Articulated Hands
            return (capability == MixedRealityCapability.ArticulatedHand);
        }


        #endregion IMixedRealityCapabilityCheck Implementation

        private LeapServiceProvider leapServiceProvider = null;
        private AttachmentHand[] attachmentHands = null;

        // List of hands that are currently in frame and detected by the leap motion controller. If there are no hands in the current frame, this list will be empty
        private List<Hand> currentHandsDetectedByLeap;

        private Vector3 leapHandsOffset = new Vector3(0, -0.2f, 0.2f);

        /// <summary>
        /// Dictionary to capture all active hands detected
        /// </summary>
        private readonly Dictionary<Handedness, LeapMotionArticulatedHand> trackedHands = new Dictionary<Handedness, LeapMotionArticulatedHand>();

        public bool IsLeftHandDetected {
            get 
            {
                foreach(var hand in attachmentHands)
                {
                    if (hand.chirality == Chirality.Left && hand.isTracked)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsRightHandDetected
        {
            get
            {
                foreach (var hand in attachmentHands)
                {
                    if (hand.chirality == Chirality.Right && hand.isTracked)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsLeapConnected => leapServiceProvider.IsConnected();

        public Handedness LeapHandednessToMRTK
        {
            get
            {
                // Different approach?
                if (currentHandsDetectedByLeap.Count != 0 || currentHandsDetectedByLeap != null)
                {
                    if (currentHandsDetectedByLeap.Count == 1)
                    {
                        if (IsLeftHandDetected)
                        {
                            return Handedness.Left;
                        }
                        else
                        {
                            return Handedness.Right;
                        }
                    }

                    if (currentHandsDetectedByLeap.Count == 2)
                    {
                        foreach (var hand in currentHandsDetectedByLeap)
                        {
                            if (hand.IsLeft && !trackedHands.ContainsKey(Handedness.Left))
                            {
                                return Handedness.Left;
                            }

                            if (!hand.IsLeft && !trackedHands.ContainsKey(Handedness.Right))
                            {
                                return Handedness.Right;
                            }
                        }
                    }
                }

                return Handedness.None; 
            }
        }

        public override void Enable()
        {
            base.Enable();
            GameObject leapProvider = new GameObject("LeapProvider");
            leapServiceProvider = leapProvider.AddComponent<LeapServiceProvider>();

            // Follow the transform of the main camera by adding the service provider as a child of the main camera
            leapProvider.transform.parent = CameraCache.Main.transform;

            // Apply hand position offset, make sure the hands are in view and in front of the camera
            leapServiceProvider.transform.position += leapHandsOffset; 

            GameObject leapAttachmentHands = new GameObject("LeapHands");

            attachmentHands = leapAttachmentHands.AddComponent<AttachmentHands>().attachmentHands;
        }

        public override void Destroy()
        {
            base.Destroy();
            Debug.Log("Leap Destroy");
        }

        public void OnHandDetected(Handedness handedness)
        {
            CreateOrGetLeapHand(handedness);
        }

        private void OnHandDetectionLost(LeapMotionArticulatedHand leapHand)
        {
            if (CoreServices.InputSystem != null)
            {
                CoreServices.InputSystem.RaiseSourceLost(leapHand.InputSource);
            }

            // Remove the pointer gameobjects
            foreach (var pointer in leapHand.InputSource.Pointers)
            {
                if (pointer != null && (MonoBehaviour)pointer != null && ((MonoBehaviour)pointer).gameObject != null)
                {
                    GameObject.Destroy(((MonoBehaviour)pointer).gameObject);
                }
            }

            Debug.Log("Removing hand " + leapHand.ControllerHandedness);

            // Remove tracked hands
            trackedHands.Remove(leapHand.ControllerHandedness);
        }

        private LeapMotionArticulatedHand CreateOrGetLeapHand(Handedness handedness)
        {
            // If the hand already exists then return the hand
            if (trackedHands.ContainsKey(handedness))
            {
                return trackedHands[handedness];
            }

            var pointers = RequestPointers(SupportedControllerType.ArticulatedHand, handedness);
            var inputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Leap {handedness} Controller", pointers, InputSourceType.Hand);
            var leapHand = new LeapMotionArticulatedHand(TrackingState.Tracked, handedness, inputSource);

            // Set the pointers for an articulated hand to the leap hand
            foreach (var pointer in pointers)
            {
                pointer.Controller = leapHand;
            }

            // Add to trackedHands
            trackedHands.Add(handedness, leapHand);

            CoreServices.InputSystem.RaiseSourceDetected(inputSource, leapHand);

            return leapHand;   
        }

        public override void Update()
        {
            base.Update();

            if (IsLeapConnected)
            {
                // Update this list with the current hands in the frame
                currentHandsDetectedByLeap = leapServiceProvider.CurrentFrame.Hands;

                // There should be some leap events that say, on hand enter/exit
                // Attachment hands does have IsTracked for each hand

                // Different Approach?
                if (currentHandsDetectedByLeap.Count != trackedHands.Count)
                {
                    // If the number has increased, add a hand to the scene
                    if (currentHandsDetectedByLeap.Count > trackedHands.Count)
                    {
                        // Create a new hand 
                        OnHandDetected(LeapHandednessToMRTK);
                    }

                    // If the tracked hands number has decreased then remove a hand
                    if (currentHandsDetectedByLeap.Count < trackedHands.Count)
                    {
                        // Find which hand is not in frame 
                        if (!IsRightHandDetected && trackedHands.ContainsKey(Handedness.Right))
                        {
                            OnHandDetectionLost(trackedHands[Handedness.Right]);
                        }

                        if (!IsLeftHandDetected && trackedHands.ContainsKey(Handedness.Left))
                        {
                            OnHandDetectionLost(trackedHands[Handedness.Left]);
                        }
                    }
                }              
            }

            // Update the hand/hands that are in trackedhands
            foreach (KeyValuePair<Handedness, LeapMotionArticulatedHand> hand in trackedHands)
            {
                hand.Value.UpdateState();
            }
        }
    }
}
