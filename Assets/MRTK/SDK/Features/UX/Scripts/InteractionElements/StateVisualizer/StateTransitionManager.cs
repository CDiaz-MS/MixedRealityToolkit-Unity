// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public class StateTransitionManager
    {

        public StateTransitionManager()
        {

        }


        public bool UseTransitions { get; set; }

        public List<GameObject> objectDefaults = new List<GameObject>();

        private Material cachedMat;

        private Vector3 cachedPosition;
        private Quaternion cachedRotation;
        private Vector3 cachedScale;


        public void SaveDefaultStates(GameObject objectToSave)
        {

            // But what if someone wants to set the default state?
            // Allow someone to set the defalut state via code but not via in the editor?
            objectDefaults.Add(objectToSave);

            cachedMat = objectToSave.GetComponent<MeshRenderer>().material;

            cachedPosition = objectToSave.transform.position;
            cachedRotation = objectToSave.transform.rotation;
            cachedScale = objectToSave.transform.localScale;
        }


        public void SetDefaults(GameObject objectToSave)
        {
            objectToSave.GetComponent<MeshRenderer>().material = cachedMat;

            objectToSave.transform.position = cachedPosition;
            objectToSave.transform.rotation = cachedRotation;
            objectToSave.transform.localScale = cachedScale;
        }


        // Go through each object referenced in each style property and store their defalut states


    }
}
