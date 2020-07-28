using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CustomEditor(typeof(BasicButton))]
    public class BasicButtonInspector : BaseInteractableInspector
    {

        protected override void OnEnable()
        {
            base.OnEnable();
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
