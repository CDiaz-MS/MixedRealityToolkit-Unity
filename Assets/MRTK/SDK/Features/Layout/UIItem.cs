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

        private Vector3 startParentScale;
        private Transform parentTransform;

        private void Start()
        {
            ScaleToLock = transform.localScale;

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

            // Get differences relative to the parent
            float differenceX = currentParentLossyScale.x / startParentScale.x;
            float differenceY = currentParentLossyScale.y / startParentScale.y;
            float differenceZ = currentParentLossyScale.z / startParentScale.z;

            // Invert differences
            var diffVector = new Vector3(1 / differenceX, 1 / differenceY, 1 / differenceZ);

            transform.localScale = Vector3.Scale(ScaleToLock, diffVector);
        }
    }
}