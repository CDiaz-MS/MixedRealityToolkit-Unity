﻿// Copyright (c) Microsoft Corporation.
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
            set
            {
                scaleToLock = value;
            }
        }

        [SerializeField]
        private Vector3 startParentSize;

        public ChildVolumeItem(Transform transform)
        {
            Transform = transform;

            startParentSize = GetParentContainerSize();
        }

        private void AdjustScale()
        {
            if (Transform != null)
            {
                Vector3 currentParentContainerSize = GetParentContainerSize();

                // Get differences relative to the parent
                float differenceX = currentParentContainerSize.x / startParentSize.x;
                float differenceY = currentParentContainerSize.y / startParentSize.y;
                float differenceZ = currentParentContainerSize.z / startParentSize.z;

                // Invert differences
                var diffVector = new Vector3(1 / differenceX, 1 / differenceY, 1 / differenceZ);

                Transform.localScale = Vector3.Scale(ScaleToLock, diffVector);
            }
        }

        public void OnParentScaleChanged()
        {
            if (MaintainScale)
            {
                AdjustScale();
            }
            else
            {
                if (Transform.parent.hasChanged)
                {
                    startParentSize = GetParentContainerSize();
                }

                ScaleToLock = Transform.localScale;
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
    }
}
