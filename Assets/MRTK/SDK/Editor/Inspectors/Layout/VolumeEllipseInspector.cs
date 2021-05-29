using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeEllipse))]
    [CanEditMultipleObjects]
    public class VolumeEllipseInspector : VolumeInspector
    {
        private VolumeEllipse instanceElipse;
        private SerializedProperty radius;
        private SerializedProperty useCustomRadius;
        private SerializedProperty volumeEllipseOrientation;

        public override void OnEnable()
        {
            base.OnEnable();

            instanceElipse = target as VolumeEllipse;

            radius = serializedObject.FindProperty("radius");
            useCustomRadius = serializedObject.FindProperty("useCustomRadius");
            volumeEllipseOrientation = serializedObject.FindProperty("volumeEllipseOrientation");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DrawElipseSection();

            serializedObject.ApplyModifiedProperties();
        }


        public override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }


        private void DrawElipseSection()
        {
            DrawTitle("Volume Ellipse Settings");

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
