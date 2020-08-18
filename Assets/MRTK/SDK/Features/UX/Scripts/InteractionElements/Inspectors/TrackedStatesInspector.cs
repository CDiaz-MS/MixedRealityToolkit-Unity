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
    /// <summary>
    /// Custom inspector for the Tracked States scriptable object that is contained in an object that inherits from 
    /// BaseInteractable.
    /// </summary>
    [CustomEditor(typeof(TrackedStates))]
    public class TrackedStatesInspector : UnityEditor.Editor
    {
        private SerializedProperty stateList;
        private SerializedProperty availableStates;

        private static GUIContent RemoveStateButtonLabel;
        private static GUIContent AddStateButtonLabel;

        protected virtual void OnEnable()
        {
            RemoveStateButtonLabel = new GUIContent(InspectorUIUtility.Minus, "Remove State");
            AddStateButtonLabel = new GUIContent(InspectorUIUtility.Plus, "Add State");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InspectorUIUtility.DrawTitle("Tracked States");

            availableStates = serializedObject.FindProperty("availableStates");

            stateList = serializedObject.FindProperty("stateList");

            RenderStates();


            using (new EditorGUILayout.VerticalScope())
            {

                using (new EditorGUILayout.HorizontalScope())
                {
                    //if (InspectorUIUtility.FlexButton(AddStateButtonLabel))
                    //{




                    if (EditorGUILayout.DropdownButton(new GUIContent("Add State"), FocusType.Keyboard))
                    {
                            // Create a menu with the list of available state names 
                            GenericMenu menu = new GenericMenu();

                        for (int i = 0; i < availableStates.arraySize; i++)
                        {
                            SerializedProperty stateName = availableStates.GetArrayElementAtIndex(i);

                            // Disable the menu item if the state is already in the list 

                            menu.AddItem(new GUIContent(stateName.stringValue), false, OnStateAdded);

                        }

                        menu.ShowAsContext();
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();


            if (GUILayout.Button("Create New State"))
            {
                stateList.InsertArrayElementAtIndex(stateList.arraySize);

                SerializedProperty newState = stateList.GetArrayElementAtIndex(stateList.arraySize - 1);

                SerializedProperty name = newState.FindPropertyRelative("stateName");

                Debug.Log(name.stringValue);

                name.stringValue = "NewState";
            }

            serializedObject.ApplyModifiedProperties();
        }



        private void RenderStates()
        {
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
                            InspectorUIUtility.DrawLabel(name.stringValue, 14, InspectorUIUtility.ColorTint10);

                            if (Application.isPlaying)
                            {
                                EditorGUILayout.LabelField(value.intValue.ToString());
                            }

                            if (InspectorUIUtility.SmallButton(RemoveStateButtonLabel))
                            {
                                stateList.DeleteArrayElementAtIndex(i);
                                break;
                            }
                        }

                        EditorGUILayout.Space();
                        EditorGUILayout.Space();

                        // Render state event configuration if an event configuration exists
                        using (new EditorGUILayout.VerticalScope())
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                // Check if this state has state events before they are drawn
                                // For example, the Default state does not have an event configuration but the Focus state does
                                if (CreateEventScriptable(eventConfiguration,name.stringValue))
                                {
                                    if (InspectorUIUtility.DrawSectionFoldoutWithKey(name.stringValue + " State Events", name.stringValue + "Events", MixedRealityStylesUtility.TitleFoldoutStyle, false))
                                    {
                                        using (new EditorGUILayout.VerticalScope())
                                        {
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


        /// <summary>
        /// Create an instance of an associated event scriptable object given the state.
        /// </summary>
        /// <param name="eventConfiguration">The event configuration on an Interaction State</param>
        /// <param name="stateName">The state name</param>
        /// <returns>Returns true if the associated event configuration for this state exists and has been 
        /// initialized.  Returns false if an event configuration for the state does not exist.</returns>
        private bool CreateEventScriptable(SerializedProperty eventConfiguration, string stateName)
        {
            // Get the list of the subclasses for BaseInteractionEventConfiguration and find the
            // event configuration that contains the given state name
            var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
            var eventConfigType = eventConfigurationTypes.Find(t => t.Name.Contains(stateName));

            // Check if the state has an existing event configuration
            if (eventConfigType != null)
            {
                string className = eventConfigType.Name;

                if (eventConfiguration.objectReferenceValue == null)
                {
                    // Initialize the associated scriptable object event configuration with the correct state 
                    eventConfiguration.objectReferenceValue = ScriptableObject.CreateInstance(className);
                    eventConfiguration.objectReferenceValue.name = stateName + "EventConfiguration";
                }

                return true;
            }

            return false;
        }

        private void OnStateAdded()
        {
            Debug.Log("State selected: " );

            stateList.InsertArrayElementAtIndex(stateList.arraySize);

            SerializedProperty newState = stateList.GetArrayElementAtIndex(stateList.arraySize - 1);

            SerializedProperty name = newState.FindPropertyRelative("stateName");

            Debug.Log(name.stringValue);

            name.stringValue = "NewState";
        }

    }
}
