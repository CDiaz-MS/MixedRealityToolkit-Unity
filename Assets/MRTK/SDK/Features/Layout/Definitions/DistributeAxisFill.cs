// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [Serializable]
    public class DistributeAxisFill
    {
        [SerializeField]
        private VolumeAxis distributeAxis;

        public VolumeAxis DistributeAxis
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

        public DistributeAxisFill(VolumeAxis axis)
        {
            distributeAxis = axis;
        }

        public float CalculateScaleFactor(int childCount)
        {
            return Mathf.Pow(childCount, -1);
        }

        public void ResetContainerFillProperties(VolumeAnchorPosition itemUIVolume)
        {
            ContainerFillX = false;
            ContainerFillY = false;
            ContainerFillZ = false;

            EnsureFillPropertyConfiguration(itemUIVolume);
        }

        public void UpdateDistributeContainerFillAxis(VolumeAxis axis, VolumeAnchorPosition itemUIVolume, int childItems)
        {
            if (ContainerFillX || ContainerFillX || ContainerFillX)
            {
                itemUIVolume.Volume.SetMaintainScale(false, itemUIVolume.gameObject);
            }

            if (ContainerFillX)
            {
                itemUIVolume.VolumeSizeScaleFactorX = axis == VolumeAxis.X ? CalculateScaleFactor(childItems) : 1;
            }

            if (ContainerFillY)
            {
                itemUIVolume.VolumeSizeScaleFactorY = axis == VolumeAxis.Y ? CalculateScaleFactor(childItems) : 1;
            }

            if (ContainerFillZ)
            {
                itemUIVolume.VolumeSizeScaleFactorZ = axis == VolumeAxis.Z ? CalculateScaleFactor(childItems) : 1;
            }

            EnsureFillPropertyConfiguration(itemUIVolume);
        }

        private void EnsureFillPropertyConfiguration(VolumeAnchorPosition itemUIVolume)
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
