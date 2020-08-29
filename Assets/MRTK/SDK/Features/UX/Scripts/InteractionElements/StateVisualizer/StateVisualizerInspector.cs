// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Custom inspector for the StateVisualizer component
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StateVisualizer))]
    public class StateVisualizerInspector : UnityEditor.Editor
    {
        private StateVisualizer instance;
        private SerializedProperty stateVisualizerDefinition;

        private void OnEnable()
        {
            instance = (StateVisualizer)target;           
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // The stateVisualizerDefinition is scriptable object that is modified regularly and cannot be cached in OnEnable 
            stateVisualizerDefinition = serializedObject.FindProperty("stateVisualizerDefinition");

            RenderStateVisualizerDefinition();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderStateVisualizerDefinition()
        {
            // The StateVisualizerDefinition has its own custom inspector
            InspectorUIUtility.DrawScriptableFoldout<StateVisualizerDefinition>(stateVisualizerDefinition, "State Visualizer Definition", true);

            // If the states in Tracked states are modified then update the states drawn in the state visualizer to match.
            if (InspectorUIUtility.FlexButton(new GUIContent("Update State Visualizer Definition States")))
            {
                instance.UpdateStateVisualizerDefinitionStates();
            }
        }
    }
}
