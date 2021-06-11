// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [Serializable]
    public class ChildVolumeItem
    {
        [SerializeField]
        private BaseVolume volume;

        public BaseVolume Volume
        {
            get => volume;
            internal set => volume = value;
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
                    if (IsValidVector(diffVector))
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

            if (Volume == null && Transform.GetComponent<BaseVolume>() != null)
            {
                Volume = Transform.GetComponent<BaseVolume>();
            }
        }

        private Vector3 GetParentContainerSize()
        {
            return Transform.parent.GetComponent<BaseVolume>().VolumeSize;
        }

        public bool IsVolume()
        {
            return Transform.GetComponent<BaseVolume>() != null;
        }

        public bool IsParentUIVolume()
        {
            return Transform.parent.GetComponent<BaseVolume>() != null;
        }

        public bool IsValidVector(Vector3 vector)
        {
            return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
                   !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
        }
    }
}
