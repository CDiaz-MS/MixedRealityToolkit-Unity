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

        public void ResetContainerFillProperties(UIVolume itemUIVolume)
        {
            ContainerFillX = false;
            ContainerFillY = false;
            ContainerFillZ = false;

            EnsureFillPropertyConfiguration(itemUIVolume);
        }

        public void UpdateDistributeContainerFillAxis(Axis axis, UIVolume itemUIVolume, int childItems)
        {
            if (ContainerFillX || ContainerFillX || ContainerFillX)
            {
                itemUIVolume.SetMaintainScale(false, itemUIVolume.gameObject);
            }

            if (ContainerFillX)
            {
                itemUIVolume.VolumeSizeScaleFactorX = axis == Axis.X ? CalculateScaleFactor(childItems) : 1;
            }

            if (ContainerFillY)
            {
                itemUIVolume.VolumeSizeScaleFactorY = axis == Axis.Y ? CalculateScaleFactor(childItems) : 1;
            }

            if (ContainerFillZ)
            {
                itemUIVolume.VolumeSizeScaleFactorZ = axis == Axis.Z ? CalculateScaleFactor(childItems) : 1;
            }

            EnsureFillPropertyConfiguration(itemUIVolume);
        }

        private void EnsureFillPropertyConfiguration(UIVolume itemUIVolume)
        {
            // X Axis
            if (ContainerFillX)
            {
                if (!itemUIVolume.FillToParentX)
                {
                    itemUIVolume.FillToParentX = true;
                }
            }
            else
            {
                if (itemUIVolume.FillToParentX)
                {
                    itemUIVolume.FillToParentX = false;
                }
            }

            // Y Axis 
            if (ContainerFillY)
            {
                if (!itemUIVolume.FillToParentY)
                {
                    itemUIVolume.FillToParentY = true;
                }
            }
            else
            {
                if (itemUIVolume.FillToParentY)
                {
                    itemUIVolume.FillToParentY = false;
                }
            }

            // Z Axis 
            if (ContainerFillZ)
            {
                if (!itemUIVolume.FillToParentZ)
                {
                    itemUIVolume.FillToParentZ = true;
                }
            }
            else
            {
                if (itemUIVolume.FillToParentZ)
                {
                    itemUIVolume.FillToParentZ = false;
                }
            }
        }
    }
}
