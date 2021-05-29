// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class VolumeEllipse : Volume
    {
        [SerializeField]
        private VolumeEllipseOrientation volumeEllipseOrientation = VolumeEllipseOrientation.Vertical;

        public VolumeEllipseOrientation VolumeEllipseOrientation
        {
            get => volumeEllipseOrientation;
            set => volumeEllipseOrientation = value;
        }

        [SerializeField]
        private bool useCustomRadius = false;

        public bool UseCustomRadius
        {
            get => useCustomRadius;
            set => useCustomRadius = value;
        }

        [SerializeField]
        private float radius = 0;

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        public int Segments
        {
            get => ChildVolumeItems.Count;
        }

        public override void Update()
        {
            base.Update();

            UpdateObjectPositions();
        }

        private void UpdateObjectPositions()
        {
            Vector3[] positions = GetObjectPositions();

            for (int i = 0; i < ChildVolumeItems.Count; i++)
            {
                ChildVolumeItems[i].Transform.position = Application.isPlaying ? Vector3.Lerp(ChildVolumeItems[i].Transform.position, positions[i], Time.deltaTime * 3f) : positions[i];
            }
        }

        private Vector3[] GetObjectPositions()
        {
            float currentRadius = UseCustomRadius ? Radius : VolumeBounds.Extents.x;

            Vector3[] points = new Vector3[Segments];

            for (int i = 0; i < Segments; i++)
            {
                float angle = ((float)i / (float)Segments) * 360 * Mathf.Deg2Rad;

                float sin = Mathf.Sin(angle) * currentRadius;
                float cos = Mathf.Cos(angle) * currentRadius;

                Vector3 targetOffset = new Vector3(sin, VolumeEllipseOrientation == VolumeEllipseOrientation.Vertical ? cos : 0, VolumeEllipseOrientation == VolumeEllipseOrientation.Vertical ? 0 : cos);
                Vector3 centerPos = targetOffset + VolumeCenter;

                Vector3 direction = (centerPos - VolumeCenter).normalized;
                Vector3 newPosition = VolumeCenter + (VolumeBounds.Rotation * (direction * currentRadius));

                points[i] = newPosition;
            }

            return points;
        }
    }
}
