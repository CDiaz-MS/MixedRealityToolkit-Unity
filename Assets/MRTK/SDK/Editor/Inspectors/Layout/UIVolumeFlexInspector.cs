// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(UIVolumeFlex))]
    public class UIVolumeFlexInspector : UIVolumeInspector
    {
        private UIVolumeFlex instanceFlex;

        private SerializedProperty primaryAxis;
        private SerializedProperty secondaryAxis;
        private SerializedProperty tertiaryAxis;

        private SerializedProperty minimumXDistanceBetween;
        private SerializedProperty maximumXDistanceBetween;

        private SerializedProperty minimumXRemainingSpace;
        private SerializedProperty maximumXRemainingSpace;

        private SerializedProperty minXContainerDistance;
        private SerializedProperty maxXContainerDistance;

        private SerializedProperty yOffsetMargin;

        public override void OnEnable()
        {
            base.OnEnable();

            instanceFlex = target as UIVolumeFlex;

            primaryAxis = serializedObject.FindProperty("primaryAxis");
            secondaryAxis = serializedObject.FindProperty("secondaryAxis");
            tertiaryAxis = serializedObject.FindProperty("tertiaryAxis");

            minimumXDistanceBetween = serializedObject.FindProperty("minimumXDistanceBetween");
            maximumXDistanceBetween = serializedObject.FindProperty("maximumXDistanceBetween");

            minimumXRemainingSpace = serializedObject.FindProperty("minimumXRemainingSpace");
            maximumXRemainingSpace = serializedObject.FindProperty("maximumXRemainingSpace");

            minXContainerDistance = serializedObject.FindProperty("minXContainerDistance");
            maxXContainerDistance = serializedObject.FindProperty("maxXContainerDistance");

            yOffsetMargin = serializedObject.FindProperty("yOffsetMargin");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DrawFlexSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawFlexSettings()
        {
            EditorGUILayout.Space();

            InspectorUIUtility.DrawTitle("Flex Settings");

            DrawAxisPriority();
            DrawBetweenDistances();

            YSlider();

            DrawContainerDistances();

            DrawRemainingSpace();

        }


        private void DrawAxisPriority()
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(primaryAxis);
            EditorGUILayout.PropertyField(secondaryAxis);
            EditorGUILayout.PropertyField(tertiaryAxis);

            EditorGUILayout.Space();
        }


        private void DrawRemainingSpace()
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(minimumXRemainingSpace);

                if (GUILayout.Button("Set Space X as minimum"))
                {
                    minimumXRemainingSpace.floatValue = instanceFlex.GetRemainingSpace(UnityEngine.Animations.Axis.X, instanceFlex.DirectChildUIVolumes);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(maximumXRemainingSpace);

                if (GUILayout.Button("Set Space X as maximum"))
                {
                    maximumXRemainingSpace.floatValue = instanceFlex.GetRemainingSpace(UnityEngine.Animations.Axis.X, instanceFlex.DirectChildUIVolumes);
                }
            }


            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.FloatField("Remaining Space Between", instanceFlex.GetRemainingSpace(UnityEngine.Animations.Axis.X, instanceFlex.DirectChildUIVolumes));

            EditorGUI.EndDisabledGroup();
        }



        private void DrawBetweenDistances()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(minimumXDistanceBetween);

                if (GUILayout.Button("Set Current X as minimum"))
                {
                    minimumXDistanceBetween.floatValue = instanceFlex.GetCurrentXDistance();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(maximumXDistanceBetween);

                if (GUILayout.Button("Set Current X as maximum"))
                {
                    maximumXDistanceBetween.floatValue = instanceFlex.GetCurrentXDistance();
                }
            }


            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.FloatField("Current X Distance Between", instanceFlex.GetCurrentXDistance());

            EditorGUI.EndDisabledGroup();
        }




        private void YSlider()
        {
            yOffsetMargin.floatValue = EditorGUILayout.Slider("Y Offset", yOffsetMargin.floatValue, 0, 1);

        }


        private void DrawContainerDistances()
        {
            // X axis 
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(minXContainerDistance);

                if (GUILayout.Button("Set Current X as minimum"))
                {
                    minXContainerDistance.floatValue = instanceFlex.GetAxisDistance(UnityEngine.Animations.Axis.X);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(maxXContainerDistance);

                if (GUILayout.Button("Set Current X as maximum"))
                {
                    maxXContainerDistance.floatValue = instanceFlex.GetAxisDistance(UnityEngine.Animations.Axis.X);
                }
            }
        }
    }
}
