// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using NUnit.Framework;
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

            ConstructVolume<VolumeGrid>(itemCount, null, ButtonGUID);

            yield return PlayModeTestUtilities.WaitForEnterKey();
        }

        [UnityTest]
        public IEnumerator TestBasicEllipseLayout()
        {
            int itemCount = 6;

            ConstructVolume<VolumeEllipse>(itemCount, null, ButtonGUID);

            yield return PlayModeTestUtilities.WaitForEnterKey();
        }

        [UnityTest]
        public IEnumerator TestGridCoordinateSystem()
        {
            List<GameObject> objects = CreateUniqueObjectList();

            VolumeGrid grid = ConstructVolumeFromList<VolumeGrid>(objects) as VolumeGrid;
            grid.Depth = 1;
            grid.Rows = 1;
            yield return null;

            GameObject firstCoordinateObject = grid.GetObjectAtCoordinates(new Vector3(1, 1, 1));
            GameObject secondCoordinateObject = grid.GetObjectAtCoordinates(new Vector3(1, 2, 1));
            GameObject thirdCoordinateObject = grid.GetObjectAtCoordinates(new Vector3(1, 3, 1));
            yield return null;

            Assert.IsNotNull(firstCoordinateObject);
            Assert.IsNotNull(secondCoordinateObject);
            Assert.IsNotNull(thirdCoordinateObject);
            yield return null;

            Assert.AreEqual(firstCoordinateObject, objects[0]);
            Assert.AreEqual(secondCoordinateObject, objects[1]);
            Assert.AreEqual(thirdCoordinateObject, objects[2]);
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

        private BaseCustomVolume ConstructVolume<T>(int itemCount, GameObject objectToPopulate, string GUID = "") where T : BaseCustomVolume
        {
            GameObject volumeRoot = new GameObject("VolumeRoot");
            T customVolume = volumeRoot.AddComponent<T>();

            for (int i = 0; i < itemCount; i++)
            {
                if (GUID.Length == 0)
                {
                    GameObject go = Object.Instantiate(objectToPopulate);
                    go.transform.SetParent(volumeRoot.transform);
                }
                else
                {
                    GameObject go = InstantiateGameObjectFromGUID(GUID);
                    go.transform.SetParent(volumeRoot.transform);
                }
            }

            return customVolume;
        }

        private BaseCustomVolume ConstructVolumeFromList<T>(List<GameObject> objects) where T : BaseCustomVolume
        {
            GameObject volumeRoot = new GameObject("VolumeRoot");
            T customVolume = volumeRoot.AddComponent<T>();

            foreach (GameObject go in objects)
            {
                go.transform.SetParent(volumeRoot.transform);
            }

            return customVolume;
        }

        private GameObject InstantiateGameObjectFromGUID(string GUID)
        {
            string path = AssetDatabase.GUIDToAssetPath(GUID);
            Object go = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject result = Object.Instantiate(go) as GameObject;
            Assert.IsNotNull(result, "The GUID given does not exist in the asset database");
            return result;
        }

        private List<GameObject> CreateUniqueObjectList()
        {
            List<GameObject> objects = new List<GameObject>();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = Vector3.one * 0.2f;

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = Vector3.one * 0.2f;

            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.transform.localScale = Vector3.one * 0.2f;

            objects.Add(cube);
            objects.Add(sphere);
            objects.Add(capsule);

            return objects;
        }

        #endregion
    }
}
