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
    /// <summary>
    /// Custom inspector for the Volume component.  Contains anchor positioning buttons. 
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VolumeAnchorPosition))]
    public class VolumeAnchorPositionInspector : UnityEditor.Editor
    {
        private VolumeAnchorPosition instanceVolume;

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
        private SerializedProperty useScaleConversion;
        private SerializedProperty scaleConversion;

        private Texture fillParentXIcon;
        private Texture fillParentYIcon;
        private Texture fillParentZIcon;

        private const string VolumeSizeSettingsTitle = "Volume Size Settings";
        private const string AnchorPositionTitle = "Anchor Position";
        private const string AnchorPositionSmoothingTitle = "Anchor Position Smooting";
        private const string ScaleConversionTitle = "Scale Conversion";
        private const string UseAnchorPositioningLabel = "Use Anchor Positioning";
        private const string UseFreePositioningLabel = "Use Free Positioning";
        private const string MatchXParentSizeLabel = " Match X Parent Size";
        private const string MatchYParentSizeLabel = " Match Y Parent Size";
        private const string MatchZParentSizeLabel = " Match Z Parent Size";

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

        private readonly string TopLeftIconGUID = "4c129bd16c9d111409c8ca1eb11889bd";
        private readonly string TopCenterIconGUID = "34ed2e23c71b6d444a1d664a4b59e032";
        private readonly string TopRightIconGUID = "0146b743fc7177643b379411da35bc35";
        private readonly string CenterLeftIconGUID = "f222563fc7587964e9c0403d9f62e348";
        private readonly string CenterCenterIconGUID = "03f48c0833278ff46b899b16bb2dd1ba";
        private readonly string CenterRightIconGUID = "20c8993d3ae4af44f8d99e37f81d7912";
        private readonly string BottomLeftIconGUID = "787777b236fbb0740941b2605c484e64";
        private readonly string BottomCenterIconGUID = "eed0cbd2c1e63174ca47df9aba54d7a9";
        private readonly string BottomRightIconGUID = "e007ca13973357644b55c16becc814f6";

        private readonly string FillParentXIconGUID = "9ae151632bec42c4b80a3fa0e8bcb2e6";
        private readonly string FillParentYIconGUID = "6f5c6db74d911fe409df1179c7408d25";
        private readonly string FillParentZIconGUID = "30f1a24121ac62e4dacf6dce03e0f1b8";

        private string[] anchorGUIDs;

        private string[] dictionaryKeys;

        public void OnEnable()
        {
            instanceVolume = target as VolumeAnchorPosition;

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
            scaleConversion = serializedObject.FindProperty("scaleConversion");
            useScaleConversion = serializedObject.FindProperty("useScaleConversion");

            dictionaryKeys = depthLevelDictionary.Keys.ToArray();
        }

        public override void OnInspectorGUI()
        {
            GetAnchorIcons();

            serializedObject.Update();

            if (!instanceVolume.Volume.IsRootVolume)
            {
                DrawAnchorButtons();

                EditorGUILayout.Space();

                DrawAnchorPositionSmoothingSection();

                EditorGUILayout.Space();

                DrawAnchorSizeSection();

                EditorGUILayout.Space();

                DrawScaleConversionSection();
            }
            else
            {
                EditorGUILayout.HelpBox($"The current game object is the root volume.  The Volume Anchor Position component places this transform relative to the parent volume. " +
                    $"Please attach the Volume Anchor Position script to a game object that has the Base Volume component attached to the parent.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (!Application.isPlaying)
            {
                instanceVolume.UpdateSizingBehaviors();
            }
        }

        #region Draw Volume Sections

        private void DrawUseAnchorPositioning()
        {
            Color previousGUIColor = GUI.color;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (useAnchorPositioning.boolValue)
                {
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button(UseAnchorPositioningLabel))
                {
                    useAnchorPositioning.boolValue = true;
                }

                GUI.color = previousGUIColor;

                if (!useAnchorPositioning.boolValue)
                {
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button(UseFreePositioningLabel))
                {
                    useAnchorPositioning.boolValue = false;
                }

                GUI.color = previousGUIColor;
            }

            EditorGUILayout.Space();
        }

        private void DrawAnchorSizeSection()
        {
            VolumeInspectorUtility.DrawTitle(VolumeSizeSettingsTitle);

            var fillToParentXContent = new GUIContent()
            {
                text = MatchXParentSizeLabel,
                image = fillParentXIcon
            };

            var fillToParentYContent = new GUIContent()
            {
                text = MatchYParentSizeLabel,
                image = fillParentYIcon
            };

            var fillToParentZContent = new GUIContent()
            {
                text = MatchZParentSizeLabel,
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
                instanceVolume.EqualizeVolumeSizeToParent();
            }
        }

        private void DrawScaleConversionSection()
        {
            VolumeInspectorUtility.DrawTitle(ScaleConversionTitle);

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey(ScaleConversionTitle, ScaleConversionTitle, false))
            {
                EditorGUILayout.PropertyField(useScaleConversion);
                EditorGUILayout.PropertyField(scaleConversion);
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
            VolumeInspectorUtility.DrawTitle(AnchorPositionTitle);

            DrawUseAnchorPositioning();

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

                            string anchorPositionName = GetCurrentAnchorPositionName();

                            if (anchorPositionName.StartsWith(dictionaryKeys[index]))
                            {
                                GUI.color = Color.cyan;
                            }

                            if (GUILayout.Button(buttonContent))
                            {
                                ChangeDepthLevel(depthLevelDictionary[dictionaryKeys[index]], dictionaryKeys[index]);
                            }

                            GUI.color = previousGUIColor;
                            index++;
                        }
                    }
                }

                EditorGUI.EndDisabledGroup();
            }

            GUI.enabled = true;
        }

        private void DrawAnchorPositionSmoothingSection()
        {
            VolumeInspectorUtility.DrawTitle(AnchorPositionSmoothingTitle);

            if (VolumeInspectorUtility.DrawSectionFoldoutWithKey(AnchorPositionSmoothingTitle, AnchorPositionSmoothingTitle, false))
            {
                SerializedProperty smoothing = anchorPositionSmoothing.FindPropertyRelative("smoothing");
                SerializedProperty lerpTime = anchorPositionSmoothing.FindPropertyRelative("lerpTime");

                VolumeInspectorUtility.DrawHorizontalToggleSlider(smoothing, "Use Anchor Position Smoothing", lerpTime, "Lerp Time", 5);
            }
        }

        private void ChangeDepthLevel(int depthLevel, string locationName)
        {
            // Z Anchor Position
            instanceVolume.ZAnchorPosition = (ZAnchorPosition)depthLevel;

            // Y Anchor Position
            if (locationName.StartsWith("Top"))
            {
                instanceVolume.YAnchorPosition = YAnchorPosition.Top;
            }
            else if (locationName.StartsWith("Center"))
            {
                instanceVolume.YAnchorPosition = YAnchorPosition.Center;
            }
            else if (locationName.StartsWith("Bottom"))
            {
                instanceVolume.YAnchorPosition = YAnchorPosition.Bottom;
            }

            // X Anchor Position
            if (locationName.EndsWith("Left"))
            {
                instanceVolume.XAnchorPosition = XAnchorPosition.Left;
            }
            else if (locationName.EndsWith("Center"))
            {
                instanceVolume.XAnchorPosition = XAnchorPosition.Center;
            }
            else if (locationName.EndsWith("Right"))
            {
                instanceVolume.XAnchorPosition = XAnchorPosition.Right;
            }

            instanceVolume.UpdateAnchorPosition();

            if (depthLevelDictionary[locationName] != 2)
            {
                depthLevelDictionary[locationName]++;
            }
            else
            {
                depthLevelDictionary[locationName] = 0;
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

        #endregion

        #region EditorGUI Utils
        
        private void GetAnchorIcons()
        {
            anchorGUIDs = new string[9]
            {
                TopLeftIconGUID,
                TopCenterIconGUID,
                TopRightIconGUID,
                CenterLeftIconGUID,
                CenterCenterIconGUID,
                CenterRightIconGUID,
                BottomLeftIconGUID,
                BottomCenterIconGUID,
                BottomRightIconGUID
            };

            for (int i = 0; i < anchorGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(anchorGUIDs[i]);
                Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(path);

                if (!icons.Contains(icon))
                {
                    icons.Add(icon);
                }
            }

            fillParentXIcon = VolumeInspectorUtility.GetIcon(FillParentXIconGUID);
            fillParentYIcon = VolumeInspectorUtility.GetIcon(FillParentYIconGUID);
            fillParentZIcon = VolumeInspectorUtility.GetIcon(FillParentZIconGUID);
        }

        #endregion
    }
}
