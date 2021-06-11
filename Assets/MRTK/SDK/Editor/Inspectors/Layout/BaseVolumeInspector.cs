// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseVolume))]
    public class BaseVolumeInspector : UnityEditor.Editor
    {
        private BaseVolume instance;

        private SerializedProperty volumeID;
        private SerializedProperty volumeBounds;

        private SerializedProperty marginBounds;

        private SerializedProperty drawCornerPoints;
        private SerializedProperty drawFacePoints;
        private SerializedProperty drawVolumeBounds;
        private SerializedProperty drawMarginBounds;
        private SerializedProperty useMargin;

        private SerializedProperty childVolumeItems;
        private SerializedProperty volumeSizeOrigin;

        private SerializedProperty onChildCountChanged;
        private SerializedProperty onVolumePositionChanged;
        private SerializedProperty onVolumeSizeChanged;
        private SerializedProperty onVolumeScaleChanged;
        private SerializedProperty onVolumeModified;

        private BoxBoundsHandle volumeBoundsHandle = new BoxBoundsHandle();
        private BoxBoundsHandle marginBoundsHandle = new BoxBoundsHandle();

        private const string VolumeBoundsTitle = "Volume Bounds";
        private const string DrawGizmosTitle = "Draw Gizmos";
        private const string ChildTransformsTitle = "Child Volume Transforms";
        private const string VolumeSizePresetsTitle = "Volume Size Presets";
        private const string EventsTitle = "Volume Events";
        private const string MarginBoundsTitle = "Margin Bounds";
        private const string EnableMaintainScaleLabel = "Enable Maintain Scale All";
        private const string DisableMaintainScaleLabel = "Disable Maintain Scale All";
        private const string FacePointsDisplayLabel = " Display Face Points";
        private const string CornerPointsDisplayLabel = " Display Corner Points";
        private const string VolumeBoundsDisplayLabel = " Display Volume Bounds";
        private const string MarginBoundsDisplayLabel = " Display Margin Bounds";
        private const string ButtonSizePresetLabel = "Button Size Presets";
        private const string ButtonGroupPresetLabel = "Button Group Presets";
        private const string ActionBarPresetLabel = "Action Bar Presets";
        private const string DialogSizePresetLabel = "Dialog Size Presets";
        private const string ListMenuPresetLabel = "List Menu Presets";
        private const string MenuListPresetLabel = "Menu List Presets";
        private const string MarginBoundsResetLabel = "Reset Margin Bounds to Volume BoundsSize";
        private const string MarginValuesLabel = "View Margin Values";
        
        private Texture cornerPointsIcon;
        private Texture facePointsIcon;
        private Texture volumeBoundsIcon;

        private readonly string FacePointsIconGUID = "7d524dcfa288d5145a5d25b92440ce7b";
        private readonly string CornerPointsIconGUID = "df9428ac399ba3e4c9d07027789752c4";
        private readonly string VolumeBoundsIconGUID = "44b20929bf465864d8ad1fcc5001d9c5";

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

        public virtual void OnEnable()
        {
            instance = target as BaseVolume;

            volumeID = serializedObject.FindProperty("volumeID");

            volumeBounds = serializedObject.FindProperty("volumeBounds");

            marginBounds = serializedObject.FindProperty("marginBounds");

            drawCornerPoints = serializedObject.FindProperty("drawCornerPoints");
            drawFacePoints = serializedObject.FindProperty("drawFacePoints");
            drawVolumeBounds = serializedObject.FindProperty("drawVolumeBounds");
            drawMarginBounds = serializedObject.FindProperty("drawMarginBounds");
            useMargin = serializedObject.FindProperty("useMargin");

            childVolumeItems = serializedObject.FindProperty("childVolumeItems");
            volumeSizeOrigin = serializedObject.FindProperty("volumeSizeOrigin");

            onChildCountChanged = serializedObject.FindProperty("onChildCountChanged");
            onVolumePositionChanged = serializedObject.FindProperty("onVolumePositionChanged");
            onVolumeSizeChanged = serializedObject.FindProperty("onVolumeSizeChanged");
            onVolumeScaleChanged = serializedObject.FindProperty("onVolumeScaleChanged");
            onVolumeModified = serializedObject.FindProperty("onVolumeModified");
    }

        public override void OnInspectorGUI()
        {
            GetIcons();

            serializedObject.Update();

            if (instance.IsRootVolume)
            {
                VolumeInspectorUtility.DrawWarning("This Volume is the root");
            }


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(volumeSizeOrigin);

            if (EditorGUI.EndChangeCheck())
            {
                instance.UpdateVolumeBounds();
            }

            DrawVolumeBoundsProperties();

            DrawMarginBoundsSection();

            DrawCommonContainerSizeSection();

            DrawDebuggingSection();

            DrawEventsSection();

            if (instance.ChildVolumeItems.Count != 0)
            {
                DrawChildTransformSection();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        public virtual void OnSceneGUI()
        {
            if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.Custom)
            {
                DrawHandleBounds(volumeBoundsHandle, Color.magenta, instance.VolumeSize);
                instance.DrawVolumeBounds = false;
            }
        }

        #region Editor Volume Resize Handles
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

                    instance.OnVolumeSizeChanged.Invoke();
                }
            }
        }

        private Vector3 SetHandleSize(BoxBoundsHandle handle)
        {
            Vector3 newSize = Vector3.zero;

            newSize = handle.size;

            instance.VolumeBounds.Size = newSize;

            SerializedProperty boundsSize = volumeBounds.FindPropertyRelative("size");

            boundsSize.vector3Value = newSize;
     
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

        #endregion

        #region Draw Volume Sections
        private void DrawVolumeBoundsProperties()
        {
            VolumeInspectorUtility.DrawTitle(VolumeBoundsTitle);

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(volumeSizeOrigin.enumValueIndex != (int)VolumeSizeOrigin.Custom);

            DrawVolumeBoundsProperty(volumeBounds);

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
        }


        private void DrawMarginBoundsSection()
        {
            VolumeInspectorUtility.DrawTitle(MarginBoundsTitle);

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey(MarginBoundsTitle, MarginBoundsTitle, false))
            {

                EditorGUILayout.PropertyField(useMargin);

                EditorGUI.BeginDisabledGroup(!useMargin.boolValue);


                DrawMarginBoundsProperty(marginBounds);

                if (GUILayout.Button(MarginBoundsResetLabel))
                {
                    ResetMarginBoundsToVolumeBounds();
                }

                if (VolumeInspectorUtility.DrawSectionFoldoutWithKey(MarginValuesLabel, MarginValuesLabel, false))
                {
                    EditorGUI.BeginDisabledGroup(volumeSizeOrigin.enumValueIndex != (int)VolumeSizeOrigin.Custom);

                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.FloatField("Margin Left cm", instance.GetMarginDifference().x * 0.5f);
                            EditorGUILayout.FloatField("Margin Right cm", instance.GetMarginDifference().x * 0.5f);

                            EditorGUILayout.Space();

                            EditorGUILayout.FloatField("Margin Top cm", instance.GetMarginDifference().y * 0.5f);
                            EditorGUILayout.FloatField("Margin Bottom cm", instance.GetMarginDifference().y * 0.5f);

                            EditorGUILayout.Space();

                            EditorGUILayout.FloatField("Margin Forward cm", instance.GetMarginDifference().z * 0.5f);
                            EditorGUILayout.FloatField("Margin Back cm", instance.GetMarginDifference().z * 0.5f);
                        }
                    }

                    EditorGUI.EndDisabledGroup();
                }

                EditorGUI.EndDisabledGroup();
            }
        }

        private void ResetMarginBoundsToVolumeBounds()
        {
            instance.MarginBounds.Size = instance.VolumeBounds.Size;
        }

        private void DrawVolumeBoundsProperty(SerializedProperty volumeBounds)
        {

            EditorGUI.BeginChangeCheck();
            SerializedProperty volumeSize = volumeBounds.FindPropertyRelative("size");
            if (EditorGUI.EndChangeCheck())
            {
                instance.OnVolumeModified.Invoke();
            }

            SerializedProperty volumeCenter = volumeBounds.FindPropertyRelative("center");
            SerializedProperty volumeHostTransform = volumeBounds.FindPropertyRelative("hostTransform");

            EditorGUILayout.PropertyField(volumeSize);
            EditorGUILayout.PropertyField(volumeCenter);
            EditorGUILayout.PropertyField(volumeHostTransform);

            Vector3 extents = instance.VolumeBounds.Extents;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Vector3Field("Extents", extents);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawMarginBoundsProperty(SerializedProperty volumeBounds)
        {
            SerializedProperty volumeSize = volumeBounds.FindPropertyRelative("size");

            EditorGUILayout.PropertyField(volumeSize);
        }

        private void SetMaintainScaleAll(bool isMaintainScaleEnabled)
        {
            for (int i = 0; i < childVolumeItems.arraySize; i++)
            {
                SerializedProperty childVolumeItem = childVolumeItems.GetArrayElementAtIndex(i);
                SerializedProperty maintainScale = childVolumeItem.FindPropertyRelative("maintainScale");

                maintainScale.boolValue = isMaintainScaleEnabled;
            }
        }

        private void DrawChildTransformSection()
        {
            VolumeInspectorUtility.DrawTitle(ChildTransformsTitle);

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey(ChildTransformsTitle, ChildTransformsTitle, false))
            {
                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(EnableMaintainScaleLabel))
                    {
                        SetMaintainScaleAll(true);
                    }

                    if (GUILayout.Button(DisableMaintainScaleLabel))
                    {
                        SetMaintainScaleAll(false);
                    }
                }

                EditorGUILayout.Space();

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

                                Transform itemTransform = transform.objectReferenceValue as Transform;

                                EditorGUILayout.Vector3Field("Position", itemTransform.position);
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

        private void DrawDebuggingSection()
        {
            VolumeInspectorUtility.DrawTitle(DrawGizmosTitle);

            var cornerPointsContent = new GUIContent()
            {
                text = CornerPointsDisplayLabel,
                image = cornerPointsIcon
            };

            var facePointsContent = new GUIContent()
            {
                text = FacePointsDisplayLabel,
                image = facePointsIcon
            };

            var volumeBoundsContent = new GUIContent()
            {
                text = VolumeBoundsDisplayLabel,
                image = volumeBoundsIcon
            };

            var marginBoundsContent = new GUIContent()
            {
                text = MarginBoundsDisplayLabel,
            };

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey(DrawGizmosTitle, DrawGizmosTitle, false))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        VolumeInspectorUtility.DrawColorToggleButton(drawCornerPoints, cornerPointsContent, 30, 160);
                        VolumeInspectorUtility.DrawColorToggleButton(drawFacePoints, facePointsContent, 30, 160);
                        VolumeInspectorUtility.DrawColorToggleButton(drawVolumeBounds, volumeBoundsContent, 30, 160);
                        VolumeInspectorUtility.DrawColorToggleButton(drawMarginBounds, marginBoundsContent, 65, 160);
                    }

                    EditorGUILayout.Space();

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(volumeID);
                    EditorGUILayout.Vector3Field("Lossy Scale", instance.transform.lossyScale);
                    EditorGUILayout.Vector3Field("Local Scale", instance.transform.localScale);
                    EditorGUILayout.Vector3Field("Size in cm", instance.transform.lossyScale * 100);
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        private void DrawCommonContainerSizeSection()
        {
            if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.LocalScale || volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.LossyScale)
            {
                VolumeInspectorUtility.DrawTitle(VolumeSizePresetsTitle);

                if (VolumeInspectorUtility.DrawSectionFoldoutWithKey(VolumeSizePresetsTitle, VolumeSizePresetsTitle, false))
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        DrawSizePresetSection(ButtonSizePresetLabel, buttonSizePresetsRow1, buttonSizePresetsRow2);
                        DrawSizePresetSection(ButtonGroupPresetLabel, buttonGroupPresets);
                        DrawSizePresetSection(ActionBarPresetLabel, actionBarPresets);
                        DrawSizePresetSection(DialogSizePresetLabel, dialogPresetsRow1, dialogPresetsRow2);
                        DrawSizePresetSection(ListMenuPresetLabel, listMenuPresets);
                        DrawSizePresetSection(MenuListPresetLabel, menuListPresets);
                    }
                }
            }
        }


        private void DrawEventsSection()
        {
            VolumeInspectorUtility.DrawTitle(EventsTitle);

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey(EventsTitle, EventsTitle, false))
            {
                EditorGUILayout.PropertyField(onChildCountChanged);
                EditorGUILayout.PropertyField(onVolumePositionChanged);
                EditorGUILayout.PropertyField(onVolumeSizeChanged);
                EditorGUILayout.PropertyField(onVolumeScaleChanged);
                EditorGUILayout.PropertyField(onVolumeModified);
            }
         }

        private void DrawSizePresetSection(string title, Vector3[] list1, Vector3[] list2 = null)
        {
            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey(title, title, false))
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


        #endregion

        #region Utils

        public void GetIcons()
        {
            cornerPointsIcon = VolumeInspectorUtility.GetIcon(CornerPointsIconGUID);
            facePointsIcon = VolumeInspectorUtility.GetIcon(FacePointsIconGUID);
            volumeBoundsIcon = VolumeInspectorUtility.GetIcon(VolumeBoundsIconGUID);
        }

        #endregion
    }
}
