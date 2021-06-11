// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using UnityEditor;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeMesh))]
    [CanEditMultipleObjects]
    public class VolumeMeshInspector : UnityEditor.Editor
    {
        private VolumeMesh instanceMesh;

        private SerializedProperty mesh;

        public void OnEnable()
        {
            instanceMesh = target as VolumeMesh;
            mesh = serializedObject.FindProperty("mesh");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawMeshSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMeshSection()
        {
            VolumeInspectorUtility.DrawTitle("Volume Mesh Settings");

            EditorGUILayout.PropertyField(mesh);
        }
    }
}
