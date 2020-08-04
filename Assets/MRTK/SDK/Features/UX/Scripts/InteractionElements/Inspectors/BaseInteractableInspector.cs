using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseInteractable))]
    public class BaseInteractableInspector : UnityEditor.Editor
    {
        private BaseInteractable instance;
        private SerializedProperty activeStates;
        private SerializedProperty events;
        private SerializedProperty focusevent;

        protected virtual void OnEnable()
        {
            instance = (BaseInteractable)target;
            activeStates = serializedObject.FindProperty("states");
            events = serializedObject.FindProperty("events");
            focusevent = serializedObject.FindProperty("OnFocusOn");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the States scriptable object
            InspectorUIUtility.DrawScriptableFoldout<ActiveStates>(activeStates, "Active States", true);



            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Receivers", "Receivers", MixedRealityStylesUtility.TitleFoldoutStyle, false))
            {
                //SerializedProperty events = serializedObject.FindProperty("Events");
                for (int i = 0; i < events.arraySize; i++)
                {
                    SerializedProperty eventItem = events.GetArrayElementAtIndex(i);
                    //if (InteractableEventInspector.RenderEvent(eventItem))
                    //{
                    //    events.DeleteArrayElementAtIndex(i);
                    //    // If removed, skip rendering rest of list till next redraw
                    //    break;
                    //}

                    EditorGUILayout.PropertyField(eventItem);

                }
            }

                //if (GUILayout.Button(AddEventReceiverLabel))
                //{
                //    AddEvent(events.arraySize);
                //}



                serializedObject.ApplyModifiedProperties();
        }

    }
}
