using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI
{
    [CreateAssetMenu(fileName = "States", menuName = "Mixed Reality Toolkit/ActiveState", order = 1)]
    public class ActiveStates : ScriptableObject
    {
        [SerializeField]
        private List<State> stateList = new List<State>();

        /// <summary>
        /// List of available states defined by asset
        /// </summary>
        public List<State> StateList
        {
            get { return stateList; }
            set { stateList = value; }
        }

        public ActiveStates()
        {
            // Add default states to StateList
            StateList.Add(new State() { Index = 0, Name = "Default", ActiveIndex = -1, Bit = 0, Value = 0 });
            StateList.Add(new State() { Index = 1, Name = "Focus", ActiveIndex = -1, Bit = 0, Value = 0 });
        }



        public void AddStateEvents(string StateName)
        {
            //if (StateName == "Focus")
            //{
            //    FocusEvents focusEvents = new FocusEvents();
            //    // BaseInteractable
            //}
        }
    }
}
