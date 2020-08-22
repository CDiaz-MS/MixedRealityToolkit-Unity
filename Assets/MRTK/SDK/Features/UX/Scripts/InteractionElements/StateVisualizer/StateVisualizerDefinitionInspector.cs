using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
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
                
                DrawScriptableSubEditor(stateStyleContainer);
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {

                if (GUILayout.Button("Save Current Theme"))
                {
                    //stateList.InsertArrayElementAtIndex(stateList.arraySize);
                }

            }

            

            serializedObject.ApplyModifiedProperties();

        }

        private void DrawScriptableSubEditor(SerializedProperty scriptable)
        {
            if (scriptable.objectReferenceValue != null)
            {
                UnityEditor.Editor configEditor = UnityEditor.Editor.CreateEditor(scriptable.objectReferenceValue);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.Space();
                configEditor.OnInspectorGUI();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }





    }
}
