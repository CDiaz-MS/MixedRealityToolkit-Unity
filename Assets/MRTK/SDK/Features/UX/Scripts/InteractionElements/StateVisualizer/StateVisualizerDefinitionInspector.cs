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

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // The stateStyleConfigurationContainers is a list of scriptable objects that are modified regularly and cannot be cached in OnEnable 
            stateStyleConfigurationContainers = serializedObject.FindProperty("stateStyleConfigurationContainers");

            // Each state in tracked states has an associated state container
            RenderStateContainers();

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save Current Theme"))
                {
                    // Save the current config of the statevisualizer definition to a folder in the proj
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void RenderStateContainers()
        {
            for (int i = 0; i < stateStyleConfigurationContainers.arraySize; i++)
            {
                SerializedProperty stateStyleContainer = stateStyleConfigurationContainers.GetArrayElementAtIndex(i);

                InspectorUIUtility.DrawScriptableSubEditor(stateStyleContainer);
            }
        }


    }
}
