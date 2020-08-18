using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CustomEditor(typeof(StateStylePropertiesConfigurationContainer))]
    public class StateStylePropertiesConfigurationContainerInspector : UnityEditor.Editor
    {
        private SerializedProperty stateName;
        private SerializedProperty Target;
        private SerializedProperty stateStylePropList;
        private StateStylePropertyConfiguration stateStyleConfig;
        private static GUIContent AddStateButtonLabel;


        private void OnEnable()
        {
            //stateName = serializedObject.FindProperty("StateName");

            //Target = serializedObject.FindProperty("Target");

            //stateStylePropList = serializedObject.FindProperty("StateStylePropList");

            AddStateButtonLabel = new GUIContent(InspectorUIUtility.Plus, "Add State");
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

            if (stateStylePropList == null)
            {
                stateStylePropList = serializedObject.FindProperty("stateStylePropList");
            }



            if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateName.stringValue, stateName.stringValue, MixedRealityStylesUtility.BoldTitleFoldoutStyle, false))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        for (int i = 0; i < stateStylePropList.arraySize; i++)
                        {
                            SerializedProperty stateStyleProperty = stateStylePropList.GetArrayElementAtIndex(i);



                            if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateStyleProperty.objectReferenceValue.name, "Material_"+ stateName.stringValue, MixedRealityStylesUtility.BoldTitleFoldoutStyle))
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    DrawScriptableSubEditor(stateStyleProperty);
                                }
                            }

                            EditorGUILayout.Space();

                            AddMenu();

                            EditorGUILayout.Space();

                        }
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


        private void AddMenu()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (InspectorUIUtility.FlexButton(AddStateButtonLabel))
                {
                    var stateStyleTypes = TypeCacheUtility.GetSubClasses<StateStylePropertyConfiguration>();

                    var stateStyleClassNames = stateStyleTypes.Select(t => t?.Name).ToArray();

                        // Create a menu with the list of available state names 
                        GenericMenu menu = new GenericMenu();

                        for (int i = 0; i < stateStyleClassNames.Length; i++)
                        {
                            // Disable the menu item if the state is already in the list 

                            menu.AddItem(new GUIContent(stateStyleClassNames[i]), false, OnStylePropertyAdded);

                        }

                        menu.ShowAsContext();

                }
            }
        }



        private void OnStylePropertyAdded()
        {
            stateStylePropList.InsertArrayElementAtIndex(stateStylePropList.arraySize);
        }

    }
}
