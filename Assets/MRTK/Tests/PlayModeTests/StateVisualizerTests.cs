using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Tests;
using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class StateVisualizerTests : BasePlayModeTests
    {

        [UnityTest]
        public IEnumerator TestAddStateVisualizerAndSettingAStateStyleProperty()
        {
            // Create a cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            Vector3 initialScale = interactiveElement.gameObject.transform.localScale;

            // Add StateVisualizer
            StateVisualizer stateVisualizer = interactiveElement.gameObject.AddComponent<StateVisualizer>();
            yield return null;

            TransformOffsetStateStylePropertyConfiguration transformOffsetConfig = stateVisualizer.AddStateStylePropertyToAState(CoreStyleProperty.TransformOffset, CoreInteractionState.Focus) as TransformOffsetStateStylePropertyConfiguration;

            // Increase the local scale of the object by one
            transformOffsetConfig.Scale = Vector3.one * 0.1f;

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            Assert.True(interactiveElement.gameObject.transform.localScale == initialScale + (Vector3.one * 0.1f));

            // Move hand away from object to remove focus
            yield return MoveHandOutOfFocus(leftHand);

            Assert.True(interactiveElement.gameObject.transform.localScale == initialScale);

        }

        [UnityTest]
        public IEnumerator TestAdjustStatesDuringRuntime()
        {
            InteractiveElement interactiveElement = CreateInteractiveCube();

            interactiveElement.AddNewState("NewState");

            StateVisualizer stateVisualizer = interactiveElement.gameObject.AddComponent<StateVisualizer>();

            stateVisualizer.AddStateStylePropertyToAState(CoreStyleProperty.TransformOffset, "NewState");

            interactiveElement.AddNewState("AnotherNewState");

            // Add state style property to a new state
            stateVisualizer.AddStateStylePropertyToAState(CoreStyleProperty.TransformOffset, "AnotherNewState");

            interactiveElement.RemoveState("newState");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestAddingStateStylePropertyToNullContainer()
        {
            yield return null;
        }

        #region Test Helper Methods

        private InteractiveElement CreateInteractiveCube()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0, 0, 0.7f);
            cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            InteractiveElement basicButton = cube.AddComponent<InteractiveElement>();

            return basicButton;
        }

        private IEnumerator ShowHandWithObjectInFocus(TestHand hand)
        {
            // Set hand position 
            Vector3 handStartPosition = new Vector3(0, 0.2f, 0.4f);
            yield return hand.Show(handStartPosition);
        }

        private IEnumerator MoveHandOutOfFocus(TestHand hand)
        {
            yield return hand.Move(new Vector3(0, -0.3f, 0), 30);
        }

        #endregion
    }
}
