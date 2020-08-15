// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Boo.Lang;
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

            InspectorUIUtility.DrawTitle("Tracked States");


            stateList = serializedObject.FindProperty("stateList");

            for (int i = 0; i < stateList.arraySize; i++)
            {
               
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty stateItem = stateList.GetArrayElementAtIndex(i);

                    SerializedProperty name = stateItem.FindPropertyRelative("stateName");
                    SerializedProperty value = stateItem.FindPropertyRelative("stateValue");

                    SerializedProperty eventConfiguration = stateItem.FindPropertyRelative("eventConfiguration");

                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.Space();

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            //EditorGUILayout.LabelField(name.stringValue);

                            InspectorUIUtility.DrawLabel(name.stringValue, 14, InspectorUIUtility.ColorTint10);

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
                    
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();


                        using (new EditorGUILayout.VerticalScope())
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                // If this state has state events then draw them
                                if (HasStateEvents(name.stringValue))
                                {
                                    if (InspectorUIUtility.DrawSectionFoldoutWithKey(name.stringValue + " State Events", name.stringValue + "Events", MixedRealityStylesUtility.BoldTitleFoldoutStyle, false))
                                    {
                                        // Get the state name
                                        // find the associated event config
                                        // create an instance of the event config, if they want an event present

                                        using (new EditorGUILayout.VerticalScope())
                                        {
                                            if (eventConfiguration.objectReferenceValue == null)
                                            {
                                                CreateEventScriptable(eventConfiguration, name.stringValue);
                                            }

                                            DrawScriptableSubEditor(eventConfiguration);
                                        }



                                    }
                                }
                            }
                        }
                        


                        EditorGUILayout.Space();
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();

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
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawScriptableSubEditor(SerializedProperty scriptable)
        {
            if (scriptable.objectReferenceValue != null)
            {
                UnityEditor.Editor configEditor = UnityEditor.Editor.CreateEditor(scriptable.objectReferenceValue);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                configEditor.OnInspectorGUI();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }



        private void CreateEventScriptable(SerializedProperty eventConfiguration, string stateName)
        {
            var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();

            var eventConfigType = eventConfigurationTypes.Find(t => t.Name.Contains(stateName));

            string className = eventConfigType.Name;

            eventConfiguration.objectReferenceValue = ScriptableObject.CreateInstance(className);
    
            
        }


        private bool HasStateEvents(string stateName)
        {
            var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();

            var eventConfigType = eventConfigurationTypes.Find(t => t.Name.Contains(stateName));

            if (eventConfigType == null)
            {
                return false;
            }

            return true;
        }

    }
}
