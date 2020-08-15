using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseInteractable))]
    public class BaseInteractableInspector : UnityEditor.Editor
    {
        private BaseInteractable instance;
        private SerializedProperty trackedStates;
        private SerializedProperty events;

        protected virtual void OnEnable()
        {
            instance = (BaseInteractable)target;
            trackedStates = serializedObject.FindProperty("trackedStates");
            events = serializedObject.FindProperty("events");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the States scriptable object
            InspectorUIUtility.DrawScriptableFoldout<TrackedStates>(trackedStates, "Tracked States", true);

            serializedObject.ApplyModifiedProperties();
        }

    }
}
