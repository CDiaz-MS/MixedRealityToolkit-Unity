using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using System.CodeDom;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CustomEditor(typeof(StateStyleConfigurationContainer))]
    public class StateStyleConfigurationContainerInspector : UnityEditor.Editor
    {
        private SerializedProperty stateName;
        private SerializedProperty Target;
        private SerializedProperty stateStyleProperties;
        private static GUIContent AddStyleButtonLabel;
        private static GUIContent RemoveStyleButtonLabel;

        private StateStyleConfigurationContainer instance;

        private void OnEnable()
        {
            instance = target as StateStyleConfigurationContainer;

            //stateName = serializedObject.FindProperty("stateName");

            //Target = serializedObject.FindProperty("target");

            //stateStyleProperties = serializedObject.FindProperty("stateStyleProperties");

            AddStyleButtonLabel = new GUIContent(InspectorUIUtility.Plus, "Add State Style Property");
            RemoveStyleButtonLabel = new GUIContent(InspectorUIUtility.Minus, "Add State Style Property");
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (stateName == null)
            {
                stateName = serializedObject.FindProperty("stateName");
            }

            if (Target == null)
            {
                Target = serializedObject.FindProperty("target");
            }

            if (stateStyleProperties == null)
            {
                stateStyleProperties = serializedObject.FindProperty("stateStyleProperties");
            }

            RenderStateStylePropertyContainer();

            serializedObject.ApplyModifiedProperties();
        }


        private void RenderStateStylePropertyContainer()
        {
            if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateName.stringValue, stateName.stringValue, MixedRealityStylesUtility.BoldTitleFoldoutStyle, false))
            {
                if (stateName.stringValue == "Default")
                {
                    EditorGUILayout.HelpBox("The Default state does not contain properties to edit, the appearance" +
                        " of the object in edit mode is the object's default appearance", MessageType.Info);
                }
                else
                {
                    for (int i = 0; i < stateStyleProperties.arraySize; i++)
                    {
                        using (new EditorGUILayout.VerticalScope(InspectorUIUtility.Box(35)))
                        {
                            SerializedProperty stateStyleProperty = stateStyleProperties.GetArrayElementAtIndex(i);

                            StateStylePropertyConfiguration configuration = stateStyleProperty.objectReferenceValue as StateStylePropertyConfiguration;

                            EditorGUILayout.Space();

                            // If a new property has been added, render a selection menu for the type of property 
                            if (configuration.name == "New Style Property")
                            {
                                SetStateStylePropertyType(stateStyleProperty, configuration);
                            }
                            else
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    using (new EditorGUILayout.VerticalScope())
                                    {
                                        if (InspectorUIUtility.DrawSectionFoldoutWithKey(configuration.StylePropertyName, configuration.StylePropertyName + "_" + stateName.stringValue + i.ToString(), MixedRealityStylesUtility.BoldTitleFoldoutStyle, false))
                                        {
                                            DrawStateStylePropertyScriptableEditor(stateStyleProperty);
                                        }
                                    }

                                    // Draw a - button to remove a state style property from a single state 
                                    if (InspectorUIUtility.SmallButton(RemoveStyleButtonLabel))
                                    {
                                        instance.StateStyleProperties.Remove(configuration);
                                        break;
                                    }
                                }
                            }

                            EditorGUILayout.Space();
                        }
                    }

                    // Draw a + button to add a state style property for a single state
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (InspectorUIUtility.FlexButton(AddStyleButtonLabel))
                        {
                            AddStateStyleProperty();
                        }
                    }
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



        private void SetStateStylePropertyType(SerializedProperty stateStyleProperty, StateStylePropertyConfiguration configuration)
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

                        stateStyleProperty.objectReferenceValue = instance.AddStateStyleProperty(selectedProperty + "StateStylePropertyConfiguration");

                        if (!stateStyleProperty.objectReferenceValue.IsNull())
                        {
                            // Set the values of the local configuration on creation
                            var localConfiguration = stateStyleProperty.objectReferenceValue as StateStylePropertyConfiguration;
                            localConfiguration.StateName = stateName.stringValue;
                            localConfiguration.name = configuration.StylePropertyName;
                        }
                        else
                        {
                            Debug.LogError("The state style property added has a null type.");
                        }
                    }
                }
            }

        }

        private void AddStateStyleProperty()
        {
            // When a new property is added via inspector, initially initialize the state style property with the 
            // MaterialStateStylePropertyConfiguration type and add it to the stateStyleProperties list
            // This stateStyleProperty Configuration type will be changed after the user selects a the type from an enum
            StateStylePropertyConfiguration configuration = ScriptableObject.CreateInstance<MaterialStateStylePropertyConfiguration>();

            // Change the configuration name as a flag to display the CoreStylesProperty enum values for the user to select from 
            configuration.name = "New Style Property";

            instance.StateStyleProperties.Add(configuration);
        }




    }
}
