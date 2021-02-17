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

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(UIVolume))]
    public class UIVolumeInspector : UnityEditor.Editor
    {
        private UIVolume instance;

        private SerializedProperty anchorLocation;

        private SerializedProperty xAxisDynamicDistribute;
        private SerializedProperty yAxisDynamicDistribute;
        private SerializedProperty zAxisDynamicDistribute;
        private SerializedProperty leftMargin;
        private SerializedProperty topMargin;
        private SerializedProperty forwardMargin;

        private SerializedProperty fillToParentX;
        private SerializedProperty fillToParentY;
        private SerializedProperty fillToParentZ;
        private SerializedProperty volumeSizeScaleFactorX;
        private SerializedProperty volumeSizeScaleFactorY;
        private SerializedProperty volumeSizeScaleFactorZ;

        private SerializedProperty anchorPositionOverrideEnabled;
        private SerializedProperty drawCornerPoints;
        private SerializedProperty drawFacePoints;
        private SerializedProperty childVolumeItems;
        private SerializedProperty backPlateObject;
        private SerializedProperty volumeSizeOrigin;

        private Texture xAxisDistributeIcon;
        private Texture yAxisDistributeIcon;
        private Texture zAxisDistributeIcon;        
        
        private Texture fillParentXIcon;
        private Texture fillParentYIcon;
        private Texture fillParentZIcon;

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

            xAxisDynamicDistribute = serializedObject.FindProperty("xAxisDynamicDistribute");
            yAxisDynamicDistribute = serializedObject.FindProperty("yAxisDynamicDistribute");
            zAxisDynamicDistribute = serializedObject.FindProperty("zAxisDynamicDistribute");
            leftMargin = serializedObject.FindProperty("leftMargin");
            topMargin = serializedObject.FindProperty("topMargin");
            forwardMargin = serializedObject.FindProperty("forwardMargin");

            fillToParentX = serializedObject.FindProperty("fillToParentX");
            fillToParentY = serializedObject.FindProperty("fillToParentY");
            fillToParentZ = serializedObject.FindProperty("fillToParentZ");

            volumeSizeScaleFactorX = serializedObject.FindProperty("volumeSizeScaleFactorX");
            volumeSizeScaleFactorY = serializedObject.FindProperty("volumeSizeScaleFactorY");
            volumeSizeScaleFactorZ = serializedObject.FindProperty("volumeSizeScaleFactorZ");

            anchorPositionOverrideEnabled = serializedObject.FindProperty("anchorPositionOverrideEnabled");

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
                DrawAnchorPositionOverride();

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

            fillParentXIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "FillParentX" + ".png");
            fillParentYIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "FillParentY" + ".png");
            fillParentZIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/" + "FillParentZ" + ".png");
        }

        private void DrawVolumeSizeOrigin()
        {
            EditorGUILayout.PropertyField(volumeSizeOrigin);
        }

        private void DrawAnchorPositionOverride()
        {
            InspectorUIUtility.DrawTitle("Volume Position");

            Color previousGUIColor = GUI.color;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (anchorPositionOverrideEnabled.boolValue)
                {
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button("Use Anchor Positioning"))
                {
                    anchorPositionOverrideEnabled.boolValue = true;
                }

                GUI.color = previousGUIColor;

                if (!anchorPositionOverrideEnabled.boolValue)
                {
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button("Use Free Positioning"))
                {
                    anchorPositionOverrideEnabled.boolValue = false;
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

            // Only disable buttons for anchors if anchorPositionOverrideEnabled is true
            EditorGUI.BeginDisabledGroup(volumeSizeOrigin.enumValueIndex == (int)VolumeSizeOrigin.TextMeshPro);

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

            EditorGUI.EndDisabledGroup();
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
                EditorGUI.BeginDisabledGroup(anchorPositionOverrideEnabled.boolValue == false);

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

                    if (GUILayout.Button(xAxisDistributeButtonContent, GUILayout.MinHeight(40), GUILayout.MinWidth(150)))
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

                using (new EditorGUILayout.HorizontalScope())
                {
                    // Y Axis Distribution 
                    if (yAxisDynamicDistribute.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }
                    if (GUILayout.Button(yAxisDistributeButtonContent, GUILayout.MinHeight(40), GUILayout.MinWidth(150)))
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

                using (new EditorGUILayout.HorizontalScope())
                {
                    // Z Axis Distribution 
                    if (zAxisDynamicDistribute.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }

                    if (GUILayout.Button(zAxisDistributeButtonContent, GUILayout.MinHeight(40), GUILayout.MinWidth(150)))
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
                
            }
        }

        private void DrawDebuggingSection()
        {
            InspectorUIUtility.DrawTitle("Debugging Settings");

            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Debugging Setting", "Debugging Setting", MixedRealityStylesUtility.BoldFoldoutStyle, false))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(drawCornerPoints);
                    EditorGUILayout.PropertyField(drawFacePoints);
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
    }
}
