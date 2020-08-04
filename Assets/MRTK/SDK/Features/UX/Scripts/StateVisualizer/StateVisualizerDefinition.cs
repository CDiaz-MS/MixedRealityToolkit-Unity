using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI
{
    public class StateVisualizerDefinition : ScriptableObject
    {
        // All the game objects contained in the definition 

        public GameObject Target;

        //public Dictionary<string, List<StateStyleProperty>> StateStyleProperties = new Dictionary<string, List<StateStyleProperty>>();

        [SerializeField]
        private List<StateStyleProperties> stateStyleProperties = new List<StateStyleProperties>();

        /// <summary>
        /// List of available states defined by asset
        /// </summary>
        public List<StateStyleProperties> StateStyleProperties
        {
            get { return stateStyleProperties; }
            set { stateStyleProperties = value; }
        }


        public void SetUp(BaseInteractable baseInteractable, GameObject target)
        {
            foreach(State state in baseInteractable.States.StateList)
            {
                StateStyleProperties.Add(new StateStyleProperties(
                    state.Name,
                    target,
                    new List<StateStylePropertyConfiguration>() { new MaterialStateStylePropertyConfiguration()}
                    ));
            }
        }
    }
}
