// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(BaseInteractiveElement))]
    public class BaseInteractiveElementInspector : UnityEditor.Editor
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

            RenderTrackedStatesScriptable();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderTrackedStatesScriptable()
        {
            // Draw the States scriptable object
            InspectorUIUtility.DrawScriptableFoldout<TrackedStates>(trackedStates, "Tracked States", true);

            
            //// If the touch state is being tracked, then make sure a near interaction touchable is added to the object
            //if (instance.TrackedStates.IsStateTracked("Touch"))
            //{
            //    if (instance.gameObject.GetComponent<NearInteractionTouchable>() == null)
            //    {
            //        instance.gameObject.AddComponent<NearInteractionTouchable>();
            //    } 
            //}

            EditorGUILayout.Space();
        }

    }
}
