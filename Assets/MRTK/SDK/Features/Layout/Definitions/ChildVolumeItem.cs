// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [Serializable]
    public class ChildVolumeItem
    {
        [SerializeField]
        private UIVolume uIVolume;

        public UIVolume UIVolume
        {
            get => uIVolume;
            internal set => uIVolume = value;
        }

        [SerializeField]
        private Transform transform;

        public Transform Transform
        {
            get => transform;
            internal set => transform = value;
        }

        [SerializeField]
        [Tooltip("Maintain the current scale appearance of this transform if the parent scale is changed.")]
        private bool maintainScale = false;

        public bool MaintainScale
        {
            get => maintainScale;
            set => maintainScale = value;
        }

        [SerializeField]
        private Vector3 scaleToLock;

        public Vector3 ScaleToLock
        {
            get => scaleToLock;
            set => scaleToLock = value;
        }

        [SerializeField]
        private Vector3 startParentSize;

        public ChildVolumeItem(Transform transform)
        {
            Transform = transform;

            startParentSize = GetParentContainerSize();

            ScaleToLock = Transform.localScale;
        }

        private void AdjustScale(VolumeSizeOrigin volumeSizeOrigin)
        {
            if (Transform != null)
            {
                Vector3 currentParentContainerSize = GetParentContainerSize();

                // Get differences relative to the parent
                float differenceX = currentParentContainerSize.x / startParentSize.x;
                float differenceY = currentParentContainerSize.y / startParentSize.y;
                float differenceZ = currentParentContainerSize.z / startParentSize.z;

                // Invert differences
                Vector3 diffVector = new Vector3(1 / differenceX, 1 / differenceY, 1 / differenceZ);

                // If the volume origin size is local scale, then it is an empty container
                if (volumeSizeOrigin != VolumeSizeOrigin.LocalScale)
                {
                    if (diffVector.IsValidVector())
                    {
                        Transform.localScale = Vector3.Scale(ScaleToLock, diffVector);
                    }
                }
            }
        }

        public void OnParentScaleChanged(VolumeSizeOrigin volumeSizeOrigin)
        {
            if (MaintainScale)
            {
                AdjustScale(volumeSizeOrigin);
            }
            else
            {
                if (Transform != null)
                {
                    if (Transform.parent.hasChanged)
                    {
                        startParentSize = GetParentContainerSize();
                    }

                    ScaleToLock = Transform.localScale;
                }
            }

            if (UIVolume == null && Transform.GetComponent<UIVolume>() != null)
            {
                UIVolume = Transform.GetComponent<UIVolume>();
            }
        }

        private Vector3 GetParentContainerSize()
        {
            if (Transform.parent.GetComponent<Collider>() != null)
            {
                return Transform.parent.GetColliderBounds().size;
            }
            else
            {
                return Transform.parent.lossyScale;
            }
        }

        public bool IsUIVolume()
        {
            return Transform.GetComponent<UIVolume>() != null;
        }

        public bool IsParentUIVolume()
        {
            return Transform.parent.GetComponent<UIVolume>() != null;
        }
    }
}
