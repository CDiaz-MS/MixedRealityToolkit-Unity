using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;

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

                            

                            //stateStyleConfig = stateStyleProperty;

                            //SerializedProperty name = stateStyleProperty.objectReferenceValue.name;

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

    }
}
