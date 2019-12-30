using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Tests
{

    public class ExampleTests
    {
        private const string ExamplePrefabPath = "Assets/Packages/TestingUtilitiesMRTK.1.0.0/PlayModeTests/Prefabs/InteractableCube.prefab";

        /// <summary>
        /// Create a scene with MRTK gameobjects (MixedRealityToolkit and MixedRealityPlayspace) for
        /// testing. SetUp is called before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Creates an empty scene and adds the MRTK gameobjects.
            PlayModeTestUtilities.Setup();

            // Set the position of the camera to (0,0,0), by default the camera is 
            // set to position (1.0f, 1.5f, -2.0f)
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        /// <summary>
        /// Destory gameobjects in scene at the end of each test. TearDown is called at the end of each test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// Simple example on how to create and move a test hand through a scene.  Each action for the test hand 
        /// requires a couple of frames, make sure "yield return" is in front of a function call in this class.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator UsingTestHandTest()
        {
            // Create new right hand to move throught the scene
            var rightHand = new TestHand(Handedness.Right);

            Vector3 initialHandPosition = new Vector3(0, 0, 1);
            // Show the right hand given a position in the scene
            yield return rightHand.Show(initialHandPosition);

            // Move the hand up 0.3 from the previous position 
            yield return rightHand.Move(new Vector3(0, 0.3f, 0));

            // Move the hand to a new position
            yield return rightHand.MoveTo(new Vector3(0.1f, -0.1f, 0.5f));

            // Do not continue the test until the enter key is pressed.
            // This function is not used in any tests but it is useful for debugging a test.
            // While in this state you can observe the scene in the hierarchy and 
            // explore the scene view. 
            ///yield return PlayModeTestUtilities.WaitForEnterKey();

            yield return null;
        }

        /// <summary>
        /// Example of how to load and test a prefab.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestingPrefabExampleTest()
        {
            Vector3 initialHandPosition = new Vector3(0, -0.1f, 0.6f);
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(initialHandPosition);
            yield return null;

            // Instantiate prefab based on path
            GameObject ExamplePrefab = InstantiatePrefabFromPath(new Vector3(0,0,1), Quaternion.identity, ExamplePrefabPath);
            yield return null;

            Interactable interactable = ExamplePrefab.GetComponent<Interactable>();
            Assert.NotNull(interactable);

            bool wasClicked = false;

            // Add OnClick Listener
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            // Click gesture for the hand
            yield return rightHand.Click();

            // Wait a frame for the click to register
            yield return null;

            // Check if the OnClick event was triggered 
            Assert.True(wasClicked);
        }

        #region Test Helpers

        private GameObject InstantiatePrefabFromPath(Vector3 position, Quaternion rotation, string path)
        {
            // Load interactable prefab
            Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject result = Object.Instantiate(prefab, position, rotation) as GameObject;
            Assert.IsNotNull(result);
            return result;
        }

        #endregion


    }
}
