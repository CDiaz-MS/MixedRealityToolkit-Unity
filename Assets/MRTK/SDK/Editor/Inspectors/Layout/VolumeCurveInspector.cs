// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeCurve))]
    public class VolumeCurveInspector : VolumeInspector
    {
        private VolumeCurve instanceCurve;
        private SerializedProperty curvePoints;
        private SerializedProperty adjustToCurve;
        private SerializedProperty backPointOverride;
        private SerializedProperty lineSteps;

        public override void OnEnable()
        {
            base.OnEnable();

            instanceCurve = target as VolumeCurve;

            adjustToCurve = serializedObject.FindProperty("adjustToCurve");
            curvePoints = serializedObject.FindProperty("curvePoints");
            backPointOverride = serializedObject.FindProperty("backPointOverride");
            lineSteps = serializedObject.FindProperty("lineSteps");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DrawAdjustToCurve();

            DrawSlider();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAdjustToCurve()
        {
            DrawTitle("Curve Settings");


            EditorGUILayout.PropertyField(curvePoints);
            EditorGUILayout.PropertyField(lineSteps);


            Color previousGUIColor = GUI.color;

            // Fill X
            if (adjustToCurve.boolValue)
            {
                GUI.color = Color.cyan;
            }

            if (GUILayout.Button("Adjust To Curve"))
            {
                if (adjustToCurve.boolValue)
                {
                    adjustToCurve.boolValue = false;
                }
                else
                {
                    adjustToCurve.boolValue = true;
                }
            }

            GUI.color = previousGUIColor;
        }


        private void DrawSlider()
        {
            SerializedProperty uiVolumePoint = curvePoints.GetArrayElementAtIndex(1);
            SerializedProperty point = uiVolumePoint.FindPropertyRelative("point");

            EditorGUILayout.PropertyField(backPointOverride);

            if (backPointOverride.boolValue)
            {
                point.vector3Value = new Vector3(
                    EditorGUILayout.Slider("X Position", point.vector3Value.x, -1, 1),
                    EditorGUILayout.Slider("Y Position", point.vector3Value.y, -1, 1),
                    EditorGUILayout.Slider("Z Position", point.vector3Value.z, -2, 2));
            }
        }

    }
}
