// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Test for Volume
    /// </summary>
    public class LayoutTests : BasePlayModeTests
    {
        private const string ButtonGUID = "47497728927d87c4791a9e1533f34fa7";
        private static readonly string ButtonPrefabAssetPath = AssetDatabase.GUIDToAssetPath(ButtonGUID);

        [UnityTest]
        public IEnumerator TestBasicLayout()
        {
            ConstructVolumeLayout();

            yield return null;

            yield return PlayModeTestUtilities.WaitForEnterKey();

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestBasicGridLayout()
        {
            int itemCount = 27;

            ConstructVolume<VolumeGrid>(itemCount);

            yield return PlayModeTestUtilities.WaitForEnterKey();
        }

        [UnityTest]
        public IEnumerator TestBasicEllipseLayout()
        {
            int itemCount = 6;

            ConstructVolume<VolumeEllipse>(itemCount);

            yield return PlayModeTestUtilities.WaitForEnterKey();
        }

        #region Test Helpers
        private void ConstructVolumeLayout()
        {
            GameObject volumeRoot = new GameObject("VolumeRoot");
            VolumeAnchorPosition rootVolume = volumeRoot.AddComponent<VolumeAnchorPosition>();
            
            GameObject childContainer = new GameObject("ChildVolume");
            VolumeAnchorPosition childContainerVolume = childContainer.AddComponent<VolumeAnchorPosition>();

            childContainer.transform.SetParent(volumeRoot.transform);

            childContainerVolume.EqualizeVolumeSizeToParent();

            childContainerVolume.FillToParentX = true;
            childContainerVolume.VolumeSizeScaleFactorX = 0.5f;
        }

        public void ConstructVolume<T>(int itemCount) where T : BaseCustomVolume
        {
            GameObject volumeRoot = new GameObject("VolumeRoot");
            T rootVolume = volumeRoot.AddComponent<T>();

            for (int i = 0; i < itemCount; i++)
            {
                InstantiateButtonPrefab(rootVolume.Volume);
            }
        }

        private void InstantiateButtonPrefab(BaseVolume volume)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(ButtonPrefabAssetPath, typeof(Object));
            GameObject volumeItem = Object.Instantiate(prefab) as GameObject;

            volumeItem.transform.SetParent(volume.transform);
        }

        #endregion
    }
}
