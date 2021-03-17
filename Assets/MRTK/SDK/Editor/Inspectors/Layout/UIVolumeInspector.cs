// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.UI.Layout;
using System;
using System.Linq;
using System.IO;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine.Animations;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIVolume))]
    public class UIVolumeInspector : UnityEditor.Editor
    {
        private UIVolume instance;

        private SerializedProperty anchorLocation;
        private SerializedProperty anchorPositionSmoothing;

        private SerializedProperty xAxisDynamicDistribute;
        private SerializedProperty yAxisDynamicDistribute;
        private SerializedProperty zAxisDynamicDistribute;
        private SerializedProperty leftMargin;
        private SerializedProperty topMargin;
        private SerializedProperty forwardMargin;
        private SerializedProperty distributeContainerFillX;
        private SerializedProperty distributeContainerFillY;
        private SerializedProperty distributeContainerFillZ;
        private SerializedProperty distributeSmoothing;

        private SerializedProperty fillToParentX;
        private SerializedProperty fillToParentY;
        private SerializedProperty fillToParentZ;
        private SerializedProperty volumeSizeScaleFactorX;
        private SerializedProperty volumeSizeScaleFactorY;
        private SerializedProperty volumeSizeScaleFactorZ;

        private SerializedProperty useAnchorPositioning;
        private SerializedProperty drawCornerPoints;
        private SerializedProperty drawFacePoints;
        private SerializedProperty childVolumeItems;
        private SerializedProperty backPlateObject;
        private SerializedProperty volumeSizeOrigin;

        private Texture xAxisDistributeIcon;
        private Texture yAxisDistributeIcon;
        private Texture zAxisDistributeIcon;

        private Texture xAxisDistributeFillXIcon;
        private Texture xAxisDistributeFillYIcon;
        private Texture xAxisDistributeFillZIcon;

        private Texture fillParentXIcon;
        private Texture fillParentYIcon;
        private Texture fillParentZIcon;

        private Texture cornerPointsIcon;
        private Texture facePointsIcon;

        // Size Presets 

        private Vector3[] buttonSizePresetsRow1 = new Vector3[4]
        {   
            new Vector3(0.024f, 0.024f, 0.008f), // buttonSizePreset24x24x8
            new Vector3(0.032f, 0.032f, 0.008f), // buttonSizePreset32x32x8
            new Vector3(0.064f, 0.064f, 0.008f), // buttonSizePreset64x64x8
            new Vector3(0.072f, 0.032f, 0.008f), // buttonSizePreset72x32x8
        };

        private Vector3[] buttonSizePresetsRow2 = new Vector3[4]
        {
            new Vector3(0.096f, 0.032f, 0.008f), // buttonSizePreset96x32x8
            new Vector3(0.128f, 0.032f, 0.008f), // buttonSizePreset128x32x8
            new Vector3(0.128f, 0.036f, 0.008f), // buttonSizePreset128x36x8
            new Vector3(0.160f, 0.032f, 0.008f)  // buttonSizePreset160x32x8
        };

        private Vector3[] buttonGroupPresets = new Vector3[4]
        {
            new Vector3(0.190f, 0.032f, 0.008f), // buttonGroupSizePreset192x32x8
            new Vector3(0.160f, 0.032f, 0.008f), // buttonGroupSizePreset160x32x8
            new Vector3(0.032f, 0.096f, 0.008f), // buttonGroupSizePreset32x96x8
            new Vector3(0.024f, 0.072f, 0.008f) // buttonGroupSizePreset24x72x8
        };        
        
        private Vector3[] actionBarPresets = new Vector3[2]
        {
            new Vector3(0.192f, 0.032f, 0.008f), // actionBarSizePreset192x32x8
            new Vector3(0.256f, 0.032f, 0.008f) // actionBarSizePreset256x32x8
        };

        private Vector3[] dialogPresetsRow1 = new Vector3[4]
        {
            new Vector3(0.168f, 0.168f, 0.008f), // dialogMenuSizePreset168x168x8
            new Vector3(0.168f, 0.140f, 0.008f), // dialogMenuSizePreset168x140x8
            new Vector3(0.168f, 0.076f, 0.008f), // dialogMenuSizePreset168x76x8
            new Vector3(0.168f, 0.088f, 0.008f), // dialogMenuSizePreset168x88x8
        };

        private Vector3[] dialogPresetsRow2 = new Vector3[4]
        {
            new Vector3(0.168f, 0.112f, 0.008f), // dialogMenuSizePreset168x112x8
            new Vector3(0.168f, 0.136f, 0.008f), // dialogMenuSizePreset168x136x8
            new Vector3(0.168f, 0.152f, 0.008f), // dialogMenuSizePreset168x152x8
            new Vector3(0.140f, 0.168f, 0.008f) // dialogMenuSizePreset140x168x8
        };

        private Vector3[] listMenuPresets = new Vector3[4]
        {
            new Vector3(0.168f, 0.168f, 0.008f), // listMenuSizePreset168x168x8
            new Vector3(0.168f, 0.200f, 0.008f), // listMenuSizePreset168x200x8
            new Vector3(0.168f, 0.234f, 0.008f), // listMenuSizePreset168x234x8
            new Vector3(0.168f, 0.232f, 0.008f) // listMenuSizePreset168x232x8
        };

        private Vector3[] menuListPresets = new Vector3[3]
        {
            new Vector3(0.104f, 0.104f, 0.008f), // menuListSizePreset104x104x8
            new Vector3(0.104f, 0.168f, 0.008f), // menuListSizePreset104x168x8
            new Vector3(0.080f, 0.128f, 0.008f) // menuListSizePreset80x128x8
        };

        private List<Texture> icons = new List<Texture>();

        private Dictionary<string, int> depthLevelDictionary = new Dictionary<string, int>()
        {
            { "TopLeft", 0 },
            { "TopCenter", 0 },
            { "TopRight", 0 },
            { "CenterLeft", 0 },
            { "CenterCenter", 0 },
            { "CenterRight", 0 },
            { "BottomLeft", 0 },
            { "BottomCenter", 0 },
            { "BottomRight", 0 },
        };

        private string[] dictionaryKeys; 

        private string[] anchorLocations = Enum.GetNames(typeof(AnchorLocation)).ToArray();

        public virtual void OnEnable()
        {
            instance = target as UIVolume;

            anchorLocation = serializedObject.FindProperty("anchorLocation");
            anchorPositionSmoothing = serializedObject.FindProperty("anchorPositionSmoothing");

            xAxisDynamicDistribute = serializedObject.FindProperty("xAxisDynamicDistribute");
            yAxisDynamicDistribute = serializedObject.FindProperty("yAxisDynamicDistribute");
            zAxisDynamicDistribute = serializedObject.FindProperty("zAxisDynamicDistribute");
            leftMargin = serializedObject.FindProperty("leftMargin");
            topMargin = serializedObject.FindProperty("topMargin");
            forwardMargin = serializedObject.FindProperty("forwardMargin");
            distributeContainerFillX = serializedObject.FindProperty("distributeContainerFillX");
            distributeContainerFillY = serializedObject.FindProperty("distributeContainerFillY");
            distributeContainerFillZ = serializedObject.FindProperty("distributeContainerFillZ");
            distributeSmoothing = serializedObject.FindProperty("distributeSmoothing");

            fillToParentX = serializedObject.FindProperty("fillToParentX");
            fillToParentY = serializedObject.FindProperty("fillToParentY");
            fillToParentZ = serializedObject.FindProperty("fillToParentZ");

            volumeSizeScaleFactorX = serializedObject.FindProperty("volumeSizeScaleFactorX");
            volumeSizeScaleFactorY = serializedObject.FindProperty("volumeSizeScaleFactorY");
            volumeSizeScaleFactorZ = serializedObject.FindProperty("volumeSizeScaleFactorZ");

            useAnchorPositioning = serializedObject.FindProperty("useAnchorPositioning");

            drawCornerPoints = serializedObject.FindProperty("drawCornerPoints");
            drawFacePoints = serializedObject.FindProperty("drawFacePoints");
            childVolumeItems = serializedObject.FindProperty("childVolumeItems");
            backPlateObject = serializedObject.FindProperty("backPlateObject");
            volumeSizeOrigin = serializedObject.FindProperty("volumeSizeOrigin");

            dictionaryKeys = depthLevelDictionary.Keys.ToArray();
        }

        public override void OnInspectorGUI()
        {
            GetIcons();

            serializedObject.Update();

            if (!instance.IsRootUIVolume)
            {
                DrawUseAnchorPositioning();

                DrawAnchorButtons();

                EditorGUILayout.Space();

                DrawAnchorSizeSection();

                EditorGUILayout.Space();
            }
            else
            {
                InspectorUIUtility.DrawWarning("This UIVolume is the root transform");

                DrawBackPlateSection();
            }

            DrawCommonContainerSizeSection();

            DrawDistributeButtons();

            EditorGUILayout.Space();

            DrawDebuggingSection();

            DrawChildTransformSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void GetIcons()
        {
            // Get Icons
            foreach (var key in dictionaryKeys)
            {
                Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + key + ".png");

                if (!icons.Contains(icon))
                {
                    icons.Add(icon);
                }
            }

            xAxisDistributeIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "XAxisDistribute" + ".png");
            yAxisDistributeIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "YAxisDistribute" + ".png");
            zAxisDistributeIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "ZAxisDistribute" + ".png");

            xAxisDistributeFillXIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "XAxisDistributeFillX" + ".png");
            xAxisDistributeFillYIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "XAxisDistributeFillY" + ".png");
            xAxisDistributeFillZIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "XAxisDistributeFillZ" + ".png");

            fillParentXIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "FillParentX" + ".png");
            fillParentYIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "FillParentY" + ".png");
            fillParentZIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "FillParentZ" + ".png");

            cornerPointsIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "CornerPoints" + ".png");
            facePointsIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "FacePoints" + ".png");
        }

        private void DrawVolumeSizeOrigin()
        {
            EditorGUILayout.PropertyField(volumeSizeOrigin);
        }

        private void DrawUseAnchorPositioning()
        {
            InspectorUIUtility.DrawTitle("Volume Position");

            Color previousGUIColor = GUI.color;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (useAnchorPositioning.boolValue)
                {
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button("Use Anchor Positioning"))
                {
                    useAnchorPositioning.boolValue = true;
                }

                GUI.color = previousGUIColor;

                if (!useAnchorPositioning.boolValue)
                {
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button("Use Free Positioning"))
                {
                    useAnchorPositioning.boolValue = false;
                }

                GUI.color = previousGUIColor;
            }

            EditorGUILayout.Space();
        }

        private void DrawAnchorSizeSection()
        {
            InspectorUIUtility.DrawTitle("UIVolume Size Settings");

            var fillToParentXContent = new GUIContent()
            {
                text = " Match X Parent Size",
                image = fillParentXIcon
            };

            var fillToParentYContent = new GUIContent()
            {
                text = " Match Y Parent Size",
                image = fillParentYIcon
            };

            var fillToParentZContent = new GUIContent()
            {
                text = " Match Z Parent Size",
                image = fillParentZIcon
            };

            using (new EditorGUILayout.VerticalScope())
            {
                DrawVolumeSizeAxisSection(fillToParentX, volumeSizeScaleFactorX, "X", fillToParentXContent);
                DrawVolumeSizeAxisSection(fillToParentY, volumeSizeScaleFactorY, "Y", fillToParentYContent);

                // Disable Match Z size for text mesh pro volume
                EditorGUI.BeginDisabledGroup(volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.TextMeshPro);

                DrawVolumeSizeAxisSection(fillToParentZ, volumeSizeScaleFactorZ, "Z", fillToParentZContent);

                EditorGUI.EndDisabledGroup();
            }

            if (GUILayout.Button("Equalize Volume Size to Parent"))
            {
                instance.EqualizeVolumeSizeToParent();
            } 
        }

        private void DrawVolumeSizeAxisSection(SerializedProperty axisFillProperty, SerializedProperty axisScaleFactor, string axis, GUIContent icon)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Color previousGUIColor = GUI.color;

                // Fill X
                if (axisFillProperty.boolValue)
                {
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button(icon, GUILayout.MaxHeight(50)))
                {
                    axisFillProperty.boolValue = !axisFillProperty.boolValue;
                }

                GUI.color = previousGUIColor;

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField($"Volume Size {axis} Scale Factor");
                    axisScaleFactor.floatValue = EditorGUILayout.Slider("", axisScaleFactor.floatValue, 0.01f, 1);
                }
            }
        }

        private void DrawAnchorButtons()
        {
            InspectorUIUtility.DrawTitle("Anchor Position");

            using (new EditorGUILayout.VerticalScope())
            {
                int index = 0;

                // Only disable buttons for anchors if anchorPositionOverrideEnabled is true
                EditorGUI.BeginDisabledGroup(useAnchorPositioning.boolValue == false);

                for (int i = 0; i < 3; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Color previousGUIColor = GUI.color;

                            string title = Regex.Replace(dictionaryKeys[index], "([a-z])([A-Z])", "$1 $2");

                            var buttonContent = new GUIContent()
                            {
                                text = " " + title,
                                image = icons[index]
                            };

                            if (instance.AnchorLocation.ToString().StartsWith(dictionaryKeys[index]))
                            {
                                GUI.color = Color.cyan;
                            }

                            if (GUILayout.Button(buttonContent))
                            {
                                ChangeDepthLevel(dictionaryKeys[index], depthLevelDictionary[dictionaryKeys[index]]);
                            }

                            GUI.color = previousGUIColor;
                            index++;
                        }
                    }
                }

                EditorGUI.EndDisabledGroup();
            }

            GUI.enabled = true;

            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Anchor Position Smoothing", "Anchor Position Smoothing", MixedRealityStylesUtility.BoldFoldoutStyle, false))
            {
                SerializedProperty smoothing = anchorPositionSmoothing.FindPropertyRelative("smoothing");
                SerializedProperty lerpTime = anchorPositionSmoothing.FindPropertyRelative("lerpTime");

                DrawHorizontalToggleSlider(smoothing, "Use Anchor Position Smoothing", lerpTime, "Lerp Time", 5);
            }
        }

        private void DrawHorizontalToggleSlider(SerializedProperty boolProperty, string boolTitle, SerializedProperty floatProperty, string floatTitle, float sliderMax)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Color previousGUIColor = GUI.color;

                if (boolProperty.boolValue)
                {
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button(boolTitle, GUILayout.MinHeight(40)))
                {
                    boolProperty.boolValue = !boolProperty.boolValue;
                }

                GUI.color = previousGUIColor;

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField(floatTitle);
                    floatProperty.floatValue = EditorGUILayout.Slider("", floatProperty.floatValue, 0, sliderMax);
                }
            }
        }

        private void DrawChildTransformSection()
        {
            InspectorUIUtility.DrawTitle("Child Transforms");

            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Child Volume Transforms", "Child Volume Transforms", MixedRealityStylesUtility.BoldFoldoutStyle, false))
            {
                EditorGUILayout.Space();

                for (int i = 0; i < childVolumeItems.arraySize; i++)
                {
                    SerializedProperty childVolumeItem = childVolumeItems.GetArrayElementAtIndex(i);
                    SerializedProperty transform = childVolumeItem.FindPropertyRelative("transform");
                    SerializedProperty maintainScale = childVolumeItem.FindPropertyRelative("maintainScale");
                    SerializedProperty scaleToLock = childVolumeItem.FindPropertyRelative("scaleToLock");

                    using (new EditorGUI.IndentLevelScope())
                    {
                        Color previousGUIColor = GUI.color;

                        if (childVolumeItem != null)
                        {
                            if (maintainScale.boolValue)
                            {
                                GUI.color = Color.cyan;
                            }

                            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                Transform transformRef = transform.objectReferenceValue as Transform;

                                GUI.enabled = false;
                                EditorGUILayout.PropertyField(transform);
                                EditorGUILayout.PropertyField(scaleToLock);
                                GUI.enabled = true;

                                EditorGUILayout.PropertyField(maintainScale);
                            }

                            EditorGUILayout.Space();

                            GUI.color = previousGUIColor;
                        }
                    }
                }
            }
        }

        private void DrawBackPlateSection()
        {
            if (instance.IsRootUIVolume)
            {
                InspectorUIUtility.DrawTitle("Root Back Plate");

                EditorGUILayout.PropertyField(backPlateObject);
            }
        }

        private void DrawDistributeButtons()
        {
            var xAxisDistributeButtonContent = new GUIContent()
            {
                text = " X Axis Distribute",
                image = xAxisDistributeIcon
            };

            var yAxisDistributeButtonContent = new GUIContent()
            {
                text = " Y Axis Distribute",
                image = yAxisDistributeIcon
            };

            var zAxisDistributeButtonContent = new GUIContent()
            {
                text = " Z Axis Distribute",
                image = zAxisDistributeIcon
            };

            var xAxisDistributeFillXButtonContent = new GUIContent()
            {
                text = " X Axis Container Fill X",
                image = xAxisDistributeFillXIcon
            };

            var xAxisDistributeFillYButtonContent = new GUIContent()
            {
                text = " X Axis Container Fill Y",
                image = xAxisDistributeFillYIcon
            };

            var xAxisDistributeFillZButtonContent = new GUIContent()
            {
                text = " X Axis Container Fill Z",
            };

            var yAxisDistributeFillXButtonContent = new GUIContent(){ text = " Y Axis Container Fill X",};
            var yAxisDistributeFillYButtonContent = new GUIContent(){ text = " Y Axis Container Fill Y",};
            var yAxisDistributeFillZButtonContent = new GUIContent(){ text = " Y Axis Container Fill Z",};

            var zAxisDistributeFillXButtonContent = new GUIContent() { text = " Z Axis Container Fill X", };
            var zAxisDistributeFillYButtonContent = new GUIContent() { text = " Z Axis Container Fill Y", };
            var zAxisDistributeFillZButtonContent = new GUIContent() { text = " Z Axis Container Fill Z", };


            InspectorUIUtility.DrawTitle("Distribute Child Transforms");

            Color previousGUIColor = GUI.color;

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    // X Axis Distribution 
                    if (xAxisDynamicDistribute.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }

                    if (GUILayout.Button(xAxisDistributeButtonContent, GUILayout.MinHeight(50), GUILayout.MinWidth(190)))
                    {
                        if (xAxisDynamicDistribute.boolValue)
                        {
                            xAxisDynamicDistribute.boolValue = false;
                        }
                        else
                        {
                            xAxisDynamicDistribute.boolValue = true;
                            yAxisDynamicDistribute.boolValue = false;
                            zAxisDynamicDistribute.boolValue = false;
                        }
                    }

                    GUI.color = previousGUIColor;

                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField($"Left Margin Percentage");
                        leftMargin.floatValue = EditorGUILayout.Slider("", leftMargin.floatValue, 0, 1); 
                    }
                }

                if (xAxisDynamicDistribute.boolValue)
                {
                    DrawDistributeContainerFill(Axis.X, xAxisDistributeFillXButtonContent, xAxisDistributeFillYButtonContent, xAxisDistributeFillZButtonContent, distributeContainerFillX);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    // Y Axis Distribution 
                    if (yAxisDynamicDistribute.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }
                    if (GUILayout.Button(yAxisDistributeButtonContent, GUILayout.MinHeight(40), GUILayout.MinWidth(190)))
                    {
                        if (yAxisDynamicDistribute.boolValue)
                        {
                            yAxisDynamicDistribute.boolValue = false;
                        }
                        else
                        {
                            yAxisDynamicDistribute.boolValue = true;
                            xAxisDynamicDistribute.boolValue = false;
                            zAxisDynamicDistribute.boolValue = false;
                        }
                    }

                    GUI.color = previousGUIColor;

                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField($"Top Margin Percentage");
                        topMargin.floatValue = EditorGUILayout.Slider("", topMargin.floatValue, 0, 1);
                    }
                }

                if (yAxisDynamicDistribute.boolValue)
                {
                    DrawDistributeContainerFill(Axis.Y, yAxisDistributeFillXButtonContent, yAxisDistributeFillYButtonContent, yAxisDistributeFillZButtonContent, distributeContainerFillY);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    // Z Axis Distribution 
                    if (zAxisDynamicDistribute.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }

                    if (GUILayout.Button(zAxisDistributeButtonContent, GUILayout.MinHeight(40), GUILayout.MinWidth(190)))
                    {
                        if (zAxisDynamicDistribute.boolValue)
                        {
                            zAxisDynamicDistribute.boolValue = false;
                        }
                        else
                        {
                            zAxisDynamicDistribute.boolValue = true;
                            xAxisDynamicDistribute.boolValue = false;
                            yAxisDynamicDistribute.boolValue = false;
                        }
                    }

                    GUI.color = previousGUIColor;

                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField($"Forward Margin Percentage");
                        forwardMargin.floatValue = EditorGUILayout.Slider("", forwardMargin.floatValue, 0, 1);
                    }
                }

                if (zAxisDynamicDistribute.boolValue)
                {
                    DrawDistributeContainerFill(Axis.Z, zAxisDistributeFillXButtonContent, zAxisDistributeFillYButtonContent, zAxisDistributeFillZButtonContent, distributeContainerFillZ);
                }
            }

            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Distribute Smoothing", "Distribute Smoothing", MixedRealityStylesUtility.BoldFoldoutStyle, false))
            {
                SerializedProperty smoothing = distributeSmoothing.FindPropertyRelative("smoothing");
                SerializedProperty lerpTime = distributeSmoothing.FindPropertyRelative("lerpTime");

                DrawHorizontalToggleSlider(smoothing, "Use Distribute Smoothing", lerpTime, "Lerp Time", 5);
            }
        }

        private void DrawDistributeContainerFill(Axis axis, GUIContent button1, GUIContent button2, GUIContent button3, SerializedProperty distributeFillContainer)
        {
            SerializedProperty containerFillX = distributeFillContainer.FindPropertyRelative("containerFillX");
            SerializedProperty containerFillY = distributeFillContainer.FindPropertyRelative("containerFillY");
            SerializedProperty containerFillZ = distributeFillContainer.FindPropertyRelative("containerFillZ");

            using (new EditorGUI.IndentLevelScope())
            {
                if (InspectorUIUtility.DrawSectionFoldoutWithKey($"{axis} Axis Distribute Container Fill", $"{axis} Axis Distribute Container Fill", MixedRealityStylesUtility.BoldFoldoutStyle, false))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        DrawColorToggleButton(containerFillX, button1, 50, 190);
                        DrawColorToggleButton(containerFillY, button2, 50, 190);
                        DrawColorToggleButton(containerFillZ, button3, 50, 150);
                    }

                    EditorGUILayout.Space();
                }
            }
        }

        private void DrawColorToggleButton(SerializedProperty boolProperty, GUIContent buttonContent, int minHeight, int minWidth)
        {
            Color previousGUIColor = GUI.color;

            // X Axis Distribution 
            if (boolProperty.boolValue)
            {
                GUI.color = Color.cyan;
            }

            if (GUILayout.Button(buttonContent, GUILayout.MinHeight(minHeight), GUILayout.MinWidth(minWidth)))
            {
                boolProperty.boolValue = !boolProperty.boolValue;
            }

            GUI.color = previousGUIColor;
        }

        private void DrawDebuggingSection()
        {
            InspectorUIUtility.DrawTitle("Debugging Settings");

            var cornerPointsContent = new GUIContent()
            {
                text = "Display Corner Points",
                image = cornerPointsIcon
            };

            var facePointsContent = new GUIContent()
            {
                text = " Display Face Points",
                image = facePointsIcon
            };

            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Debugging Setting", "Debugging Setting", MixedRealityStylesUtility.BoldFoldoutStyle, false))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        DrawColorToggleButton(drawCornerPoints, cornerPointsContent, 40, 190);
                        DrawColorToggleButton(drawFacePoints, facePointsContent, 40, 190);
                    }

                    EditorGUILayout.Space();

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.Vector3Field("Lossy Scale", instance.transform.lossyScale);
                    EditorGUILayout.Vector3Field("Local Scale", instance.transform.localScale);
                    EditorGUILayout.Vector3Field("Size in cm", instance.transform.lossyScale * 100);
                    EditorGUILayout.Vector3Field("Volume Size Axis Distances", new Vector3(instance.GetAxisDistance(Axis.X), instance.GetAxisDistance(Axis.Y), instance.GetAxisDistance(Axis.Z)));
                    
                    if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.TextMeshPro)
                    {
                        RectTransform rect = instance.transform as RectTransform;

                        EditorGUILayout.Vector3Field("Lossy * Rect Width/Height", new Vector3(instance.transform.lossyScale.x * rect.rect.width, instance.transform.lossyScale.y * rect.rect.height, 0));
                    }

                    if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.ColliderBounds)
                    {
                        Vector3 colliderBounds = instance.transform.GetColliderBounds().size;

                        EditorGUILayout.Vector3Field("Collider Bounds Size", colliderBounds);
                    }

                    EditorGUI.EndDisabledGroup();
                } 
            }
        }

        private void ChangeDepthLevel(string locationName, int depthLevel)
        {
            instance.AnchorLocation = GetAnchorLocation(locationName, depthLevel);

            if (depthLevelDictionary[locationName] != 2)
            {
                depthLevelDictionary[locationName]++;
            }
            else
            {
                depthLevelDictionary[locationName] = 0;
            }
        }

        private AnchorLocation GetAnchorLocation(string locationName, int depthLevel)
        {
            string depthLevelName = GetDepthLevelName(depthLevel);

            int index = Array.IndexOf(anchorLocations, locationName + depthLevelName);

            instance.ChangeAnchorLocation((AnchorLocation)index);

            return (AnchorLocation)index;
        }

        private string GetDepthLevelName(int depthLevel)
        {
            if (depthLevel == 0)
            {
                return "Forward";
            }
            else if (depthLevel == 1)
            {
                return "Center";
            }
            else 
            {
                return "Back";
            }
        }

        private void DrawCommonContainerSizeSection()
        {
            if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.LocalScale)
            {
                InspectorUIUtility.DrawTitle("Volume Size Presets");

                if (InspectorUIUtility.DrawSectionFoldoutWithKey("Volume Size Presets", "Volume Size Presets", MixedRealityStylesUtility.BoldFoldoutStyle, false))
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        DrawSizePresetSection("Button Size Presets", buttonSizePresetsRow1, buttonSizePresetsRow2);
                        DrawSizePresetSection("Button Group Presets", buttonGroupPresets);
                        DrawSizePresetSection("Action Bar Presets", actionBarPresets);
                        DrawSizePresetSection("Dialog Size Presets", dialogPresetsRow1, dialogPresetsRow2);
                        DrawSizePresetSection("List Menu Presets", listMenuPresets);
                        DrawSizePresetSection("Menu List Presets", menuListPresets);
                    }
                }
            }
        }

        private void DrawSizePresetSection(string title, Vector3[] list1, Vector3[] list2 = null)
        {
            if (InspectorUIUtility.DrawSectionFoldoutWithKey(title, title, MixedRealityStylesUtility.BoldFoldoutStyle, false))
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.Space();

                    DrawSizePresetRow(list1);

                    if (list2 != null)
                    {
                        DrawSizePresetRow(list2);
                    }

                    EditorGUILayout.Space();
                }
            }
        }

        private void DrawSizePresetRow(Vector3[] sizePresets)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (Vector3 sizePreset in sizePresets)
                {
                    if (GUILayout.Button(GenerateMMLabel(sizePreset), GUILayout.MinHeight(30)))
                    {
                        instance.transform.localScale = sizePreset;
                    }
                }
            }
        }

        private string GenerateMMLabel(Vector3 sizePreset)
        {
            // Convert to millimeters
            string xValue = (sizePreset.x * 1000).ToString();
            string yValue = (sizePreset.y * 1000).ToString();
            string zValue = (sizePreset.z * 1000).ToString();

            return xValue + "mm x " + yValue + "mm x " + zValue + "mm";
        }
    }
}
