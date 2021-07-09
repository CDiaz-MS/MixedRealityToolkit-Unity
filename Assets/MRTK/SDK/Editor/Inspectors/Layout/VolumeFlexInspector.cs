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

        private SerializedProperty spacing;
        private SerializedProperty startCornerPoint;
        private SerializedProperty primaryFlowAxis;
        private SerializedProperty secondaryFlowAxis;
        private SerializedProperty tertiaryFlowAxis;
        //private SerializedProperty useBaseVolumeSizes;

        private Texture flexXYIcon;
        private Texture flexYXIcon;

        private readonly string FlexXYIconGUID = "bfb9169a015b70e4a856ca6b11862e85";
        private readonly string FlexYXIconGUID = "5e0dee437ef17a946b4392781ec2fd6e";

        private const string FlexConfigurationsTitle = "Flex Configurations";

        public void OnEnable()
        {
            instanceFlex = target as VolumeFlex;
            spacing = serializedObject.FindProperty("spacing");
            startCornerPoint = serializedObject.FindProperty("startCornerPoint");
            primaryFlowAxis = serializedObject.FindProperty("primaryFlowAxis");
            secondaryFlowAxis = serializedObject.FindProperty("secondaryFlowAxis");
            tertiaryFlowAxis = serializedObject.FindProperty("tertiaryFlowAxis");
            //useBaseVolumeSizes = serializedObject.FindProperty("useBaseVolumeSizes");
        }

        public override void OnInspectorGUI()
        {
            GetIcons();

            serializedObject.Update();

            DrawFlexSettings();
            DrawFlexConfigurations();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (!Application.isPlaying)
            {
                instanceFlex.UpdateFlex();
            } 
        }

        private void DrawFlexSettings()
        {
            EditorGUILayout.PropertyField(startCornerPoint);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(primaryFlowAxis);
            EditorGUILayout.PropertyField(secondaryFlowAxis);
            EditorGUILayout.PropertyField(tertiaryFlowAxis);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spacing);
            //EditorGUILayout.PropertyField(useBaseVolumeSizes);
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
                    primaryFlowAxis.enumValueIndex = (int)VolumeAxis.X;
                    secondaryFlowAxis.enumValueIndex = (int)VolumeAxis.Y;
                }

                if (GUILayout.Button(flexYXContent, GUILayout.MaxHeight(100), GUILayout.MaxWidth(150)))
                {
                    primaryFlowAxis.enumValueIndex = (int)VolumeAxis.Y;
                    secondaryFlowAxis.enumValueIndex = (int)VolumeAxis.X;
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
