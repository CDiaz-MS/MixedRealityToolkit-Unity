// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class VolumeMesh : Volume
    {
        [SerializeField]
        private Mesh mesh;

        public Mesh Mesh
        {
            get => mesh;
            set => mesh = value;
        }

        public override void Update()
        {
            base.Update();

            UpdateObjectPositions();
        }

        private void UpdateObjectPositions()
        {
            Vector3[] positions = GetMeshPositions();

            int vertexCount = positions.Length;
            int childCount = ChildVolumeItems.Count;

            if (childCount != 0)
            {
                for (int i = 0; i < ChildVolumeItems.Count; i++)
                {
                    ChildVolumeItems[i].Transform.position = positions[i];
                }
            }
        }

        private Vector3[] GetMeshPositions()
        {
            if (Mesh != null)
            {
                Vector3[] vertices = mesh.vertices;
                Vector3[] objectPositions = new Vector3[vertices.Length];

                for (int i = 0; i < objectPositions.Length; i++)
                {
                    Vector3 targetPosition = vertices[i];
                    Vector3 centerPosition = targetPosition + VolumeCenter;
                    Vector3 direction = (centerPosition - VolumeCenter).normalized;

                    float magnitude = Vector3.Distance(VolumeCenter, centerPosition);

                    objectPositions[i] += VolumeCenter + (VolumeBounds.Rotation * (direction * magnitude));
                }

                Vector3[] normals = mesh.normals;

                for (int i = 0; i < objectPositions.Length; i++)
                { 
                    Debug.DrawRay(objectPositions[i], normals[i] * 0.2f, Color.green);
                    Debug.DrawRay(objectPositions[i], Vector3.down * 0.2f, Color.red);
                }

                return objectPositions;
            }

            return null;

        }

    }
}
