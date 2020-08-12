﻿using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CustomEditor(typeof(StateVisualizerDefinition))]
    public class StateVisualizerDefinitionInspector : UnityEditor.Editor
    {
        private SerializedProperty stateStyleProperties;

        private void OnEnable()
        {
            

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (stateStyleProperties == null)
            {
                stateStyleProperties = serializedObject.FindProperty("StateStyleProperties");
            }

            for (int i = 0; i < stateStyleProperties.arraySize; i++)
            {
                SerializedProperty stateConfig = stateStyleProperties.GetArrayElementAtIndex(i);

                //SerializedProperty targetGameObject = stateConfig.FindPropertyRelative("Target");

                //EditorGUILayout.LabelField(targetGameObject.stringValue);


                DrawScriptableSubEditor(stateConfig);
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
