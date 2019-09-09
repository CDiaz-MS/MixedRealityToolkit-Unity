#if !WINDOWS_UWP


using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class PointerTests 
    {

        // this method is called once before we enter play mode and execute any of the tests
        // do any kind of setup here that can't be done in playmode

        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }


        #region Tests

        /// <summary>
        /// Tests that right after being instantiated, the pointer's direction matches
        /// the rotation of the hand.
        /// </summary>
        /// <returns></returns>
        /// 
        [UnityTest]
        public IEnumerator TestPointerDirectionMatchesHandRayFirstFrame()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            // Raise the hand
            var rightHand = new TestHand(Handedness.Right);

            //Position 1
            Vector3 initialPos = new Vector3(0.01f, 0.1f, 0.5f);
            yield return rightHand.Show(initialPos);

            var controllers = inputSystem.DetectedControllers;

            //Make sure detected controllers count is greater than 0
            Assert.Greater(controllers.Count(), 0);

            //return first hand controller that is right and source type hand
            var handController = inputSystem.DetectedControllers.First(x => x.ControllerHandedness == Utilities.Handedness.Right && x.InputSource.SourceType == InputSourceType.Hand);
            Debug.Assert(handController != null);

            // Get the line pointer from the hand controller
            var linePointer = handController.InputSource.Pointers.First(x => x is LinePointer);
            Assert.IsNotNull(linePointer);

            // Make sure that the z value of the direction of the ray in the line pointer is not negative
            // A negative value for z in the direction vector means that the ray is not pointing forward
            Assert.Positive(Mathf.Sign(linePointer.Rays[0].Direction.z));
            Debug.Log("Ray Direction1: " + linePointer.Rays[0].Direction);

            //Wait and move to position 2 
            yield return new WaitForSeconds(2);
            yield return rightHand.MoveTo(new Vector3(-1.0f, 0, 2.0f));

            // Make sure z is positive after move
            Assert.Positive(Mathf.Sign(linePointer.Rays[0].Direction.z));
            Debug.Log("Ray Direction2: " + linePointer.Rays[0].Direction);

            //Wait and move to position 3 
            yield return new WaitForSeconds(2);
            yield return rightHand.MoveTo(new Vector3(1.0f, 0, 2.0f));

            // Make sure z is positive after move
            Assert.Positive(Mathf.Sign(linePointer.Rays[0].Direction.z));
            Debug.Log("Ray Direction3: " + linePointer.Rays[0].Direction);

            //Wait and move to position 4
            yield return new WaitForSeconds(2);
            yield return rightHand.MoveTo(new Vector3(1.0f, 1.0f, 2.0f));

            // Make sure z is positive after move
            Assert.Positive(Mathf.Sign(linePointer.Rays[0].Direction.z));
            Debug.Log("Ray Direction4: " + linePointer.Rays[0].Direction);

        }

        #endregion

    }
}
#endif
