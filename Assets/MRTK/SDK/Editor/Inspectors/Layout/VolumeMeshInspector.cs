using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeMesh))]
    [CanEditMultipleObjects]
    public class VolumeMeshInspector : VolumeInspector
    {
        private VolumeMesh instanceMesh;

        private SerializedProperty mesh;

        public override void OnEnable()
        {
            base.OnEnable();

            instanceMesh = target as VolumeMesh;
            mesh = serializedObject.FindProperty("mesh");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DrawMeshSection();

            serializedObject.ApplyModifiedProperties();
        }


        public override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }


        private void DrawMeshSection()
        {
            DrawTitle("Volume Mesh Settings");

            EditorGUILayout.PropertyField(mesh);
        }


    }
}
