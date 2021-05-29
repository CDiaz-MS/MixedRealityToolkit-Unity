using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeGrid))]
    [CanEditMultipleObjects]
    public class VolumeGridInspector : VolumeInspector
    {
        private VolumeGrid instanceGrid;

        private SerializedProperty primaryFlowAxis;
        private SerializedProperty secondaryFlowAxis;
        private SerializedProperty tertiaryFlowAxis;

        private SerializedProperty rows;
        private SerializedProperty cols;
        private SerializedProperty depth;

        private SerializedProperty useCustomCellWidth;
        private SerializedProperty useCustomCellHeight;
        private SerializedProperty useCustomCellDepth;

        private SerializedProperty cellHeight;
        private SerializedProperty cellWidth;
        private SerializedProperty cellDepth;
        
        private SerializedProperty matchRowsToChildren;
        private SerializedProperty matchColsToChildren;
        private SerializedProperty matchDepthToChildren;

        private SerializedProperty axisAlignment;

        private SerializedProperty drawGrid;

        private SerializedProperty disableObjectsWithoutGridPosition;
        private SerializedProperty placeDisabledTransforms;

        private SerializedProperty minWidth;

        private SerializedProperty allowCustomPositionSet;

        public override void OnEnable()
        {
            base.OnEnable();

            instanceGrid = target as VolumeGrid;

            primaryFlowAxis = serializedObject.FindProperty("primaryFlowAxis");
            secondaryFlowAxis = serializedObject.FindProperty("secondaryFlowAxis");
            tertiaryFlowAxis = serializedObject.FindProperty("tertiaryFlowAxis");

            rows = serializedObject.FindProperty("rows");
            cols = serializedObject.FindProperty("cols");
            depth = serializedObject.FindProperty("depth");

            useCustomCellWidth = serializedObject.FindProperty("useCustomCellWidth");
            useCustomCellHeight = serializedObject.FindProperty("useCustomCellHeight");
            useCustomCellDepth = serializedObject.FindProperty("useCustomCellDepth");

            cellWidth = serializedObject.FindProperty("cellWidth");
            cellHeight = serializedObject.FindProperty("cellHeight");
            cellDepth = serializedObject.FindProperty("cellDepth");

            matchRowsToChildren = serializedObject.FindProperty("matchRowsToChildren");
            matchColsToChildren = serializedObject.FindProperty("matchColsToChildren");
            matchDepthToChildren = serializedObject.FindProperty("matchDepthToChildren");

            axisAlignment = serializedObject.FindProperty("axisAlignment");

            drawGrid = serializedObject.FindProperty("drawGrid");
            disableObjectsWithoutGridPosition = serializedObject.FindProperty("disableObjectsWithoutGridPosition");
            placeDisabledTransforms = serializedObject.FindProperty("placeDisabledTransforms");

            minWidth = serializedObject.FindProperty("minWidth");
            allowCustomPositionSet = serializedObject.FindProperty("allowCustomPositionSet");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DrawGridSection();

            serializedObject.ApplyModifiedProperties();
        }


        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            instanceGrid.SetObjectPositions();
        }

        private void DrawFlowSection()
        {
            DrawTitle("Flow Settings");

            EditorGUILayout.PropertyField(primaryFlowAxis);
            EditorGUILayout.PropertyField(secondaryFlowAxis);
            EditorGUILayout.PropertyField(tertiaryFlowAxis);
        }

        private void DrawGridSection()
        {
            DrawTitle("Grid Settings");

            DrawRowColDepthSection();

            DrawColRowGridMatchingSection();

            DrawCustomCellSizeSettings();

            DrawFlowSection();

            DrawOverFlowSettings();

            DrawGridDebuggingSection();
        }

        private void DrawRowColDepthSection()
        {
            EditorGUILayout.PropertyField(rows);
            EditorGUILayout.PropertyField(cols);
            EditorGUILayout.PropertyField(depth);

            EditorGUILayout.PropertyField(axisAlignment);
        }

        private void DrawOverFlowSettings()
        {
            DrawTitle("Grid Overflow Settings");

            EditorGUILayout.PropertyField(disableObjectsWithoutGridPosition);
            EditorGUILayout.PropertyField(placeDisabledTransforms);
        }


        private void DrawCustomCellSizeSettings()
        {
            EditorGUILayout.Space();

            if (DrawSectionFoldoutWithKey("Custom Cell Size Settings", "Custom Cell Size Settings", false))
            {
                EditorGUILayout.PropertyField(useCustomCellWidth);
                EditorGUILayout.PropertyField(cellWidth);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useCustomCellHeight);
                EditorGUILayout.PropertyField(cellHeight);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(useCustomCellDepth);
                EditorGUILayout.PropertyField(cellDepth);
            }
        }

        private void DrawGridDebuggingSection()
        {
            EditorGUILayout.Space();

            if (DrawSectionFoldoutWithKey("Grid Debugging", "Grid Debugging", false))
            {
                EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.FloatField("Width", instanceGrid.BoundsWidthIncrement);
                EditorGUILayout.FloatField("Height", instanceGrid.BoundsHeightIncrement);
                EditorGUILayout.FloatField("Depth", instanceGrid.BoundsDepthIncrement);

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(drawGrid);
            }
        }

        private void DrawColRowGridMatchingSection()
        {
            EditorGUILayout.Space();

            if (DrawSectionFoldoutWithKey("Grid Child Transform Matching", "Grid Child Transform Matching", false))
            {
                GUIContent matchRows = new GUIContent()
                {
                    text = "Match Rows \n to Child Number",
                };

                GUIContent matchCols = new GUIContent()
                {
                    text = "Match Cols \n to Child Number",
                };

                GUIContent matchDepth = new GUIContent()
                {
                    text = "Match Depth \n to Child Number",
                };

                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (DrawColorToggleButton(matchRowsToChildren, matchRows, 50, 100))
                    {
                        matchColsToChildren.boolValue = false;
                        matchDepthToChildren.boolValue = false;
                    }

                    if (DrawColorToggleButton(matchColsToChildren, matchCols, 50, 100))
                    {
                        matchRowsToChildren.boolValue = false;
                        matchDepthToChildren.boolValue = false;
                    }

                    if (DrawColorToggleButton(matchDepthToChildren, matchDepth, 50, 100))
                    {
                        matchRowsToChildren.boolValue = false;
                        matchColsToChildren.boolValue = false;
                    }
                }
            }
        }

    }
}
