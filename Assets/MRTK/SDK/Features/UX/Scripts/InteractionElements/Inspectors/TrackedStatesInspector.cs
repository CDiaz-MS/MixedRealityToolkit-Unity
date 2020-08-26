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
    /// BaseInteractiveElement.
    /// </summary>
    [CustomEditor(typeof(TrackedStates))]
    public class TrackedStatesInspector : UnityEditor.Editor
    {
        private TrackedStates instance;
        private SerializedProperty stateList;

        private static GUIContent RemoveStateButtonLabel;
        private static GUIContent AddStateButtonLabel;

        protected virtual void OnEnable()
        {
            instance = target as TrackedStates;

            RemoveStateButtonLabel = new GUIContent(InspectorUIUtility.Minus, "Remove State");
            AddStateButtonLabel = new GUIContent(InspectorUIUtility.Plus, "Add State");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InspectorUIUtility.DrawTitle("Tracked States");

            stateList = serializedObject.FindProperty("states");

            RenderStates();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            RenderAddCoreStateButton();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            RenderCreateNewStateButton();

            //RenderTextInputField();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderStates()
        {
            for (int i = 0; i < stateList.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty state = stateList.GetArrayElementAtIndex(i);
                    SerializedProperty stateName = state.FindPropertyRelative("stateName");
                    SerializedProperty stateValue = state.FindPropertyRelative("stateValue");
                    SerializedProperty stateEventConfiguration = state.FindPropertyRelative("eventConfiguration");

                    EditorGUILayout.Space();

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        InspectorUIUtility.DrawLabel(stateName.stringValue, 14, InspectorUIUtility.ColorTint10);

                        // Draw the state value in the state while the application is playing for debugging
                        if (Application.isPlaying)
                        {
                            EditorGUILayout.LabelField(stateValue.intValue.ToString());
                        }

                        // if the state visualizer is attached, then add a button for each state that highlights 
                        // the state in the state visualizer
                        //if (GUILayout.Button("Go State Visualizer"))
                        //{
                        //    if (Highlighter.Highlight("Inspector", "State Visualizer Definition (local)"))
                        //    {
                        //    }
                        //}

                        if (InspectorUIUtility.SmallButton(RemoveStateButtonLabel))
                        {
                            stateList.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }

                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();

                        using (new EditorGUI.IndentLevelScope())
                        {
                            if (CreateEventScriptable(stateEventConfiguration, stateName.stringValue))
                            {
                                // Check if this state has state events before they are drawn
                                // For example, the Default state does not have an event configuration but the Focus state does
                                if (ContainsEventConfiguration(stateEventConfiguration))
                                {
                                    string stateFoldoutID = stateName.stringValue + "EventConfiguration";

                                    if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateName.stringValue + " State Events", stateFoldoutID, MixedRealityStylesUtility.TitleFoldoutStyle, false))
                                    {
                                        // Draw the events for the associated state if this state has an event configuration
                                        DrawStateEventsSciptableSubEditor(stateEventConfiguration);
                                    }
                                }
                            }
                            
                            // When a new state is added via inspector, the name is initialized to "New State" and then changed
                            // to the name the user selects from the list of CoreInteractionStates
                            if (stateName.stringValue == "New Core State")
                            {
                                SetCoreStateType(state, stateName);
                            }

                            if (stateName.stringValue == "Create New State" )
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUILayout.PropertyField(stateName);
                                }
                            }
                        }
                    }

                    EditorGUILayout.Space();                    
                }
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
            var eventConfigType = eventConfigurationTypes.Find(t => t.Name.StartsWith(stateName));

            
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


        private void DrawStateEventsSciptableSubEditor(SerializedProperty scriptable)
        {
            if (scriptable.objectReferenceValue != null)
            {
                UnityEditor.Editor configEditor = UnityEditor.Editor.CreateEditor(scriptable.objectReferenceValue);
                
                using (new EditorGUILayout.VerticalScope(InspectorUIUtility.Box(10)))
                {
                    EditorGUILayout.Space();
                    configEditor.OnInspectorGUI();
                    EditorGUILayout.Space();
                }
            }
        }

        private bool ContainsEventConfiguration(SerializedProperty eventConfiguration)
        {
            // If the event configuration 
            if (eventConfiguration.objectReferenceValue.IsNull())
            {
                return false;
            }

            return true;
        }

        private void SetCoreStateType(SerializedProperty stateProp, SerializedProperty stateNameProp)
        {
            Rect position = EditorGUILayout.GetControlRect();
            using (new EditorGUI.PropertyScope(position, new GUIContent("State"), stateProp))
            {
                string[] coreInteractionStateNames = Enum.GetNames(typeof(CoreInteractionState)).ToArray();
                int id = Array.IndexOf(coreInteractionStateNames, -1);
                int newId = EditorGUI.Popup(position, id, coreInteractionStateNames);

                if (newId != -1)
                {
                    string selectedState = coreInteractionStateNames[newId];

                    // If this state is not already being tracked then change the name
                    if (!instance.IsStateTracked(selectedState))
                    {
                        stateNameProp.stringValue = selectedState;
                    }
                    else
                    {
                        // If a state that is already being tracked is selected, reset the id until a 
                        // state that is not being tracked is selected
                        newId = -1;
                    }
                }
            }
        }

        private SerializedProperty AddNewState(string initialStateName)
        {
            stateList.InsertArrayElementAtIndex(stateList.arraySize);

            SerializedProperty newState = stateList.GetArrayElementAtIndex(stateList.arraySize - 1);

            SerializedProperty name = newState.FindPropertyRelative("stateName");

            SerializedProperty eventConfiguration = newState.FindPropertyRelative("eventConfiguration");

            eventConfiguration.objectReferenceValue = null;

            name.stringValue = initialStateName;

            return name;
        }

        private void RenderAddCoreStateButton()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (InspectorUIUtility.FlexButton(AddStateButtonLabel))
                {
                    AddNewState("New Core State");
                }
            }
        }

        private void RenderCreateNewStateButton()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button("Create New State"))
                {
                    AddNewState("Create New State");
                }
            }
        }


        //private void RenderTextInputField()
        //{
        //    using (new EditorGUILayout.VerticalScope())
        //    {
        //        string text = EditorGUILayout.TextField(new GUIContent("new state:"), newStateName);

        //        newStateName = text;

        //        Debug.Log(newStateName);
        //    }
        //}

    }
}
