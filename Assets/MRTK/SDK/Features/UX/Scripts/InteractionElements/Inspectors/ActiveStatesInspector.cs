// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
    [CustomEditor(typeof(ActiveStates))]
    public class ActiveStatesInspector : UnityEditor.Editor
    {
        protected ActiveStates instance;
        protected SerializedProperty stateList;

        private static GUIContent RemoveStateLabel;
        //private static readonly GUIContent AddStateLabel = new GUIContent("+", "Add State");

        protected virtual void OnEnable()
        {
            instance = (ActiveStates)target;

            RemoveStateLabel = new GUIContent(InspectorUIUtility.Minus, "Remove State");
            stateList = serializedObject.FindProperty("stateList");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InspectorUIUtility.DrawTitle("Tracked States");
            //EditorGUILayout.HelpBox("Manage state configurations to drive Interactables or Transitions", MessageType.None);

            for (int i = 0; i < stateList.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty stateItem = stateList.GetArrayElementAtIndex(i);

                    SerializedProperty name = stateItem.FindPropertyRelative("Name");
                    SerializedProperty value = stateItem.FindPropertyRelative("Value");

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(name.stringValue);

                        if (Application.isPlaying)
                        {
                            EditorGUILayout.LabelField(value.intValue.ToString());
                        }
                        
                        if (InspectorUIUtility.SmallButton(RemoveStateLabel))
                        {
                            stateList.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Existing State"))
                {
                    stateList.InsertArrayElementAtIndex(stateList.arraySize);
                }

                if (GUILayout.Button("Create New State"))
                {

                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
