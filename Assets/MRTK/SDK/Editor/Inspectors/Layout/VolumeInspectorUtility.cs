using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    public static class VolumeInspectorUtility 
    {
        private static Color warningColor = new Color(1f, 0.85f, 0.6f);

        private static int TitleFontSize = 14;

        private static readonly Color ProfessionalThemeColorTint100 = new Color(0f, 0f, 0f);
        private static readonly Color ProfessionalThemeColorTint75 = new Color(0.25f, 0.25f, 0.25f);
        private static readonly Color ProfessionalThemeColorTint50 = new Color(0.5f, 0.5f, 0.5f);

        private static readonly Color PersonalThemeColorTint100 = new Color(1f, 1f, 1f);
        private static readonly Color PersonalThemeColorTint75 = new Color(0.75f, 0.75f, 0.75f);
        private static readonly Color PersonalThemeColorTint50 = new Color(0.5f, 0.5f, 0.5f);

        private static Color ColorTint50 => EditorGUIUtility.isProSkin ? ProfessionalThemeColorTint50 : PersonalThemeColorTint50;

        public static void DrawTitle(string title)
        {
            GUIStyle labelStyle = LableStyle(TitleFontSize, ColorTint50);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
            EditorGUILayout.Space();
        }

        public static void DrawWarning(string warning)
        {
            Color prevColor = GUI.color;

            GUI.color = warningColor;
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField(warning, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();

            GUI.color = prevColor;
        }

        public static GUIStyle LableStyle(int size, Color color)
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = size;
            labelStyle.fixedHeight = size * 2;
            labelStyle.normal.textColor = color;
            return labelStyle;
        }

        public static Texture GetIcon(string GUID)
        {
            string path = AssetDatabase.GUIDToAssetPath(GUID);
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(path);
            return icon;
        }

        public static void DrawHorizontalToggleSlider(SerializedProperty boolProperty, string boolTitle, SerializedProperty floatProperty, string floatTitle, float sliderMax)
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

        public static bool DrawSectionFoldoutWithKey(string headerName, string preferenceKey = null, bool defaultOpen = true)
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

        public static bool DrawSectionFoldout(string headerName, bool open = true, GUIStyle style = null)
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

        public static bool DrawColorToggleButton(SerializedProperty boolProperty, GUIContent buttonContent, int minHeight, int minWidth)
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
    }
}
