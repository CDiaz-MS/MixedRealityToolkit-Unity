// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private StateVisualizerDefinition instance;
        private SerializedProperty stateContainers;
        private SerializedProperty trackedStates;

        private static readonly GUIContent AddStyleButtonLabel = new GUIContent(InspectorUIUtility.Plus, "Add State Style Property");
        private static readonly GUIContent RemoveStyleButtonLabel = new GUIContent(InspectorUIUtility.Minus, "Add State Style Property");

        private static bool newStylePropertyAdded = false;
        private void OnEnable()
        {
            instance = target as StateVisualizerDefinition;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            trackedStates = serializedObject.FindProperty("trackedStates");

            //EditorGUILayout.PropertyField(trackedStates);

            //// The stateContainers is a list of scriptable objects that are modified regularly and cannot be cached in OnEnable 
            stateContainers = serializedObject.FindProperty("stateContainers");

            //// Each state in tracked states has an associated state container
            //RenderStateContainers();

            //EditorGUILayout.Space();

            //using (new EditorGUILayout.HorizontalScope())
            //{
            //    if (GUILayout.Button("Save Current Theme"))
            //    {
            //        // Save the current config of the statevisualizer definition to a folder in the proj
            //    }
            //}

            //using (new EditorGUILayout.HorizontalScope())
            //{
            //    if (GUILayout.Button("Sync States in Tracked States"))
            //    {
            //        instance.UpdateStateStyleContainers();
            //    }
            //}

            serializedObject.ApplyModifiedProperties();
        }


        private void RenderStateContainers()
        {
            for (int i = 0; i < stateContainers.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.Space();

                    SerializedProperty stateContainer = stateContainers.GetArrayElementAtIndex(i);
                    SerializedProperty stateName = stateContainer.FindPropertyRelative("stateName");
                    SerializedProperty stateStyleProperties = stateContainer.FindPropertyRelative("stateStyleProperties");

                    // Check if the app is playing to disable adding properties during runtime 
                    bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;

                    if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateName.stringValue, stateName.stringValue, MixedRealityStylesUtility.BoldTitleFoldoutStyle, false))
                    {
                        if (stateName.stringValue == "Default")
                        {
                            EditorGUILayout.HelpBox("The Default state does not contain properties to edit, the appearance" +
                                " of the object in edit mode is the object's default appearance", MessageType.Info);
                        }
                        else
                        {
                            RenderStateStyleProperties(stateStyleProperties, i);

                            // Draw a + button to add a state style property for a single state
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                using (new EditorGUI.DisabledScope(isPlayMode))
                                {
                                    if (InspectorUIUtility.FlexButton(AddStyleButtonLabel))
                                    {
                                        AddStateStyleProperty(stateStyleProperties, stateContainer, i);
                                    }
                                }
                            }
                        }
                    }

                    EditorGUILayout.Space();
                }
            }
        }

        private void RenderStateStyleProperties(SerializedProperty stateStyleProperties, int stateContainerIndex)
        {
            for (int i = 0; i < stateStyleProperties.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(InspectorUIUtility.Box(35)))
                {
                    SerializedProperty stateStyleProperty = stateStyleProperties.GetArrayElementAtIndex(i);

                    StateStylePropertyConfiguration configuration = stateStyleProperty.objectReferenceValue as StateStylePropertyConfiguration;

                    EditorGUILayout.Space();

                    if (newStylePropertyAdded && configuration == null)
                    {
                        Debug.Log("New style property");

                        SetStateStylePropertyType(stateStyleProperty, configuration, stateContainerIndex);
                    }
                    else
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUILayout.VerticalScope())
                            {
                                string foldoutID = configuration.StylePropertyName + configuration.StateName + i;

                                if (InspectorUIUtility.DrawSectionFoldoutWithKey(configuration.StylePropertyName, foldoutID, MixedRealityStylesUtility.BoldTitleFoldoutStyle, false))
                                {
                                    DrawStateStylePropertyScriptableEditor(stateStyleProperty);
                                }
                            }

                            // Draw a - button to remove a state style property from a single state 
                            if (InspectorUIUtility.SmallButton(RemoveStyleButtonLabel))
                            {
                                Debug.Log("Start array size: " + stateStyleProperties.arraySize);
                                //instance.RemoveStateStyleProperty(instance.StateContainers[stateContainerIndex], i);

                                stateStyleProperties.DeleteArrayElementAtIndex(i);

                                stateStyleProperties.serializedObject.ApplyModifiedProperties();

                                Debug.Log("End array size: " + stateStyleProperties.arraySize);

                                break;
                            }
                        }
                    }

                    EditorGUILayout.Space();
                }
            }

        }

        private void DrawStateStylePropertyScriptableEditor(SerializedProperty stateStyleProperty)
        {
            if (stateStyleProperty.objectReferenceValue != null)
            {
                UnityEditor.Editor configEditor = UnityEditor.Editor.CreateEditor(stateStyleProperty.objectReferenceValue);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                configEditor.OnInspectorGUI();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }
        private void AddStateStyleProperty(SerializedProperty stateStyleProperties, SerializedProperty stateContainer, int stateContainerIndex)
        {
            //This has weird behavior for a list of scriptable objects

            stateStyleProperties.InsertArrayElementAtIndex(stateStyleProperties.arraySize);

            SerializedProperty newStateStyleProperty = stateStyleProperties.GetArrayElementAtIndex(stateStyleProperties.arraySize - 1);

            newStateStyleProperty.objectReferenceValue = null;


            //// When a new property is added via inspector, initially initialize the state style property with the 
            //// MaterialStateStylePropertyConfiguration type and add it to the stateStyleProperties list
            //// This stateStyleProperty Configuration type will be changed after the user selects a the type from an enum
            //StateStylePropertyConfiguration configuration = ScriptableObject.CreateInstance<MaterialStateStylePropertyConfiguration>();

            //// Change the configuration name as a flag to display the CoreStylesProperty enum values for the user to select from 
            //configuration.name = "New State Style Property";
            ////configuration.Target = instance.ta

            //instance.StateContainers[stateContainerIndex].StateStyleProperties.Add(configuration);

            newStylePropertyAdded = true;
        }

        private void SetStateStylePropertyType(SerializedProperty stateStyleProperty, StateStylePropertyConfiguration configuration, int index)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                InspectorUIUtility.DrawLabel("Select State Style Property Type", 13, InspectorUIUtility.ColorTint10);

                EditorGUILayout.Space();

                Rect position = EditorGUILayout.GetControlRect();
                using (new EditorGUI.PropertyScope(position, new GUIContent("State Style Property"), stateStyleProperty))
                {
                    string[] stateStyleNames = Enum.GetNames(typeof(CoreStyleProperty)).ToArray();
                    int id = Array.IndexOf(stateStyleNames, -1);
                    int newId = EditorGUI.Popup(position, id, stateStyleNames);

                    if (newId != -1)
                    {
                        string selectedProperty = stateStyleNames[newId];

                        stateStyleProperty.objectReferenceValue = instance.AddStateStyleProperty(instance.StateContainers[index], selectedProperty);

                        newStylePropertyAdded = false;

                        if (stateStyleProperty.objectReferenceValue.IsNull())
                        {
                            Debug.LogError("The state style property was not added and the object reference value is null.");
                        }
                    }
                }
            }
        }
    }
}
