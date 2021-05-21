using Microsoft.MixedReality.Toolkit.UI.Layout;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(UIVolumeGrid))]
    public class UIVolumeGridInspector : UIVolumeInspector
    {
        private UIVolumeGrid instanceGrid;

        private SerializedProperty primaryFlowAxis;
        private SerializedProperty secondaryFlowAxis;
        private SerializedProperty tertiaryFlowAxis;

        private SerializedProperty rows;
        private SerializedProperty cols;
        private SerializedProperty depth;

        private SerializedProperty matchRowsToChildren;
        private SerializedProperty matchColsToChildren;
        private SerializedProperty matchDepthToChildren;

        private SerializedProperty axisAlignment;

        private SerializedProperty drawGrid;

        private SerializedProperty disableObjectsWithoutGridPosition;
        private SerializedProperty includeInactiveTransforms;

        private SerializedProperty minWidth;

        private SerializedProperty allowCustomPositionSet;

        public override void OnEnable()
        {
            base.OnEnable();

            instanceGrid = target as UIVolumeGrid;

            primaryFlowAxis = serializedObject.FindProperty("primaryFlowAxis");
            secondaryFlowAxis = serializedObject.FindProperty("secondaryFlowAxis");
            tertiaryFlowAxis = serializedObject.FindProperty("tertiaryFlowAxis");

            rows = serializedObject.FindProperty("rows");
            cols = serializedObject.FindProperty("cols");
            depth = serializedObject.FindProperty("depth");

            matchRowsToChildren = serializedObject.FindProperty("matchRowsToChildren");
            matchColsToChildren = serializedObject.FindProperty("matchColsToChildren");
            matchDepthToChildren = serializedObject.FindProperty("matchDepthToChildren");

            axisAlignment = serializedObject.FindProperty("axisAlignment");

            drawGrid = serializedObject.FindProperty("drawGrid");
            disableObjectsWithoutGridPosition = serializedObject.FindProperty("disableObjectsWithoutGridPosition");
            includeInactiveTransforms = serializedObject.FindProperty("includeInactiveTransforms");

            minWidth = serializedObject.FindProperty("minWidth");
            allowCustomPositionSet = serializedObject.FindProperty("allowCustomPositionSet");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DrawFlowSection();

            DrawGridSection();

            DrawAxisAlignment();

            serializedObject.ApplyModifiedProperties();
        }


        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            //instanceGrid.SyncObjectPositionsToGridPositions();
        }

        private void DrawFlowSection()
        {
            InspectorUIUtility.DrawTitle("Flow Settings");

            EditorGUILayout.PropertyField(primaryFlowAxis);
            EditorGUILayout.PropertyField(secondaryFlowAxis);
            EditorGUILayout.PropertyField(tertiaryFlowAxis);
        }

        private void DrawAxisAlignment()
        {
            EditorGUILayout.PropertyField(allowCustomPositionSet);

            InspectorUIUtility.DrawTitle("Display Grid");

            EditorGUILayout.PropertyField(drawGrid);

        }

        private void DrawGridSection()
        {
            InspectorUIUtility.DrawTitle("Grid Settings");

            EditorGUILayout.PropertyField(rows);
            EditorGUILayout.PropertyField(cols);
            EditorGUILayout.PropertyField(depth);

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.FloatField("Width", instanceGrid.BoundsWidthIncrement);
            EditorGUILayout.FloatField("Height", instanceGrid.BoundsHeightIncrement);
            EditorGUILayout.FloatField("Depth", instanceGrid.BoundsDepthIncrement);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(axisAlignment);

            EditorGUILayout.PropertyField(matchRowsToChildren);
            EditorGUILayout.PropertyField(matchColsToChildren);
            EditorGUILayout.PropertyField(matchDepthToChildren);

            
            EditorGUILayout.PropertyField(disableObjectsWithoutGridPosition);
            EditorGUILayout.PropertyField(includeInactiveTransforms);

            EditorGUILayout.PropertyField(minWidth);

        }

    }
}
