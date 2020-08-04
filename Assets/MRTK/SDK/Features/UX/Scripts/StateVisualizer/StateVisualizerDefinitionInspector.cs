using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI
{
    [CustomEditor(typeof(StateVisualizerDefinition))]
    public class StateVisualizerDefinitionInspector : UnityEditor.Editor
    {
        private StateVisualizerDefinition instance;
        private SerializedProperty stateStyleProperties;

        private void OnEnable()
        {
            instance = (StateVisualizerDefinition)target;
            stateStyleProperties = serializedObject.FindProperty("stateStyleProperties");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(stateStyleProperties);

            serializedObject.ApplyModifiedProperties();

        }
    }
}
