// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Custom inspector for the StateVisualizerDefinition scriptable object.
    /// </summary>
    [CustomEditor(typeof(StateVisualizerDefinition))]
    public class StateVisualizerDefinitionInspector : UnityEditor.Editor
    {
        private SerializedProperty stateStyleConfigurationContainers;

        private void OnEnable()
        {
            //stateStyleConfigurationContainers = serializedObject.FindProperty("stateStyleConfigurationContainers");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (stateStyleConfigurationContainers == null)
            {
                stateStyleConfigurationContainers = serializedObject.FindProperty("stateStyleConfigurationContainers");
            }

            for (int i = 0; i < stateStyleConfigurationContainers.arraySize; i++)
            {
                SerializedProperty stateStyleContainer = stateStyleConfigurationContainers.GetArrayElementAtIndex(i);
                
                InspectorUIUtility.DrawScriptableSubEditor(stateStyleContainer);
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save Current Theme"))
                {

                }
            }

            serializedObject.ApplyModifiedProperties();

        }

    }
}
