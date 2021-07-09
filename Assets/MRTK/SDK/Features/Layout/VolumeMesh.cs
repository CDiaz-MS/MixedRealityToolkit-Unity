// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class VolumeMesh : BaseCustomVolume
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
            Tuple<Vector3[], Quaternion[]> positionsRotations = GetMeshPositionsRotations();

            if (positionsRotations != null)
            {
                Volume.SetChildVolumePositions(positionsRotations.Item1);
                Volume.SetChildVolumeRotations(positionsRotations.Item2);
            }
        }

        private Tuple<Vector3[],Quaternion[]> GetMeshPositionsRotations()
        {
            if (Mesh != null)
            {
                Vector3[] vertices = mesh.vertices;
                Vector3[] objectPositions = new Vector3[vertices.Length];

                Quaternion[] rotations = new Quaternion[vertices.Length];

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

                for (int i = 0; i < normals.Length; i++)
                {
                    rotations[i] = Quaternion.FromToRotation(transform.up, normals[i]);

                }

                return Tuple.Create(objectPositions, rotations);
            }

            return null;
        }
    }
}
