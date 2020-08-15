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


        private void OnEnable()
        {
            //stateName = serializedObject.FindProperty("StateName");

            //Target = serializedObject.FindProperty("Target");

            //stateStylePropList = serializedObject.FindProperty("StateStylePropList");
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (stateName == null)
            {
                stateName = serializedObject.FindProperty("StateName");
            }

            if (Target == null)
            {
                Target = serializedObject.FindProperty("Target");
            }

            if (stateStylePropList == null)
            {
                stateStylePropList = serializedObject.FindProperty("StateStylePropList");
            }



            if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateName.stringValue, stateName.stringValue, MixedRealityStylesUtility.BoldTitleFoldoutStyle))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        
                        for (int i = 0; i < stateStylePropList.arraySize; i++)
                        {
                            SerializedProperty stateStyleProperty = stateStylePropList.GetArrayElementAtIndex(i);

                            InspectorUIUtility.DrawLabel("Add State Property", 12, InspectorUIUtility.ColorTint10);

                            EditorGUILayout.Space();

                            AddMenu(stateStyleProperty);

                            EditorGUILayout.Space();

                            if (InspectorUIUtility.DrawSectionFoldoutWithKey(stateStyleProperty.objectReferenceValue.name, "Material_"+ stateName.stringValue, MixedRealityStylesUtility.BoldTitleFoldoutStyle))
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    DrawScriptableSubEditor(stateStyleProperty);
                                }
                            }
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


        private void AddMenu(SerializedProperty listItem)
        {

            SerializedProperty className = listItem.FindPropertyRelative("StateStylePropertyName");

            using (new EditorGUILayout.HorizontalScope())
            {
                Rect position = EditorGUILayout.GetControlRect();

                //using (new EditorGUI.PropertyScope(position, new GUIContent("ListProps"), className))
                //{
                    var stateStyleTypes = TypeCacheUtility.GetSubClasses<StateStylePropertyConfiguration>();
      
                        
                    var stateStyleClassNames = stateStyleTypes.Select(t => t?.Name).ToArray();


                    //int id = Array.IndexOf(stateStyleClassNames, className.stringValue);


                    int newId = EditorGUI.Popup(position, 0,  stateStyleClassNames);
                //}
            }
            
        }

    }
}
