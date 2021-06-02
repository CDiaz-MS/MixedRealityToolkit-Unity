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
        private SerializedProperty childVolumeItems;
        private SerializedProperty volumeSizeOrigin;

        private Texture cornerPointsIcon;
        private Texture facePointsIcon;
        private Texture volumeBoundsIcon;

        private BoxBoundsHandle volumeBoundsHandle = new BoxBoundsHandle();
        private BoxBoundsHandle marginBoundsHandle = new BoxBoundsHandle();

        private const string VolumeBoundsTitle = "Volume Bounds";
        private const string DrawGizmosTitle = "Draw Gizmos";
        private const string ChildTransformsTitle = "Child Volume Transforms";
        private const string VolumeSizePresetsTitle = "Volume Size Presets";
        private const string EnableMaintainScaleLabel = "Enable Maintain Scale All";
        private const string DisableMaintainScaleLabel = "Disable Maintain Scale All";
        private const string FacePointsDisplayLabel = " Display Face Points";
        private const string CornerPointsDisplayLabel = " Display Corner Points";
        private const string VolumeBoundsDisplayLabel = " Display Volume Bounds";
        private const string ButtonSizePresetLabel = "Button Size Presets";
        private const string ButtonGroupPresetLabel = "Button Group Presets";
        private const string ActionBarPresetLabel = "Action Bar Presets";
        private const string DialogSizePresetLabel = "Dialog Size Presets";
        private const string ListMenuPresetLabel = "List Menu Presets";
        private const string MenuListPresetLabel = "Menu List Presets";

        private Color warningColor = new Color(1f, 0.85f, 0.6f);

        protected const int TitleFontSize = 14;

        private readonly string FacePointsIconGUID = "7d524dcfa288d5145a5d25b92440ce7b";
        private readonly string CornerPointsIconGUID = "df9428ac399ba3e4c9d07027789752c4";
        private readonly string VolumeBoundsIconGUID = "df9428ac399ba3e4c9d07027789752c4";

        private static readonly Color ProfessionalThemeColorTint100 = new Color(0f, 0f, 0f);
        private static readonly Color ProfessionalThemeColorTint75 = new Color(0.25f, 0.25f, 0.25f);
        private static readonly Color ProfessionalThemeColorTint50 = new Color(0.5f, 0.5f, 0.5f);

        private static readonly Color PersonalThemeColorTint100 = new Color(1f, 1f, 1f);
        private static readonly Color PersonalThemeColorTint75 = new Color(0.75f, 0.75f, 0.75f);
        private static readonly Color PersonalThemeColorTint50 = new Color(0.5f, 0.5f, 0.5f);

        public Color ColorTint100 => EditorGUIUtility.isProSkin ? ProfessionalThemeColorTint100 : PersonalThemeColorTint100;
        public Color ColorTint75 => EditorGUIUtility.isProSkin ? ProfessionalThemeColorTint75 : PersonalThemeColorTint75;
        public Color ColorTint50 => EditorGUIUtility.isProSkin ? ProfessionalThemeColorTint50 : PersonalThemeColorTint50;

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
            childVolumeItems = serializedObject.FindProperty("childVolumeItems");
            volumeSizeOrigin = serializedObject.FindProperty("volumeSizeOrigin");
        }

        public override void OnInspectorGUI()
        {
            GetIcons();

            serializedObject.Update();

            if (instance.IsRootUIVolume)
            {
                DrawWarning("This Volume is the root");
            }

            EditorGUILayout.PropertyField(volumeSizeOrigin);

            DrawVolumeBoundsProperties();

            DrawCommonContainerSizeSection();

            DrawDebuggingSection();

            DrawChildTransformSection();

            serializedObject.ApplyModifiedProperties();
        }

        public virtual void OnSceneGUI()
        {
            if (volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.Custom)
            {
                DrawHandleBounds(volumeBoundsHandle, Color.magenta, instance.VolumeSize);
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

                SerializedProperty boundsSize = volumeBounds.FindPropertyRelative("size");

                boundsSize.vector3Value = newSize;
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

        #endregion

        #region Draw Volume Sections
        private void DrawVolumeBoundsProperties()
        {
            DrawTitle(VolumeBoundsTitle);

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

        private void DrawVolumeBoundsProperty(SerializedProperty volumeBounds)
        {
            SerializedProperty volumeSize = volumeBounds.FindPropertyRelative("size");
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
            DrawTitle(ChildTransformsTitle);

            if (DrawSectionFoldoutWithKey(ChildTransformsTitle, ChildTransformsTitle, false))
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
            DrawTitle(DrawGizmosTitle);

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

            if (DrawSectionFoldoutWithKey(DrawGizmosTitle, DrawGizmosTitle, false))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        DrawColorToggleButton(drawCornerPoints, cornerPointsContent, 40, 190);
                        DrawColorToggleButton(drawFacePoints, facePointsContent, 40, 190);
                        DrawColorToggleButton(drawVolumeBounds, volumeBoundsContent, 40, 190);
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
                DrawTitle(VolumeSizePresetsTitle);

                if (DrawSectionFoldoutWithKey(VolumeSizePresetsTitle, VolumeSizePresetsTitle, false))
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

        private void DrawSizePresetSection(string title, Vector3[] list1, Vector3[] list2 = null)
        {
            if (DrawSectionFoldoutWithKey(title, title, false))
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

        #region EditorGUI Utils
        protected void DrawTitle(string title)
        {
            GUIStyle labelStyle = LableStyle(TitleFontSize, ColorTint50);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
            EditorGUILayout.Space();
        }

        protected void DrawWarning(string warning)
        {
            Color prevColor = GUI.color;

            GUI.color = warningColor;
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField(warning, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();

            GUI.color = prevColor;
        }

        protected GUIStyle LableStyle(int size, Color color)
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = size;
            labelStyle.fixedHeight = size * 2;
            labelStyle.normal.textColor = color;
            return labelStyle;
        }

        private void GetIcons()
        {
            cornerPointsIcon = GetIcon(CornerPointsIconGUID);
            facePointsIcon = GetIcon(FacePointsIconGUID);
            volumeBoundsIcon = GetIcon(VolumeBoundsIconGUID);
        }

        protected Texture GetIcon(string GUID)
        {
            string path = AssetDatabase.GUIDToAssetPath(GUID);
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(path);
            return icon;
        }

        protected void DrawHorizontalToggleSlider(SerializedProperty boolProperty, string boolTitle, SerializedProperty floatProperty, string floatTitle, float sliderMax)
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

        protected bool DrawSectionFoldoutWithKey(string headerName, string preferenceKey = null, bool defaultOpen = true)
        {
            GUIStyle style = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
            bool showPref = SessionState.GetBool(preferenceKey, defaultOpen);
            bool show = DrawSectionFoldout(headerName, showPref, style);
            if (show != showPref)
            {
                SessionState.SetBool(preferenceKey, show);
            }

            return show;
        }

        protected bool DrawSectionFoldout(string headerName, bool open = true, GUIStyle style = null)
        {
            if (style == null)
            {
                style = EditorStyles.foldout;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                return EditorGUILayout.Foldout(open, headerName, true, style);
            }
        }

        protected bool DrawColorToggleButton(SerializedProperty boolProperty, GUIContent buttonContent, int minHeight, int minWidth)
        {
            Color previousGUIColor = GUI.color;

            if (boolProperty.boolValue)
            {
                GUI.color = Color.cyan;
            }

            if (GUILayout.Button(buttonContent, GUILayout.MinHeight(minHeight), GUILayout.MinWidth(minWidth)))
            {
                boolProperty.boolValue = !boolProperty.boolValue;
            }

            GUI.color = previousGUIColor;
            return boolProperty.boolValue;
        }

        #endregion
    }
}
