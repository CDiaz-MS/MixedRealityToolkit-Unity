// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A way to control a list of radial type buttons or tabs
    /// </summary>
    public class InteractableToggleCollection : MonoBehaviour
    {
        [Tooltip("Interactables that will be managed by this controller")]
        public Interactable[] ToggleList;

        //[Tooltip("Currently selected index or default starting index")]
        [SerializeField]
        private int currentIndex;
        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                currentIndex = value;
                OnIndexUpdate(currentIndex);
            }
        }

        [Tooltip("exposed selection changed event")]
        public UnityEvent OnSelectionEvents;

        private void Start()
        {
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                int itemIndex = i;
                // add selection event handler to each button
                ToggleList[i].OnClick.AddListener(() => OnSelection(itemIndex));
                ToggleList[i].CanDeselect = false;
            }

            //OnSelection(CurrentIndex, true);
        }

        private void OnEnable()
        {
            // Set current index based on what is in the inspector
            // Is this legal?
            CurrentIndex = currentIndex;
            //OnSelection(CurrentIndex, true);
        }

        /// <summary>
        /// Sets the selected index and selected Interactive
        /// </summary>
        public void SetSelection(int index)
        {
            if (!isActiveAndEnabled ||
                (index < 0 || ToggleList.Length <= index))
            {
                return;
            }

            OnSelection(index, true);
        }

        /// <summary>
        /// Set the toggle state of each button based on the selected item
        /// </summary>
        protected virtual void OnSelection(int index, bool force = false)
        {
            Debug.Log("On Selection");
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                if (i != index)
                {
                    ToggleList[i].IsToggled = false;
                }
            }

            currentIndex = index;

            if (force)
            {
                ToggleList[index].IsToggled = true;
            }
            else
            {
                OnSelectionEvents.Invoke();
            }
        }

        private void OnIndexUpdate(int index)
        {
            ToggleList[index].TriggerOnClick();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                int itemIndex = i;
                ToggleList[i].OnClick.RemoveListener(() => OnSelection(itemIndex));
            }
        }
    }
}
