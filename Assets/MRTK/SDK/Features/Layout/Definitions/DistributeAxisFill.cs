using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [Serializable]
    public class DistributeAxisFill
    {
        [SerializeField]
        private Axis distributeAxis;

        public Axis DistributeAxis
        {
            get => distributeAxis;
            internal set => distributeAxis = value;
        }

        // Container Fill 
        [SerializeField]
        private bool containerFillX = false;

        public bool ContainerFillX
        {
            get => containerFillX;
            set => containerFillX = value;
        }

        [SerializeField]
        private bool containerFillY = false;

        public bool ContainerFillY
        {
            get => containerFillY;
            set => containerFillY = value;
        }

        [SerializeField]
        private bool containerFillZ = false;

        public bool ContainerFillZ
        {
            get => containerFillZ;
            set => containerFillZ = value;
        }

        public DistributeAxisFill(Axis axis)
        {
            distributeAxis = axis;
        }

        public float CalculateScaleFactor(int childCount)
        {
            return Mathf.Pow(childCount, -1);
        }
    }
}
