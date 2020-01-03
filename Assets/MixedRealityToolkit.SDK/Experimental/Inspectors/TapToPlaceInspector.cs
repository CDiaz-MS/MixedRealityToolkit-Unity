// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Inspectors
{
    [CustomEditor(typeof(TapToPlace))]
    internal class TapToPlaceInspector : UnityEditor.Editor
    {
        protected TapToPlace instance;
        protected SerializedProperty gameObjectToPlace;
        protected SerializedProperty keepOrientationVertical;
        //protected SerializedProperty autoStart;
        //protected SerializedProperty colliderPresent;
        protected SerializedProperty defaultPlacementDistance;
        protected SerializedProperty maxRaycastDistance;
        protected SerializedProperty magneticSurfaces;
        protected SerializedProperty trackedTargetType;
        

        protected virtual void OnEnable()
        {
            instance = (TapToPlace)target;
            gameObjectToPlace = serializedObject.FindProperty("gameObjectToPlace");
            defaultPlacementDistance = serializedObject.FindProperty("defaultPlacementDistance");
            magneticSurfaces = serializedObject.FindProperty("magneticSurfaces");
            maxRaycastDistance = serializedObject.FindProperty("maxRaycastDistance");

            trackedTargetType = serializedObject.FindProperty("trackedTargetType");
            //autoStart = serializedObject.FindProperty("autoStart");
            keepOrientationVertical = serializedObject.FindProperty("keepOrientationVertical");

            // Set the default behavior for the controller ray for the tracked object type
        }

        public override void OnInspectorGUI()
        {
            RenderCustomInspector();
          
        }

        public virtual void RenderCustomInspector()
        {
            serializedObject.Update();

            // Disable ability to edit through the inspector if in play mode 
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                EditorGUILayout.PropertyField(gameObjectToPlace, true);
            }

            // If the GameObject to place is null set it to the gameobject
            if (instance.GameObjectToPlace == null)
            {
                instance.GameObjectToPlace = instance.gameObject;

                //// Make sure there is a collider
                //if (!instance.ColliderPresent)
                //{
                //    // TO DO: Add a warning message
                //}
            }

            // If the object is on a surface and the hand is moved back
            // Take the object off the surface

            // Allow editing of these properties
            UpdateProperties();

            serializedObject.ApplyModifiedProperties();
        }

        public virtual void UpdateProperties()
        {
            // Allow these properties to be edited during play mode

            //EditorGUILayout.PropertyField(autoStart);
            EditorGUILayout.PropertyField(keepOrientationVertical);
            EditorGUILayout.PropertyField(defaultPlacementDistance);
            EditorGUILayout.PropertyField(maxRaycastDistance);
            EditorGUILayout.PropertyField(magneticSurfaces,true);
            

        }
    }
}
