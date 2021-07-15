// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeFlex))]
    public class VolumeFlexInspector : UnityEditor.Editor
    {
        private VolumeFlex instanceFlex;

        private SerializedProperty updateFlex;
        private SerializedProperty spacing;
        private SerializedProperty startingFlexPositionMode;

        private SerializedProperty startCornerPoint;
        private SerializedProperty startFacePoint;
        private SerializedProperty startingXPositionOffsetPercentage;
        private SerializedProperty startingYPositionOffsetPercentage;
        private SerializedProperty startingZPositionOffsetPercentage;
        private SerializedProperty primaryFlowAxis;
        private SerializedProperty secondaryFlowAxis;
        private SerializedProperty tertiaryFlowAxis;

        private Texture flexXYIcon;
        private Texture flexYXIcon;

        private readonly string FlexXYIconGUID = "bfb9169a015b70e4a856ca6b11862e85";
        private readonly string FlexYXIconGUID = "5e0dee437ef17a946b4392781ec2fd6e";

        private const string FlexConfigurationsTitle = "Flex Configurations";
        private const int minButtonHeight = 40;

        public void OnEnable()
        {
            instanceFlex = target as VolumeFlex;
            updateFlex = serializedObject.FindProperty("updateFlex");
            spacing = serializedObject.FindProperty("spacing");
            startingFlexPositionMode = serializedObject.FindProperty("startingFlexPositionMode");
            startCornerPoint = serializedObject.FindProperty("startCornerPoint");
            startFacePoint = serializedObject.FindProperty("startFacePoint");
            startingXPositionOffsetPercentage = serializedObject.FindProperty("startingXPositionOffsetPercentage");
            startingYPositionOffsetPercentage = serializedObject.FindProperty("startingYPositionOffsetPercentage");
            startingZPositionOffsetPercentage = serializedObject.FindProperty("startingZPositionOffsetPercentage");
            primaryFlowAxis = serializedObject.FindProperty("primaryFlowAxis");
            secondaryFlowAxis = serializedObject.FindProperty("secondaryFlowAxis");
            tertiaryFlowAxis = serializedObject.FindProperty("tertiaryFlowAxis");
        }

        public override void OnInspectorGUI()
        {
            GetIcons();

            serializedObject.Update();

            DrawFlexPositioningMode();

            DrawFlexSettings();

            DrawFlexConfigurations();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (!Application.isPlaying)
            {
                instanceFlex.UpdateVolumeFlex();
            } 
        }

        private void DrawFlexSettings()
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(primaryFlowAxis);
            EditorGUILayout.PropertyField(secondaryFlowAxis);
            EditorGUILayout.PropertyField(tertiaryFlowAxis);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spacing);
        }

        private void DrawFlexPositioningMode()
        {
            EditorGUILayout.PropertyField(updateFlex);

            EditorGUILayout.PropertyField(startingFlexPositionMode);

            Color previousGUIColor = GUI.color;

            GUILayoutOption maxWidthPositioningMode = GUILayout.Width((EditorGUIUtility.currentViewWidth * 0.31f));

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(maxWidthPositioningMode))
                {
                    DrawFlexEnumPositionButton(StartingFlexPositionMode.CornerPoint, new GUIContent("Corner Point"), previousGUIColor);

                    EditorGUI.BeginDisabledGroup(startingFlexPositionMode.intValue != (int)StartingFlexPositionMode.CornerPoint);
                    EditorGUILayout.LabelField("Start Corner Point", maxWidthPositioningMode);
                    EditorGUILayout.PropertyField(startCornerPoint, new GUIContent(""));
                    EditorGUI.EndDisabledGroup();
                }

                using (new EditorGUILayout.VerticalScope(maxWidthPositioningMode))
                {
                    DrawFlexEnumPositionButton(StartingFlexPositionMode.FacePoint, new GUIContent("Face Point"), previousGUIColor);

                    EditorGUI.BeginDisabledGroup(startingFlexPositionMode.intValue != (int)StartingFlexPositionMode.FacePoint);
                    EditorGUILayout.LabelField("Start Face Point", maxWidthPositioningMode);
                    EditorGUILayout.PropertyField(startFacePoint, new GUIContent(""));
                    EditorGUI.EndDisabledGroup();
                }

                using (new EditorGUILayout.VerticalScope(maxWidthPositioningMode))
                {
                    DrawFlexEnumPositionButton(StartingFlexPositionMode.CustomPointWithinBounds, new GUIContent("Custom Point"), previousGUIColor);

                    Vector3 volumeBoundsSize = instanceFlex.VolumeBounds.Size * 0.5f;

                    EditorGUI.BeginDisabledGroup(startingFlexPositionMode.intValue != (int)StartingFlexPositionMode.CustomPointWithinBounds);
                    
                    EditorGUILayout.LabelField("Starting X Position Offset Percentage", maxWidthPositioningMode);
                    startingXPositionOffsetPercentage.floatValue = EditorGUILayout.Slider("", startingXPositionOffsetPercentage.floatValue, -1, 1, maxWidthPositioningMode);

                    EditorGUILayout.LabelField("Starting Y Position Offset Percentage", maxWidthPositioningMode);
                    startingYPositionOffsetPercentage.floatValue = EditorGUILayout.Slider("", startingYPositionOffsetPercentage.floatValue, -1, 1, maxWidthPositioningMode);

                    EditorGUILayout.LabelField("Starting Z Position Offset Percentage", maxWidthPositioningMode);
                    startingZPositionOffsetPercentage.floatValue = EditorGUILayout.Slider("", startingZPositionOffsetPercentage.floatValue, -1, 1, maxWidthPositioningMode);

                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        private void DrawFlexEnumPositionButton(StartingFlexPositionMode startingFlexPositionModeVal, GUIContent buttonContent, Color previousGUIColor)
        {
            if (GUILayout.Button(buttonContent, GUILayout.MinHeight(minButtonHeight)))
            {
                startingFlexPositionMode.enumValueIndex = (int)startingFlexPositionModeVal;
            }
        }

        private void DrawFlexConfigurations()
        {
            var flexXYContent = new GUIContent()
            {
                text = " FlexXY",
                image = flexXYIcon
            };

            var flexYXContent = new GUIContent()
            {
                text = " FlexYX",
                image = flexYXIcon
            };

            VolumeInspectorUtility.DrawTitle(FlexConfigurationsTitle);

            using ( new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(flexXYContent, GUILayout.MaxHeight(100), GUILayout.MaxWidth(150)))
                {
                    primaryFlowAxis.enumValueIndex = (int)VolumeFlowAxis.X;
                    secondaryFlowAxis.enumValueIndex = (int)VolumeFlowAxis.Y;
                }

                if (GUILayout.Button(flexYXContent, GUILayout.MaxHeight(100), GUILayout.MaxWidth(150)))
                {
                    primaryFlowAxis.enumValueIndex = (int)VolumeFlowAxis.Y;
                    secondaryFlowAxis.enumValueIndex = (int)VolumeFlowAxis.X;
                }
            }
        }

        private void GetIcons()
        {
            flexXYIcon = VolumeInspectorUtility.GetIcon(FlexXYIconGUID);
            flexYXIcon = VolumeInspectorUtility.GetIcon(FlexYXIconGUID);
        }
    }
}
