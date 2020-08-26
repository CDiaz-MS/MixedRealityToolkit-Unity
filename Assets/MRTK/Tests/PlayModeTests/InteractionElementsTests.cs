using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class InteractionElementsTests : BasePlayModeTests
    {
        [UnityTest]
        public IEnumerator TestEventsOfATrackedCoreState()
        {
            BasicButton basicButton = CreateInteractionCube();
            yield return null;

            yield return PlayModeTestUtilities.WaitForEnterKey();

            // The focus state is a state that is added by default
            InteractionState focusState = basicButton.GetState(CoreInteractionState.Focus);
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

            yield return PlayModeTestUtilities.WaitForEnterKey();

            // Check if OnFocusOn has fired
            Assert.True(onFocusOn);

            // Move the Hand
            yield return MoveHandObjectOutOfFocus(leftHand);

            // Check if OnFocusOn has fired
            Assert.True(onFocusOff);

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            //yield return PlayModeTestUtilities.WaitForEnterKey();

            yield return null;
        }

        //[UnityTest]
        //public IEnumerator TestAddingAndSettingNewCoreState()
        //{
        //    BasicButton basicButton = CreateInteractionCube();
        //    yield return null;

        //    InteractionState touchState = basicButton.AddCoreState(CoreInteractionState.Touch);
        //    yield return null;

        //    //yield return PlayModeTestUtilities.WaitForEnterKey();

        //    //TouchInteractionEventConfiguration eventConfiguration = touchState.EventConfiguration as TouchInteractionEventConfiguration;

        //    bool onFocusOn = false;
        //    bool onFocusOff = false;

        //    eventConfiguration.OnFocusOn.AddListener((eventData) =>
        //    {
        //        onFocusOn = true;
        //    });

        //    eventConfiguration.OnFocusOff.AddListener((eventData) =>
        //    {
        //        onFocusOff = true;
        //    });

        //    var leftHand = new TestHand(Handedness.Left);

        //    // Set hand position 
        //    yield return ShowHandWithObjectInFocus(leftHand);

        //    yield return PlayModeTestUtilities.WaitForEnterKey();

        //    // Check if OnFocusOn has fired
        //    Assert.True(onFocusOn);

        //    // Move the Hand
        //    yield return MoveHandObjectOutOfFocus(leftHand);

        //    // Check if OnFocusOn has fired
        //    Assert.True(onFocusOff);

        //    yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

        //    //yield return PlayModeTestUtilities.WaitForEnterKey();

        //    yield return null;
        //}


        /// <summary>
        /// Test creating a new state and setting the values of the new state.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAddingAndSettingNewState()
        {
            // Create a cube 
            BasicButton basicButton = CreateInteractionCube();
            yield return null;

            // Creat a new state and add it to Tracked States
            basicButton.CreateAndAddNewState("MyNewState");

            // Change the value of my new state if the object comes into focus
            InteractionState focusState = basicButton.GetState(CoreInteractionState.Focus);

            FocusInteractionEventConfiguration focusEventConfiguration = focusState.EventConfiguration as FocusInteractionEventConfiguration;

            focusEventConfiguration.OnFocusOn.AddListener((focusEventData) => 
            {
                // When the object comes into focus, set my new state to on
                basicButton.SetStateOn("MyNewState");
            });

            focusEventConfiguration.OnFocusOff.AddListener((focusEventData) =>
            {
                // When the object comes out of  focus, set my new state to off
                basicButton.SetStateOff("MyNewState");
            });

            // Make sure MyNewState is in TrackedStates
            InteractionState myNewState = basicButton.GetState("MyNewState");
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
            BasicButton basicButton = CreateInteractionCube();
            yield return null;

            Vector3 initialScale = basicButton.gameObject.transform.localScale;

            // Add StateVisualizer
            StateVisualizer stateVisualizer = basicButton.gameObject.AddComponent<StateVisualizer>();
            yield return null;

            TransformOffsetStateStylePropertyConfiguration transformOffsetConfig = stateVisualizer.AddStateStylePropertyToAState(CoreStyleProperty.TransformOffset, CoreInteractionState.Focus) as TransformOffsetStateStylePropertyConfiguration;

            // Increase the local scale of the object by one
            transformOffsetConfig.Scale = Vector3.one * 0.1f;

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            Assert.True(basicButton.gameObject.transform.localScale == initialScale + (Vector3.one * 0.1f));

            // Move hand away from object to remove focus
            yield return MoveHandObjectOutOfFocus(leftHand);

            Assert.True(basicButton.gameObject.transform.localScale == initialScale);

        }



        [UnityTest]
        public IEnumerator AdjustStatesDuringRuntime()
        {
            yield return null;

        }



        #region Interaction Tests Helpers

        private BasicButton CreateInteractionCube()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0, 0, 0.7f);
            cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            BasicButton basicButton = cube.AddComponent<BasicButton>();

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
