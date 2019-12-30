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
    /// <summary>
    /// Experimental class for loading and testing already defined scenes from a path.
    /// </summary>
    public class ExampleSceneTests
    {
        private const string ExampleScenePath = "Assets/Packages/TestingUtilitiesMRTK.1.0.0/PlayModeTests/Scenes/ExampleTestScene";

        /// <summary>
        /// Simple example on how to load a scene and set gestures for a test hand.
        /// </summary>
        [UnityTest]
        public IEnumerator TestingASceneFromPathTest()
        {
            // Load scene from path
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode($"{ExampleScenePath}.unity", new LoadSceneParameters(LoadSceneMode.Single));

            Vector3 initialHandPosition = new Vector3(-0.2f, -0.05f, 0.5f);
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(initialHandPosition);
            yield return null;

            GameObject cube = GameObject.Find("Cube1");
            Vector3 cubeStartingPosition = cube.transform.position;

            // Use helper to move the hand
            yield return MoveObjectUpDown(rightHand);

            Vector3 cubeEndingPosition = cube.transform.position;

            Assert.AreNotEqual(cubeStartingPosition, cubeEndingPosition);
        }

        #region Test Helpers

        private IEnumerator MoveObjectUpDown(TestHand hand)
        {
            // Set the Gesture to pinch to select the object
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Move the object up and then down 
            yield return hand.Move(new Vector3(0, 0.2f, 0));
            yield return hand.Move(new Vector3(0, -0.4f, 0));

            // Set the Gesture to Open to let go of the object
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
        }

        #endregion
    }
}
