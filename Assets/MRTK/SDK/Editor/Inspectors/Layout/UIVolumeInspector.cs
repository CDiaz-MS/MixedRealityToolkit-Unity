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
using UnityEditor.IMGUI.Controls;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIVolume))]
    public class UIVolumeInspector : UnityEditor.Editor
    {
        private UIVolume instance;

        private SerializedProperty volumeID;
        private SerializedProperty volumeSize;
        private SerializedProperty volumeBounds;

        private SerializedProperty marginBounds;
        private SerializedProperty marginLeftAndRightMM;
        private SerializedProperty marginTopAndBottomMM;
        private SerializedProperty marginForwardAndBackMM;
        private SerializedProperty editMargin;

        private SerializedProperty anchorPositionSmoothing;
        private SerializedProperty xAnchorPosition;
        private SerializedProperty yAnchorPosition;
        private SerializedProperty zAnchorPosition;

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

        private SerializedProperty size;

        private BoxBoundsHandle volumeBoundsHandle = new BoxBoundsHandle();
        private BoxBoundsHandle marginBoundsHandle = new BoxBoundsHandle();

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

        public virtual void OnSceneGUI()
        {
            if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.Custom)
            {
                DrawHandleBounds(volumeBoundsHandle, Color.magenta, instance.VolumeSize);
            }


            SerializedProperty marginSize = marginBounds.FindPropertyRelative("size");
            
            if (editMargin.boolValue)
            {
                DrawHandleBounds(marginBoundsHandle, Color.red, instance.MarginBounds.Size);
            }
            
        }

        private void DrawHandleBounds(BoxBoundsHandle handle, Color color, Vector3 size)
        { 
            using (new Handles.DrawingScope(color, GetLocalMatrix()))
            {
                handle.center = Vector3.zero;
                handle.size = size;

                DrawHandleDirectionLabels(handle, Color.green);

                EditorGUI.BeginChangeCheck();

                Handles.DrawWireCube(handle.center, handle.size);

                handle.DrawHandle();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(instance, "Layout Item Size");

                    SetHandleSize(handle);
                }
            }
        }

        private Vector3 SetHandleSize(BoxBoundsHandle handle)
        {
            Vector3 newSize = Vector3.zero;

            if (handle == marginBoundsHandle)
            {
                newSize.x = handle.size.x < instance.VolumeSize.x ? instance.VolumeSize.x : handle.size.x;
                newSize.y = handle.size.y < instance.VolumeSize.y ? instance.VolumeSize.y : handle.size.y;
                newSize.z = handle.size.z < instance.VolumeSize.z ? instance.VolumeSize.z : handle.size.z;

                instance.MarginBounds.Size = newSize;
            }
            else
            {
                newSize = handle.size;

                instance.VolumeBounds.Size = newSize;
            }

            return newSize;
        }

        private void DrawHandleDirectionLabels(BoxBoundsHandle handle, Color textColor)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = textColor;

            Handles.Label(handle.center + ((handle.size.x * 0.5f) * Vector3.right), "Right", style);
            Handles.Label(handle.center + ((handle.size.x * 0.5f) * Vector3.left), "Left", style);
            Handles.Label(handle.center + ((handle.size.y * 0.5f) * Vector3.up), "Up", style);
            Handles.Label(handle.center + ((handle.size.y * 0.5f) * Vector3.down), "Down", style);
            Handles.Label(handle.center + ((handle.size.z * 0.5f) * Vector3.forward), "Back", style);
            Handles.Label(handle.center + ((handle.size.z * 0.5f) * Vector3.back), "Forward", style);
        }

        private Matrix4x4 GetLocalMatrix()
        {
            return Matrix4x4.TRS(instance.transform.position, instance.transform.rotation, Vector3.one);
        }

        private Matrix4x4 GetParentMatrix()
        {
            return Matrix4x4.TRS(instance.transform.parent.position, instance.transform.parent.rotation, instance.transform.parent.lossyScale);
        }

        private Matrix4x4 GetMatrix()
        {
            return instance.transform.parent != null ? GetParentMatrix() : GetLocalMatrix();
        }

       
        public virtual void OnEnable()
        {
            instance = target as UIVolume;

            volumeID = serializedObject.FindProperty("volumeID");

            volumeBounds = serializedObject.FindProperty("volumeBounds");

            marginBounds = serializedObject.FindProperty("marginBounds");
            marginLeftAndRightMM = serializedObject.FindProperty("marginLeftAndRightMM");
            marginTopAndBottomMM = serializedObject.FindProperty("marginTopAndBottomMM");
            marginForwardAndBackMM = serializedObject.FindProperty("marginForwardAndBackMM");
            editMargin = serializedObject.FindProperty("editMargin");

            size = volumeBounds.FindPropertyRelative("size");

            anchorPositionSmoothing = serializedObject.FindProperty("anchorPositionSmoothing");
            xAnchorPosition = serializedObject.FindProperty("xAnchorPosition");
            yAnchorPosition = serializedObject.FindProperty("yAnchorPosition");
            zAnchorPosition = serializedObject.FindProperty("zAnchorPosition");

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

                //DrawBackPlateSection();
            }

            DrawVolumeBoundsProperties();

            DrawCommonContainerSizeSection();
            
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

        private void DrawVolumeBoundsProperties()
        {
            EditorGUILayout.PropertyField(volumeSizeOrigin);

            InspectorUIUtility.DrawTitle("Volume Bounds");

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(volumeSizeOrigin.enumValueIndex != (int)VolumeSizeOrigin.Custom);
        
            EditorGUILayout.PropertyField(volumeBounds, true);

            if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.LossyScale || volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.LocalScale)
            {
                EditorGUILayout.HelpBox($"The VolumeSizeOrigin is set to Lossy or Local Scale.  Change the scale of the object to change the size of the volume.  Set the VolumeSizeOrigin to Custom to modify the size of the volume independent from scale.", MessageType.Info);
            }
            else if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.RendererBounds)
            {
                EditorGUILayout.HelpBox($"The VolumeSizeOrigin is set to Renderer Bounds.  Set the VolumeSizeOrigin to Custom to modify the size of the volume independent from scale.", MessageType.Info);
            }
            else if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.ColliderBounds)
            {
                EditorGUILayout.HelpBox($"The VolumeSizeOrigin is set to Collider Bounds.  Set the VolumeSizeOrigin to Custom to modify the size of the volume independent from scale.", MessageType.Info);
            }

            EditorGUI.EndDisabledGroup();            
            
            EditorGUILayout.PropertyField(marginBounds);
            EditorGUILayout.PropertyField(editMargin);

            if (GUILayout.Button("Set Margin to VolumeBounds Size"))
            {
                instance.MarginBounds.Size = instance.VolumeBounds.Size;
                marginBoundsHandle.size = instance.MarginBounds.Size;
            }


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(marginLeftAndRightMM);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(instance, "Undo Margin Left and Right");
                instance.MarginLeftAndRightMM = marginLeftAndRightMM.floatValue;
                marginBoundsHandle.size = instance.MarginBounds.Size;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(marginTopAndBottomMM);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(instance, "Undo Margin Top and Bottom");
                instance.MarginTopAndBottomMM = marginTopAndBottomMM.floatValue;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(marginForwardAndBackMM);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(instance, "Undo Margin Forward and Back");
                instance.MarginForwardAndBackMM = marginForwardAndBackMM.floatValue;
            }
            
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
                DrawVolumeSizeAxisSection(fillToParentZ, volumeSizeScaleFactorZ, "Z", fillToParentZContent);
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

                EditorGUILayout.PropertyField(xAnchorPosition);
                EditorGUILayout.PropertyField(yAnchorPosition);
                EditorGUILayout.PropertyField(zAnchorPosition);

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

                            string anchorPositionName = GetCurrentAnchorPositionName();

                            if (anchorPositionName.StartsWith(dictionaryKeys[index]))
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

        private string GetCurrentAnchorPositionName()
        {
            int yPositionEnumValue = yAnchorPosition.enumValueIndex;
            string yPositionName = ((YAnchorPosition)yPositionEnumValue).ToString();

            int xPositionEnumValue = xAnchorPosition.enumValueIndex;
            string xPositionName = ((XAnchorPosition)xPositionEnumValue).ToString();

            int zPositionEnumValue = zAnchorPosition.enumValueIndex;
            string zPositionName = ((ZAnchorPosition)zPositionEnumValue).ToString();

            string anchorPositionName = yPositionName + xPositionName + zPositionName;

            return anchorPositionName;
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


        private void SetMaintainScaleAll(bool isMaintainScaleEnabled)
        {
            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Child Volume Transforms", "Child Volume Transforms", MixedRealityStylesUtility.BoldFoldoutStyle, false))
            {
                EditorGUILayout.Space();

                for (int i = 0; i < childVolumeItems.arraySize; i++)
                {
                    SerializedProperty childVolumeItem = childVolumeItems.GetArrayElementAtIndex(i);
                    SerializedProperty maintainScale = childVolumeItem.FindPropertyRelative("maintainScale");

                    maintainScale.boolValue = isMaintainScaleEnabled;
                }
            }
        }

        private void DrawChildTransformSection()
        {
            InspectorUIUtility.DrawTitle("Child Transforms");

            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Child Volume Transforms", "Child Volume Transforms", MixedRealityStylesUtility.BoldFoldoutStyle, false))
            {
                EditorGUILayout.Space();

                if (GUILayout.Button("Set Maintain Scale All"))
                {
                    SetMaintainScaleAll(true);
                }

                for (int i = 0; i < childVolumeItems.arraySize; i++)
                {
                    SerializedProperty childVolumeItem = childVolumeItems.GetArrayElementAtIndex(i);
                    SerializedProperty transform = childVolumeItem.FindPropertyRelative("transform");
                    SerializedProperty maintainScale = childVolumeItem.FindPropertyRelative("maintainScale");
                    SerializedProperty scaleToLock = childVolumeItem.FindPropertyRelative("scaleToLock");
                    SerializedProperty uiVolume = childVolumeItem.FindPropertyRelative("uIVolume");

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

                                if (uiVolume != null)
                                {
                                    EditorGUILayout.PropertyField(uiVolume);
                                }
                                
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

        private void DrawDistributeContainerFill(VolumeAxis axis, GUIContent button1, GUIContent button2, GUIContent button3, SerializedProperty distributeFillContainer)
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

                    EditorGUILayout.PropertyField(volumeID);

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(volumeSizeOrigin);
                    EditorGUILayout.Space();

                    EditorGUILayout.Vector3Field("Lossy Scale", instance.transform.lossyScale);
                    EditorGUILayout.Vector3Field("Local Scale", instance.transform.localScale);
                    EditorGUILayout.Vector3Field("Size in cm", instance.transform.lossyScale * 100);
                    EditorGUILayout.Vector3Field("Volume Size Axis Distances", new Vector3(instance.GetAxisDistance(VolumeAxis.X), instance.GetAxisDistance(VolumeAxis.Y), instance.GetAxisDistance(VolumeAxis.Z)));
                    
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
            if (depthLevel == 0)
            {
                instance.ZAnchorPosition = ZAnchorPosition.Forward;
            }
            if (depthLevel == 1)
            {
                instance.ZAnchorPosition = ZAnchorPosition.Center;
            }
            if (depthLevel == 2)
            {
                instance.ZAnchorPosition = ZAnchorPosition.Back;
            }

            if (locationName.StartsWith("Top"))
            {
                instance.YAnchorPosition = YAnchorPosition.Top;
            }
            if (locationName.StartsWith("Center"))
            {
                instance.YAnchorPosition = YAnchorPosition.Center;
            }
            if (locationName.StartsWith("Bottom"))
            {
                instance.YAnchorPosition = YAnchorPosition.Bottom;
            }


            if (locationName.EndsWith("Left"))
            {
                instance.XAnchorPosition = XAnchorPosition.Left;
            }
            if (locationName.EndsWith("Center"))
            {
                instance.XAnchorPosition = XAnchorPosition.Center;
            }
            if (locationName.EndsWith("Right"))
            {
                instance.XAnchorPosition = XAnchorPosition.Right;
            }

            instance.UpdateAnchorPosition();

            if (depthLevelDictionary[locationName] != 2)
            {
                depthLevelDictionary[locationName]++;
            }
            else
            {
                depthLevelDictionary[locationName] = 0;
            }
        }


        private void DrawCommonContainerSizeSection()
        {
            if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.LocalScale || volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.LossyScale)
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
