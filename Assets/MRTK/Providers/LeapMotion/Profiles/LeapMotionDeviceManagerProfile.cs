// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿


using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Leap Motion Profile", fileName = "LeapMotionDeviceManagerProfile", order = 4)]
    [MixedRealityServiceProfile(typeof(LeapMotionDeviceManager))]
    public class LeapMotionDeviceManagerProfile : BaseMixedRealityProfile
    {
#if LEAPMOTIONCORE_PRESENT
        [SerializeField]
        [Tooltip("")]
        private LeapMotionDeviceManager.LeapControllerLocation leapControllerLocation = LeapMotionDeviceManager.LeapControllerLocation.Headset;

        /// <summary>
        /// 
        /// </summary>
        public LeapMotionDeviceManager.LeapControllerLocation LeapControllerLocation => leapControllerLocation;
#endif
    }

}

