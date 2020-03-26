
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;


#if LEAPMOTIONCORE_PRESENT
using Leap;
using Leap.Unity;
using Leap.Unity.Attachments;
#endif

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{

    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsEditor,
        "Leap Motion Device Manager",
        "Profiles/LeapMotionDeviceManagerProfile.asset",
        "MixedRealityToolkit.SDK",
        true)]
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
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }


#region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            // Leap Motion only supports Articulated Hands
            return (capability == MixedRealityCapability.ArticulatedHand);
        }


#endregion IMixedRealityCapabilityCheck Implementation
#if LEAPMOTIONCORE_PRESENT

        /// <summary>
        /// The profile used to configure the camera.
        /// </summary>
        public LeapMotionDeviceManagerProfile SettingsProfile => ConfigurationProfile as LeapMotionDeviceManagerProfile;

        public enum LeapControllerLocation
        {
            // The leap controller is mounted on a headset
            Headset,
            // The leap controller is on a desk
            Desk
        }


        private LeapControllerLocation leapControllerLocation => SettingsProfile.LeapControllerLocation;

        private LeapServiceProvider leapServiceProvider = null;
        private AttachmentHand[] attachmentHands = null;
        private AttachmentHand leftAttachmentHand = null;
        private AttachmentHand rightAttachmentHand = null;

        // List of hands that are currently in frame and detected by the leap motion controller. If there are no hands in the current frame, this list will be empty
        private List<Hand> currentHandsDetectedByLeap;

        private Vector3 leapHandsOffset = new Vector3(0, -0.2f, 0.2f);

        /// <summary>
        /// Dictionary to capture all active hands detected
        /// </summary>
        private readonly Dictionary<Handedness, LeapMotionArticulatedHand> trackedHands = new Dictionary<Handedness, LeapMotionArticulatedHand>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<Handedness, AttachmentHand> trackedLeapHands = new Dictionary<Handedness, AttachmentHand>();

        public bool IsLeapConnected => leapServiceProvider.IsConnected();

        public override void Enable()
        {
            base.Enable();

            // Create provider based on the device orientation
            if (leapControllerLocation == LeapControllerLocation.Headset)
            {
                leapServiceProvider = CameraCache.Main.gameObject.AddComponent<LeapXRServiceProvider>();
                Debug.Log("Headset");
            }

            if (leapControllerLocation == LeapControllerLocation.Desk)
            {
                GameObject leapProvider = new GameObject("LeapProvider");
                leapServiceProvider = leapProvider.AddComponent<LeapServiceProvider>();

                // Follow the transform of the main camera by adding the service provider as a child of the main camera
                leapProvider.transform.parent = CameraCache.Main.transform;

                // Apply hand position offset, make sure the hands are in view and in front of the camera
                leapServiceProvider.transform.position += leapHandsOffset;
                Debug.Log("Desk");
            }

            GameObject leapAttachmentHands = new GameObject("LeapHands");

            attachmentHands = leapAttachmentHands.AddComponent<AttachmentHands>().attachmentHands;

            foreach (var hand in attachmentHands)
            {
                if (hand.chirality == Chirality.Left)
                {
                    leftAttachmentHand = hand;
                }
                else
                {
                    rightAttachmentHand = hand;
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            Debug.Log("Leap Destroy");
        }

        public void OnHandDetected(Handedness handedness)
        {
            // Only create a new hand if the hand does not exist
            if (!trackedHands.ContainsKey(handedness))
            {
                CreateLeapHand(handedness);
            }
        }

        private void OnHandDetectionLost(Handedness handedness)
        {
            if (CoreServices.InputSystem != null)
            {
                CoreServices.InputSystem.RaiseSourceLost(trackedHands[handedness].InputSource);
            }

            // Remove the pointer gameobjects
            foreach (var pointer in trackedHands[handedness].InputSource.Pointers)
            {
                if (pointer != null && (MonoBehaviour)pointer != null && ((MonoBehaviour)pointer).gameObject != null)
                {
                    GameObject.Destroy(((MonoBehaviour)pointer).gameObject);
                }
            }

            // Remove tracked hands
            trackedHands.Remove(trackedHands[handedness].ControllerHandedness);
        }

        private void CreateLeapHand(Handedness handedness)
        {
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
        }


            private void UpdateLeapHandsDictionary(bool isLeftTracked, bool isRightTracked)
        {
            if (isLeftTracked && !trackedLeapHands.ContainsKey(Handedness.Left))
            {
                trackedLeapHands.Add(Handedness.Left, leftAttachmentHand);
                OnHandDetected(Handedness.Left);
            }
            
            if (!isLeftTracked && trackedLeapHands.ContainsKey(Handedness.Left))
            {
                trackedLeapHands.Remove(Handedness.Left);
                OnHandDetectionLost(Handedness.Left);
            }


            if (isRightTracked && !trackedLeapHands.ContainsKey(Handedness.Right))
            {
                trackedLeapHands.Add(Handedness.Right, rightAttachmentHand);
                OnHandDetected(Handedness.Right);
            }

            if (!isRightTracked && trackedLeapHands.ContainsKey(Handedness.Right))
            {
                trackedLeapHands.Remove(Handedness.Right);
                OnHandDetectionLost(Handedness.Right);
            }

        }

        // Convert leap hand chilarity to mrtk handedness
        private Handedness HandChilarityToHandedness(Chirality chirality)
        {
            return (chirality == Chirality.Left) ? Handedness.Left : Handedness.Right;
        }

        public override void Update()
        {
            base.Update();

            if (IsLeapConnected)
            {
                // Update this list with the current hands in the frame
                currentHandsDetectedByLeap = leapServiceProvider.CurrentFrame.Hands;

                // if the number of tracked hands has changed
                // update tracked hands
                if (currentHandsDetectedByLeap.Count != trackedLeapHands.Count)
                {
                    UpdateLeapHandsDictionary(leftAttachmentHand.isTracked, rightAttachmentHand.isTracked);
                }

                // Update the hand/hands that are in trackedhands
                foreach (KeyValuePair<Handedness, LeapMotionArticulatedHand> hand in trackedHands)
                {
                    hand.Value.UpdateState();
                }
            }
        }
#endif
    }

}

