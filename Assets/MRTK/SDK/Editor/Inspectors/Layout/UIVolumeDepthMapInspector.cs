using Microsoft.MixedReality.Toolkit.UI.Layout;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(UIVolumeDepthMap))]
    public class UIVolumeDepthMapInspector : UIVolumeInspector
    {
        private UIVolumeDepthMap instanceDepthMap;
        private SerializedProperty depthMap;


        public override void OnEnable()
        {
            base.OnEnable();

            instanceDepthMap = target as UIVolumeDepthMap;

            depthMap = serializedObject.FindProperty("depthMap");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DrawDepthMapSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDepthMapSection()
        {
            InspectorUIUtility.DrawTitle("Depth Map");

            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Depth Map Settings", "Depth Map", MixedRealityStylesUtility.BoldFoldoutStyle, false))
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(depthMap);

                EditorGUILayout.Space();

                if (GUILayout.Button("Apply Depth Map"))
                {
                    instanceDepthMap.SetDepthBasedOnDepthMap();
                }
            }
        }
    }
}
