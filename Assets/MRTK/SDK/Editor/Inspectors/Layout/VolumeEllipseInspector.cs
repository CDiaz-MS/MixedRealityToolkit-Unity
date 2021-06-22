// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeEllipse))]
    [CanEditMultipleObjects]
    public class VolumeEllipseInspector : UnityEditor.Editor
    {
        private VolumeEllipse instanceElipse;
        private SerializedProperty radius;
        private SerializedProperty useCustomRadius;
        private SerializedProperty volumeEllipseOrientation;

        public void OnEnable()
        {
            instanceElipse = target as VolumeEllipse;

            radius = serializedObject.FindProperty("radius");
            useCustomRadius = serializedObject.FindProperty("useCustomRadius");
            volumeEllipseOrientation = serializedObject.FindProperty("volumeEllipseOrientation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawElipseSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawElipseSection()
        {
            VolumeInspectorUtility.DrawTitle("Volume Ellipse Settings");

            EditorGUILayout.PropertyField(volumeEllipseOrientation);
            EditorGUILayout.PropertyField(useCustomRadius);

            EditorGUI.BeginDisabledGroup(!useCustomRadius.boolValue);
            
            using (new EditorGUILayout.VerticalScope())
            {
                radius.floatValue = EditorGUILayout.Slider("Radius", radius.floatValue, 0, GetMaxSliderValue());
            }

            EditorGUI.EndDisabledGroup();
        }


        private float GetMaxSliderValue()
        {
            VolumeBounds bounds = instanceElipse.VolumeBounds;
            return Mathf.Max(bounds.Extents.x, bounds.Extents.y, bounds.Extents.z);
        }
    }
}
