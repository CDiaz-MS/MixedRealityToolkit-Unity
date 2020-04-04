﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.LeapMotion;
using Microsoft.MixedReality.Toolkit.LeapMotion.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Inspectors
{
    [CustomEditor(typeof(LeapMotionDeviceManagerProfile))]
    /// <summary>
    /// Custom inspector for the Leap Motion input data provider
    /// </summary>
    public class LeapMotionDeviceManagerProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        protected const string ProfileTitle = "Leap Motion Controller Settings";
        protected const string ProfileDescription = "";

        protected LeapMotionDeviceManagerProfile instance;
        protected SerializedProperty leapControllerOrientation;
        protected SerializedProperty leapControllerOffset;

        protected override void OnEnable()
        {
            base.OnEnable();

            instance = (LeapMotionDeviceManagerProfile)target;
            leapControllerOrientation = serializedObject.FindProperty("leapControllerOrientation");
            leapControllerOffset = serializedObject.FindProperty("leapControllerOffset");
        }

        /// <summary>
        /// Display the MRTK header for the profile and render custom properties
        /// </summary>
        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target);

            RenderCustomInspector();
        }

        /// <summary>
        /// Render the custom properties for the Leap Motion profile
        /// </summary>
        public virtual void RenderCustomInspector()
        {
            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                // Show warning if the leap core assets are not in the project
                if (FileUtilities.FindFilesInAssets("LeapXRServiceProvider.cs").Length == 0)
                {
                    EditorGUILayout.HelpBox("The Leap Motion Core Assets cannot be found in your project. Please follow the instructions Leap Motion MRTK instructions to use this data provider", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.PropertyField(leapControllerOrientation);

                    if (instance.LeapControllerOrientation == LeapControllerOrientation.Desk)
                    {
                        EditorGUILayout.PropertyField(leapControllerOffset);
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.DataProviderConfigurations != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.DataProviderConfigurations.Any(s => profile == s.DeviceManagerProfile);
        }
    }
}
