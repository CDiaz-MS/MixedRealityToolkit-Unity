using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [ExecuteAlways]
    public class UIItem : MonoBehaviour
    {
        private Transform parentTransform;

        [SerializeField]
        private bool maintainScale = true;

        public bool MaintainScale
        {
            get => maintainScale;
            set
            {
                maintainScale = value;
            }
        }

        [SerializeField]
        private Vector3 scaleToLock;

        public Vector3 ScaleToLock
        {
            get => scaleToLock;
            set
            {
                scaleToLock = value;
            }
        }

        //private Vector3 startScale = new Vector3(0.01f, 0.01f, 0.01f);

        private Vector3 startParentScale;
        private Vector3 startParentParentScale;

        private void Start()
        {
            ScaleToLock = transform.localScale;

            //startScale = transform.localScale;

            startParentScale = transform.parent.lossyScale;

            parentTransform = transform.parent;
        }

        private void Update()
        {
            if (MaintainScale)
            {
                MaintainScaleObject();
            }
            else
            {
                if (parentTransform.hasChanged)
                {
                    startParentScale = transform.parent.lossyScale;
                }

                ScaleToLock = transform.localScale;     
            }
        }

        public void MaintainScaleObject()
        {
            var currentParentLossyScale = transform.parent.lossyScale;

            //Debug.Log("Current parent local scale: " + currentParentLossyScale);

            // Get the relative difference to the original scale
            var diffX = currentParentLossyScale.x / startParentScale.x;

            //Debug.Log("diffX: " + currentParentLossyScale.x + " / " + startParentScale.x + " = " + diffX);


            var diffY = currentParentLossyScale.y / startParentScale.y;

            //Debug.Log("diffY: " + currentParentLossyScale.y + " / " + startParentScale.y + " = " + diffY);


            var diffZ = currentParentLossyScale.z / startParentScale.z;

            //Debug.Log("diffZ: " + currentParentLossyScale.z + " / " + startParentScale.z + " = " + diffZ);

            // This inverts the scale differences
            var diffVector = new Vector3(1 / diffX, 1 / diffY, 1 / diffZ);

            //Debug.Log("diff Vector: " + diffVector);


            // Apply the inverted differences to the original scale
            transform.localScale = Vector3.Scale(ScaleToLock, diffVector);


            //Debug.Log("Result: " + transform.localScale);

        }

        //public void MaintainScaleObjectRoot()
        //{
        //    var currentParentParentScale = transform.parent.transform.parent.localScale;

        //    // Get the relative difference to the original scale
        //    var diffX = currentParentParentScale.x / startParentParentScale.x;
        //    var diffY = currentParentParentScale.y / startParentParentScale.y;
        //    var diffZ = currentParentParentScale.z / startParentParentScale.z;

        //    // This inverts the scale differences
        //    var diffVector = new Vector3(1 / diffX, 1 / diffY, 1 / diffZ);

        //    // Apply the inverted differences to the original scale
        //    transform.localScale = Vector3.Scale(startScale, diffVector);

        //}

    }
}