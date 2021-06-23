// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeGrid))]
    [CanEditMultipleObjects]
    public class VolumeGridInspector : UnityEditor.Editor
    {
        private VolumeGrid instanceGrid;

        private SerializedProperty updateGrid;

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

        private SerializedProperty grid;

        private SerializedProperty onRowCountChanged;
        private SerializedProperty onColCountChanged;
        private SerializedProperty onDepthCountChanged;

        private SerializedProperty smoothingSpeed;
        private SerializedProperty useSmoothing;

        public void OnEnable()
        {
            instanceGrid = target as VolumeGrid;

            updateGrid = serializedObject.FindProperty("updateGrid");

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

            onRowCountChanged = serializedObject.FindProperty("onRowCountChanged");
            onColCountChanged = serializedObject.FindProperty("onColCountChanged");
            onDepthCountChanged = serializedObject.FindProperty("onDepthCountChanged");

            smoothingSpeed = serializedObject.FindProperty("smoothingSpeed");
            useSmoothing = serializedObject.FindProperty("useSmoothing");

            grid = serializedObject.FindProperty("grid");
        }

        private void OnSceneGUI()
        {
            if (!Application.isPlaying && instanceGrid.enabled)
            {
                instanceGrid.CreateGridSetPositions();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGridSection();

            DrawTransitions();

            DrawEvents();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEvents()
        {
            VolumeInspectorUtility.DrawTitle("Grid Events");

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey("Grid Events", "Grid Events", false))
            {
                EditorGUILayout.PropertyField(onRowCountChanged);
                EditorGUILayout.PropertyField(onColCountChanged);
                EditorGUILayout.PropertyField(onDepthCountChanged);
            }
        }

        private void DrawTransitions()
        {
            VolumeInspectorUtility.DrawTitle("Object Movement Settings");

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey("Object Movement Settings", "Object Movement Settings", false))
            {
                EditorGUILayout.PropertyField(useSmoothing);
                EditorGUILayout.PropertyField(smoothingSpeed);
            }
        }

        private void DrawFlowSection()
        {
            VolumeInspectorUtility.DrawTitle("Grid Flow Settings");

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey("Grid Flow Settings", "Grid Flow Settings", false))
            {
                EditorGUILayout.PropertyField(primaryFlowAxis);
                EditorGUILayout.PropertyField(secondaryFlowAxis);
                EditorGUILayout.PropertyField(tertiaryFlowAxis);
            }
        }

        private void DrawGridSection()
        {
            VolumeInspectorUtility.DrawTitle("Grid Settings");

            DrawRowColDepthSection();

            DrawColRowGridMatchingSection();

            DrawCustomCellSizeSettings();
            
            DrawFlowSection();
            
            DrawOverFlowSettings();

            DrawGridDebuggingSection();
        }

        private void DrawRowColDepthSection()
        {
            EditorGUILayout.PropertyField(updateGrid);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(rows);
            if (EditorGUI.EndChangeCheck())
            {
                instanceGrid.Rows = rows.intValue;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(cols);
            if (EditorGUI.EndChangeCheck())
            {
                instanceGrid.Cols = cols.intValue;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(depth);
            EditorGUILayout.PropertyField(axisAlignment);
            if (EditorGUI.EndChangeCheck())
            {
                instanceGrid.AxisAlignment = (AxisAlignment)axisAlignment.enumValueIndex;
                instanceGrid.Depth = depth.intValue;
            }  

        }

        private void DrawOverFlowSettings()
        {
            VolumeInspectorUtility.DrawTitle("Grid Overflow Settings");

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey("Grid Overflow Settings", "Grid Overflow Settings", false))
            {
                EditorGUILayout.PropertyField(disableObjectsWithoutGridPosition);
                EditorGUILayout.PropertyField(placeDisabledTransforms);
            }
        }


        private void DrawCustomCellSizeSettings()
        {
            EditorGUILayout.Space();

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey("Custom Cell Size Settings", "Custom Cell Size Settings", false))
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
            VolumeInspectorUtility.DrawTitle("Draw Grid Gizmos");

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey("Draw Grid Gizmos", "Draw Grid Gizmos", false))
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

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey("Grid Child Transform Matching", "Grid Child Transform Matching", false))
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
                    if (VolumeInspectorUtility.DrawColorToggleButton(matchRowsToChildren, matchRows, 50, 100))
                    {
                        matchColsToChildren.boolValue = false;
                        matchDepthToChildren.boolValue = false;
                    }

                    if (VolumeInspectorUtility.DrawColorToggleButton(matchColsToChildren, matchCols, 50, 100))
                    {
                        matchRowsToChildren.boolValue = false;
                        matchDepthToChildren.boolValue = false;
                    }

                    if (VolumeInspectorUtility.DrawColorToggleButton(matchDepthToChildren, matchDepth, 50, 100))
                    {
                        matchRowsToChildren.boolValue = false;
                        matchColsToChildren.boolValue = false;
                    }
                }
            }
        }
    }
}
