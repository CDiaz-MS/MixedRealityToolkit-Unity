using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StateVisualizer))]
    public class StateVisualizerInspector : UnityEditor.Editor
    {
        private SerializedProperty stateVisualizerDefinition;

        private void OnEnable()
        {
            stateVisualizerDefinition = serializedObject.FindProperty("stateVisualizerDefinition");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the States scriptable object
            InspectorUIUtility.DrawScriptableFoldout<StateVisualizerDefinition>(stateVisualizerDefinition, "State Visualizer Definition", true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
