// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{ 
    [Serializable]
    public class VolumeGridNode
    {
        public string Name;
        public Bounds CellBounds;
        public GameObject CellGameObject;
        public Transform CellGameObjectTransform;
        public Vector3 Coordinates;
        public Vector3 Offset;
        public int Count;
        public bool IsCellPopulated => CheckCellTaken();

        private bool CheckCellTaken()
        {
            return CellGameObject != null;
        }
    }
}