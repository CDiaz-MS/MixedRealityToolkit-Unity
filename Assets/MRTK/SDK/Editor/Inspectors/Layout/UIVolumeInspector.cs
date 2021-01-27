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
        private SerializedProperty fillToParentX;
        private SerializedProperty fillToParentY;
        private SerializedProperty fillToParentZ;
        private SerializedProperty anchorPositionOverrideEnabled;
        private SerializedProperty drawCornerPoints;
        private SerializedProperty drawFacePoints;
        private SerializedProperty childVolumeItems;
        private SerializedProperty backPlateObject;

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

            fillToParentX = serializedObject.FindProperty("fillToParentX");
            fillToParentY = serializedObject.FindProperty("fillToParentY");
            fillToParentZ = serializedObject.FindProperty("fillToParentZ");

            anchorPositionOverrideEnabled = serializedObject.FindProperty("anchorPositionOverrideEnabled");

            drawCornerPoints = serializedObject.FindProperty("drawCornerPoints");
            drawFacePoints = serializedObject.FindProperty("drawFacePoints");
            childVolumeItems = serializedObject.FindProperty("childVolumeItems");
            backPlateObject = serializedObject.FindProperty("backPlateObject");

            dictionaryKeys = depthLevelDictionary.Keys.ToArray();
        }

        public override void OnInspectorGUI()
        {
            GetIcons();

            serializedObject.Update();

            if (!instance.IsRootUIVolume)
            {
                DrawAnchorButtons();

                EditorGUILayout.Space();

                DrawAnchorSizeSection();

                EditorGUILayout.Space();
            }
            else
            {
                InspectorUIUtility.DrawWarning("This UIVolume is the root transform");

                EditorGUILayout.PropertyField(anchorPositionOverrideEnabled);

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

            Color previousGUIColor = GUI.color;

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    // Fill X
                    if (fillToParentX.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }

                    if (GUILayout.Button(fillToParentXContent, GUILayout.MaxHeight(50)))
                    {
                        fillToParentX.boolValue = !fillToParentX.boolValue;
                    }

                    GUI.color = previousGUIColor;

                    // Fill Y
                    if (fillToParentY.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }

                    if (GUILayout.Button(fillToParentYContent, GUILayout.MaxHeight(50)))
                    {
                        fillToParentY.boolValue = !fillToParentY.boolValue;
                    }

                    GUI.color = previousGUIColor;

                    // Fill Z
                    if (fillToParentZ.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }

                    if (GUILayout.Button(fillToParentZContent, GUILayout.MaxHeight(50)))
                    {
                        fillToParentZ.boolValue = !fillToParentZ.boolValue;
                    }

                    GUI.color = previousGUIColor;
                }
            }
        }

        private void DrawAnchorButtons()
        {
            InspectorUIUtility.DrawTitle("Anchor Position");

            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.PropertyField(anchorPositionOverrideEnabled);

                EditorGUILayout.Space();

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

                        if (IsMaintainScaleValid(transform))
                        {
                            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                Transform transformRef = transform.objectReferenceValue as Transform;

                                GUI.enabled = false;
                                EditorGUILayout.PropertyField(transform);
                                EditorGUILayout.PropertyField(scaleToLock);
                                GUI.enabled = true;

                                EditorGUILayout.PropertyField(maintainScale);
                            }
                        }

                        EditorGUILayout.Space();

                        GUI.color = previousGUIColor;
                    }
                }
            }
        }

        private bool IsMaintainScaleValid(SerializedProperty transform)
        {
            Transform transformRef = transform.objectReferenceValue as Transform;

            if (transformRef.gameObject.GetComponent<UIVolume>() != null)
            {
                if (transformRef.gameObject.GetComponent<TextMesh>() != null || transformRef.gameObject.GetComponent<Collider>() != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
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

                    if (GUILayout.Button(xAxisDistributeButtonContent, GUILayout.MinHeight(40)))
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

                    // Y Axis Distribution 
                    if (yAxisDynamicDistribute.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }
                    if (GUILayout.Button(yAxisDistributeButtonContent, GUILayout.MinHeight(40)))
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

                    // Z Axis Distribution 
                    if (zAxisDynamicDistribute.boolValue)
                    {
                        GUI.color = Color.cyan;
                    }

                    if (GUILayout.Button(zAxisDistributeButtonContent, GUILayout.MinHeight(40)))
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
