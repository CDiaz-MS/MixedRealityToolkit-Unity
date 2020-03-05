
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;


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

        LeapMotionArticulatedHand leapHand;

        public void GetLeapMotionController()
        {
            // get the controller and its tracking state
        }



        //public override void Disable()
        //{
        //    base.Disable();
        //}

        public override void Enable()
        {
            //Is the leap motion plugged in? 
            base.Enable();

            LeapXRServiceProvider provider = new LeapXRServiceProvider();

            Debug.Log("Leap Enable");
            OnSourceDetected();

        }

        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Leap initialize");

            // Get the leap motion service 


        }

        public override void Destroy()
        {
            base.Destroy();
            Debug.Log("Leap Destroy");
        }


        public IMixedRealityInputSource inputSource;

        public void OnSourceDetected()
        {
            IMixedRealityPointer[] pointers;
            pointers = RequestPointers(SupportedControllerType.ArticulatedHand, Handedness.Right);
            inputSource = new BaseGenericInputSource("SimulateSelect", pointers, InputSourceType.Hand);
            leapHand = new LeapMotionArticulatedHand(TrackingState.Tracked, Handedness.Right );
            leapHand.SetupDefaultInteractions(); 

            // Set the pointers for an articulated hand to the leap hand
            foreach (var pointer in pointers)
            {
                pointer.Controller = leapHand;
            }

            CoreServices.InputSystem.RaiseSourceDetected(inputSource, leapHand);


        }



        //public override void Reset()
        //{
        //    base.Reset();
        //}

        //public override void Update()
        //{
        //    base.Update();
        //}

        

    }
}
