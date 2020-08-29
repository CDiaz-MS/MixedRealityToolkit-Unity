// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Tests for a BaseInteractiveElement and the StateVisualizer
    /// </summary>
    public class InteractionElementsTests : BasePlayModeTests
    {
        /// <summary>
        /// Tests adding event data
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestEventsOfATrackedCoreState()
        {
            InteractiveElement interactiveElement = CreateInteractionCube();
            yield return null;

            // The focus state is a state that is added by default
            InteractionState focusState = interactiveElement.GetState(CoreInteractionState.Focus);
            yield return null;

            //yield return PlayModeTestUtilities.WaitForEnterKey();

            FocusInteractionEventConfiguration eventConfiguration = focusState.EventConfiguration as FocusInteractionEventConfiguration;

            bool onFocusOn = false;
            bool onFocusOff = false;

            eventConfiguration.OnFocusOn.AddListener((eventData) =>
                {
                    onFocusOn = true;
                });

            eventConfiguration.OnFocusOff.AddListener((eventData) =>
                {
                    onFocusOff = true;
                });

            var leftHand = new TestHand(Handedness.Left);

            // Set hand position 
            yield return ShowHandWithObjectInFocus(leftHand);

            // Check if OnFocusOn has fired
            Assert.True(onFocusOn);

            // Move the Hand
            yield return MoveHandObjectOutOfFocus(leftHand);

            // Check if OnFocusOn has fired
            Assert.True(onFocusOff);

            yield return null;
        }

        /// <summary>
        /// Test creating a new state and setting the values of the new state.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAddingAndSettingNewState()
        {
            // Create a cube 
            InteractiveElement interactiveElement = CreateInteractionCube();
            yield return null;

            // Creat a new state and add it to Tracked States
            interactiveElement.AddNewState("MyNewState");

            // Change the value of my new state if the object comes into focus
            InteractionState focusState = interactiveElement.GetState(CoreInteractionState.Focus);

            FocusInteractionEventConfiguration focusEventConfiguration = focusState.EventConfiguration as FocusInteractionEventConfiguration;

            focusEventConfiguration.OnFocusOn.AddListener((focusEventData) => 
            {
                // When the object comes into focus, set my new state to on
                interactiveElement.SetStateOn("MyNewState");
            });

            focusEventConfiguration.OnFocusOff.AddListener((focusEventData) =>
            {
                // When the object comes out of  focus, set my new state to off
                interactiveElement.SetStateOff("MyNewState");
            });

            // Make sure MyNewState is in TrackedStates
            InteractionState myNewState = interactiveElement.GetState("MyNewState");
            Assert.IsNotNull(myNewState);

            // Make sure the value is 0 initially
            Assert.AreEqual(myNewState.Value, 0);

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            // Make sure the value of MyNewState was changed when the object is in Focus
            Assert.AreEqual(myNewState.Value, 1);

            // Move hand away from object to remove focus
            yield return MoveHandObjectOutOfFocus(leftHand);

            // Make sure the value of MyNewState was changed when the object is no longer in focus
            Assert.AreEqual(myNewState.Value, 0);
        }



        [UnityTest]
        public IEnumerator TestAddStateVisualizerAndSettingAStateStyleProperty()
        {
            // Create a cube 
            InteractiveElement interactiveElement = CreateInteractionCube();
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
            yield return MoveHandObjectOutOfFocus(leftHand);

            Assert.True(interactiveElement.gameObject.transform.localScale == initialScale);

        }



        [UnityTest]
        public IEnumerator TestAdjustStatesDuringRuntime()
        {
            InteractiveElement interactiveElement = CreateInteractionCube();

            interactiveElement.AddNewState("NewState");

            StateVisualizer stateVisualizer = interactiveElement.gameObject.AddComponent<StateVisualizer>();

            stateVisualizer.AddStateStylePropertyToAState(CoreStyleProperty.TransformOffset, "NewState");

            interactiveElement.AddNewState("AnotherNewState");

            // Add state style property to a new state

            stateVisualizer.AddStateStylePropertyToAState(CoreStyleProperty.TransformOffset, "AnotherNewState");


            //basicButton.RemoveState("newState");

           
            yield return null;
        }


        [UnityTest]
        public IEnumerator TestAddingStateStylePropertyToNullContainer()
        {

            


            yield return null;
        }



        #region Interaction Tests Helpers

        private InteractiveElement CreateInteractionCube()
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

        private void MoveHandToFocusOnObject()
        {

        }

        private IEnumerator MoveHandObjectOutOfFocus(TestHand hand)
        {
            yield return hand.Move(new Vector3(0, -0.3f, 0), 30);
        }




        #endregion


    }
}
