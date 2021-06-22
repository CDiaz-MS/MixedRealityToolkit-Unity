using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [Serializable]
    public class VolumeItemSmoothing
    {
        [SerializeField]
        private bool smoothing = false;

        public bool Smoothing
        {
            get => smoothing;
            set => smoothing = value;
        }

        [SerializeField]
        private float lerpTime = 3;

        public float LerpTime
        {
            get => lerpTime;
            set => lerpTime = value;
        }
    }
}
