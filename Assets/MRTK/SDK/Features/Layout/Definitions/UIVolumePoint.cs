using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [Serializable]
    public class UIVolumePoint 
    {
        public string PointName;


        [SerializeField]
        private Vector3 point = new Vector3();

        public Vector3 Point
        {
            get => point;
            set
            {
                point = value;
            }
        }

        public UIVolumePoint(string pointName)
        {
            PointName = pointName;
        }
    }
}
