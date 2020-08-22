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
        private SerializedProperty state;
        private SerializedProperty Target;
        private SerializedProperty stateStyleProperties;
        private StateStylePropertyConfiguration stateStyleConfig;
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

            if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateName.stringValue, stateName.stringValue, MixedRealityStylesUtility.BoldTitleFoldoutStyle, false))
            {
                for (int i = 0; i < stateStyleProperties.arraySize; i++)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                SerializedProperty stateStyleProperty = stateStyleProperties.GetArrayElementAtIndex(i);

                                StateStylePropertyConfiguration configuration = stateStyleProperty.objectReferenceValue as StateStylePropertyConfiguration;

                                EditorGUILayout.Space();

                                if (InspectorUIUtility.FlexButton(RemoveStyleButtonLabel))
                                {
                                    instance.StateStyleProperties.Remove(configuration);

                                    break;
                                }

                                if (configuration.name == "NewStyleProperty")
                                {
                                    using (new EditorGUILayout.VerticalScope())
                                    {
                                        Rect position = EditorGUILayout.GetControlRect();
                                        using (new EditorGUI.PropertyScope(position, new GUIContent("State Style Property"), stateStyleProperty))
                                        {
                                            var stateStyleTypes = TypeCacheUtility.GetSubClasses<StateStylePropertyConfiguration>();

                                            string[] stateStyleClassNames = Enum.GetNames(typeof(CoreStyleProperty)).ToArray();

                                            int id = Array.IndexOf(stateStyleClassNames, -1);

                                            int newId = EditorGUI.Popup(position, id, stateStyleClassNames);

                                            Type propertyType;

                                            if (newId != -1)
                                            {
                                                propertyType = stateStyleTypes[newId];

                                                stateStyleProperty.objectReferenceValue = instance.AddStateStyleProperty(propertyType);

                                                var localConfiguration = stateStyleProperty.objectReferenceValue as StateStylePropertyConfiguration;
                                                localConfiguration.StateName = stateName.stringValue;
                                                localConfiguration.name = configuration.StylePropertyName;
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (InspectorUIUtility.DrawSectionFoldoutWithKey(configuration.StylePropertyName, configuration.StylePropertyName + "_" + stateName.stringValue + i.ToString(), MixedRealityStylesUtility.BoldTitleFoldoutStyle, false))
                                    {
                                        EditorGUILayout.Space();
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            DrawScriptableSubEditor(stateStyleProperty);
                                        }
                                    }
                                }

                                EditorGUILayout.Space();
                            }

                        }
                    }

                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (InspectorUIUtility.FlexButton(AddStyleButtonLabel))
                    {
                        StateStylePropertyConfiguration configuration = ScriptableObject.CreateInstance<MaterialStateStylePropertyConfiguration>();

                        configuration.name = "NewStyleProperty";

                        //configuration.StylePropertyName = "StateStyleProperty";

                        instance.StateStyleProperties.Add(configuration);
                    }
                }
                

                serializedObject.ApplyModifiedProperties();
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

                StateStylePropertyConfiguration configurationTarget = configEditor.target as StateStylePropertyConfiguration;

                
                //Debug.Log(configurationTarget.Target.name);
                

                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }



        private void SetStateStyleProperty(SerializedProperty stateStyleProperty)
        {

        }

        private void AddMenu()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                //if (InspectorUIUtility.FlexButton(AddStyleButtonLabel))
                //{
                //    stateStyleProperties.InsertArrayElementAtIndex(stateStyleProperties.arraySize);

                //    SerializedProperty stateStyleProperty = stateStyleProperties.GetArrayElementAtIndex(stateStyleProperties.arraySize - 1);

                //    //SerializedProperty name = stateStyleProperty.FindPropertyRelative("stylePropertyName");

                //    //stateStyleProperty.objectReferenceValue.name = "New State Style Property";

                //    StateStylePropertyConfiguration configuration = stateStyleProperty.objectReferenceValue as StateStylePropertyConfiguration;

                //    configuration.StylePropertyName = "New State Style Property";

                //    Debug.Log(configuration.StylePropertyName);

                   
                //}
            }
        }




    }
}
