using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class InteractionElementsTests : BasePlayModeTests
    {

        [UnityTest]
        public IEnumerator TestAddingAndSettingCoreState()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0, 0.1f, 0.7f);
            cube.transform.localScale = Vector3.one * 0.2f;
            yield return null;

            BasicButton basicButton = cube.AddComponent<BasicButton>();
            yield return null; 

            InteractionState focusState = basicButton.AddCoreState(CoreInteractionState.Focus);
            yield return null;

            //yield return PlayModeTestUtilities.WaitForEnterKey();

            var eventConfiguration = basicButton.EventReceiverManager.GetEventConfiguration("Focus") as FocusInteractionEventConfiguration;

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

            // Set hand position 
            Vector3 handStartPosition = new Vector3(0, 0.2f, 0.4f);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handStartPosition);

            //yield return PlayModeTestUtilities.WaitForEnterKey();

            // Check if OnFocusOn has fired
            Assert.True(onFocusOn);

            // Move the Hand
            yield return leftHand.Move(new Vector3(0, -0.3f, 0), 30);

            // Check if OnFocusOn has fired
            Assert.True(onFocusOff);

            //yield return PlayModeTestUtilities.WaitForEnterKey();

            yield return null;
        }


        [UnityTest]
        public IEnumerator TestAddingAndSettingNewState()
        {
            GameObject cube = new GameObject("cube");
            cube.transform.position = new Vector3(0, 0, 0.7f);
            cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            BasicButton basicButton = cube.AddComponent<BasicButton>();

            //basicButton.ad

            yield return null;

            yield return PlayModeTestUtilities.WaitForEnterKey();

            yield return null;
        }
    }
}
