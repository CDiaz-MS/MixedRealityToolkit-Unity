// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(VolumeFlex))]
    public class VolumeFlexInspector : VolumeGridInspector
    {
        private VolumeFlex instanceFlex;

        public override void OnEnable()
        {
            base.OnEnable();

            instanceFlex = target as VolumeFlex;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DrawFlexSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawFlexSettings()
        {

        }
    }
}
