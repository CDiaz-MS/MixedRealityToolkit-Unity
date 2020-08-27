﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseInteractiveElement))]
    public class BaseInteractableInspector : UnityEditor.Editor
    {
        private BaseInteractiveElement instance;
        private SerializedProperty trackedStates;

        protected virtual void OnEnable()
        {
            instance = (BaseInteractiveElement)target;
            trackedStates = serializedObject.FindProperty("trackedStates");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the States scriptable object
            InspectorUIUtility.DrawScriptableFoldout<TrackedStates>(trackedStates, "Tracked States", true);

            EditorGUILayout.Space();

            if (InspectorUIUtility.FlexButton(new GUIContent("Add State Visualizer")))
            {
                if (instance.gameObject.GetComponent<StateVisualizer>() == null)
                {
                    instance.gameObject.AddComponent<StateVisualizer>();
                }
                else
                {
                    Debug.LogError("A State Visualizer component is already attached to this gameobject.");                }
                
            }


            serializedObject.ApplyModifiedProperties();
        }

    }
}
