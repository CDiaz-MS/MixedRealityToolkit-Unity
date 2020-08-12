// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CustomEditor(typeof(TrackedStates))]
    public class TrackedStatesInspector : UnityEditor.Editor
    {
        private SerializedProperty stateList;

        private static GUIContent RemoveStateLabel;

        protected virtual void OnEnable()
        {

            RemoveStateLabel = new GUIContent(InspectorUIUtility.Minus, "Remove State");
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (stateList == null)
            {
                stateList = serializedObject.FindProperty("stateList");
            }



            InspectorUIUtility.DrawTitle("Tracked States");

            for (int i = 0; i < stateList.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty stateItem = stateList.GetArrayElementAtIndex(i);

                    SerializedProperty name = stateItem.FindPropertyRelative("stateName");
                    SerializedProperty value = stateItem.FindPropertyRelative("stateValue");

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

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Add State Events"))
                        {

                        }

                        if (GUILayout.Button("Remove State Events"))
                        {

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
