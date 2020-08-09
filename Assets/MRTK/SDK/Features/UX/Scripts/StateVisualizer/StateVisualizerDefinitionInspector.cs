using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI
{
    [CustomEditor(typeof(StateVisualizerDefinition))]
    public class StateVisualizerDefinitionInspector : UnityEditor.Editor
    {
        private SerializedProperty stateStyleProperties;

        private void OnEnable()
        {
            stateStyleProperties = serializedObject.FindProperty("stateStyleProperties");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //EditorGUILayout.PropertyField(stateStyleProperties);

            for (int i = 0; i < stateStyleProperties.arraySize; i++)
            {
                SerializedProperty stateConfig = stateStyleProperties.GetArrayElementAtIndex(i);

                SerializedProperty stateName = stateConfig.FindPropertyRelative("StateName");
                
                SerializedProperty target = stateConfig.FindPropertyRelative("Target");

                // This is a scriptable object within a scriptable object
                SerializedProperty stateStylePropList = stateConfig.FindPropertyRelative("StateStylePropList");

                //SerializedProperty scriptableProp = stateStylePropList.FindPropertyRelative("Target");

                //InspectorUIUtility.DrawScriptableFoldout<StateStylePropertyConfiguration>(stateStylePropList, "State Style Config", true);


                EditorGUILayout.LabelField(stateName.stringValue);

                EditorGUILayout.PropertyField(target);


                EditorGUILayout.PropertyField(stateStylePropList);

                //EditorGUILayout.PropertyField(scriptableProp);






            }


            serializedObject.ApplyModifiedProperties();

        }
    }
}
