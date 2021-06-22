// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeCurve))]
    public class VolumeCurveInspector : UnityEditor.Editor
    {
        private VolumeCurve instanceCurve;
        private SerializedProperty curvePoints;
        private SerializedProperty adjustToCurve;
        private SerializedProperty backPointOverride;
        private SerializedProperty lineSteps;

        public void OnEnable()
        {
            instanceCurve = target as VolumeCurve;

            adjustToCurve = serializedObject.FindProperty("adjustToCurve");
            curvePoints = serializedObject.FindProperty("curvePoints");
            backPointOverride = serializedObject.FindProperty("backPointOverride");
            lineSteps = serializedObject.FindProperty("lineSteps");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawAdjustToCurve();

            DrawSlider();

            DrawAdjustToCurveButton();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAdjustToCurve()
        {
            VolumeInspectorUtility.DrawTitle("Curve Settings");

            EditorGUILayout.PropertyField(curvePoints);
            EditorGUILayout.PropertyField(lineSteps);

            Color previousGUIColor = GUI.color;

            // Fill X
            if (adjustToCurve.boolValue)
            {
                GUI.color = Color.cyan;
            }

            GUI.color = previousGUIColor;
        }

        private void DrawAdjustToCurveButton()
        {
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
        }

        private void DrawSlider()
        {
            SerializedProperty uiVolumePoint = curvePoints.GetArrayElementAtIndex(1);
            //SerializedProperty point = uiVolumePoint.FindPropertyRelative("point");

            EditorGUILayout.PropertyField(backPointOverride);

            if (backPointOverride.boolValue)
            {
                uiVolumePoint.vector3Value = new Vector3(
                    EditorGUILayout.Slider("X Position", uiVolumePoint.vector3Value.x, -1, 1),
                    EditorGUILayout.Slider("Y Position", uiVolumePoint.vector3Value.y, -1, 1),
                    EditorGUILayout.Slider("Z Position", uiVolumePoint.vector3Value.z, -2, 2));
            }
        }

    }
}
