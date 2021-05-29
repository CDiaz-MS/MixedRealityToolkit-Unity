// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [Serializable]
    public class VolumeBounds 
    {
        [SerializeField]
        private Vector3 size;

        public Vector3 Size
        {
            get => size;
            set => size = value;
        }

        [SerializeField]
        private Vector3 center;

        public Vector3 Center
        {
            get => center;
            set => center = value;
        }

        public Quaternion Rotation
        {
            get => HostTransform.rotation;
        }

        [SerializeField]
        private Transform hostTransform;

        public Transform HostTransform
        {
            get => hostTransform;
            set => hostTransform = value;
        }

        public float Width
        {
            get => size.x;
            set => size.x = value;
        }

        public float Height
        {
            get => size.y;
            set => size.y = value;
        }

        public float Depth
        {
            get => size.z;
            set => size.z = value;
        }

        public Vector3 Extents
        {
            get => Size * 0.5f;
        }

        public Vector3 Up
        {
            get => GetDirection(GetFacePoint(FacePoint.Top), GetFacePoint(FacePoint.Bottom));
        }

        public Vector3 Down
        {
            get => GetDirection(GetFacePoint(FacePoint.Bottom), GetFacePoint(FacePoint.Top));
        }

        public Vector3 Left
        {
            get => GetDirection(GetFacePoint(FacePoint.Left), GetFacePoint(FacePoint.Right));
        }

        public Vector3 Right
        {
            get => GetDirection(GetFacePoint(FacePoint.Right), GetFacePoint(FacePoint.Left));
        }

        public Vector3 Forward
        {
            get => GetDirection(GetFacePoint(FacePoint.Forward), GetFacePoint(FacePoint.Back));
        }

        public Vector3 Back
        {
            get => GetDirection(GetFacePoint(FacePoint.Back), GetFacePoint(FacePoint.Forward));
        }

        public VolumeBounds(Vector3 boundsSize, Vector3 boundsCenter, Transform boundsHostTransform)
        {
            Size = boundsSize;
            Center = boundsCenter;
            HostTransform = boundsHostTransform;
        }

        /// <summary>
        /// Calculates the face point positions of the VolumeBounds in world space based on volume size
        /// and the host transform's rotation.
        /// 
        /// Index and Face Point name:
        /// [0] == Top
        /// [1] == Bottom
        /// [2] == Left
        /// [3] == Right
        /// [4] == Back
        /// [5] == Forward
        /// </summary>
        /// <returns>Array of the VolumeBounds face point positions in world space</returns>
        public Vector3[] GetFacePositions()
        {
            Vector3[] localFacePositions = new Vector3[6];

            localFacePositions[0] = Center + (Rotation * (Vector3.up * Extents.y)); // Top
            localFacePositions[1] = Center + (Rotation * (Vector3.down * Extents.y)); // Bottom
            localFacePositions[2] = Center + (Rotation * (Vector3.left * Extents.x)); // Left 
            localFacePositions[3] = Center + (Rotation * (Vector3.right * Extents.x)); // Right
            localFacePositions[4] = Center + (Rotation * (Vector3.forward * Extents.z)); // Back
            localFacePositions[5] = Center + (Rotation * (Vector3.back * Extents.z)); // Forward

            return localFacePositions;
        }

        /// <summary>
        /// Calculates the corner point positions of the VolumeBounds in world space based on volume size
        /// and the host transform's rotation.
        /// 
        /// Index and Corner Point name:
        /// [0] == LeftBottomForward
        /// [1] == LeftBottomBack
        /// [2] == LeftTopForward
        /// [3] == LeftTopBack
        /// [4] == RightBottomForward
        /// [5] == RightBottomBack
        /// [6] == RightTopForward
        /// [7] == RightTopBack
        /// </summary>
        /// <returns>Array of the VolumeBounds corner point positions in world space</returns>
        public Vector3[] GetCornerPositions()
        {
            Vector3[] cornerPositions = new Vector3[8];

            cornerPositions[0] = Center + (Rotation * ((Vector3.left * Extents.x) + (Vector3.back * Extents.z) + (Vector3.down * Extents.y))); 
            cornerPositions[1] = Center + (Rotation * ((Vector3.left * Extents.x) + (Vector3.forward * Extents.z) + (Vector3.down * Extents.y))); 
            cornerPositions[2] = Center + (Rotation * ((Vector3.left * Extents.x) + (Vector3.back * Extents.z) + (Vector3.up * Extents.y))); 
            cornerPositions[3] = Center + (Rotation * ((Vector3.left * Extents.x) + (Vector3.forward * Extents.z) + (Vector3.up * Extents.y)));

            cornerPositions[4] = Center + (Rotation * ((Vector3.right * Extents.x) + (Vector3.back * Extents.z) + (Vector3.down * Extents.y)));
            cornerPositions[5] = Center + (Rotation * ((Vector3.right * Extents.x) + (Vector3.forward * Extents.z) + (Vector3.down * Extents.y)));
            cornerPositions[6] = Center + (Rotation * ((Vector3.right * Extents.x) + (Vector3.back * Extents.z) + (Vector3.up * Extents.y)));
            cornerPositions[7] = Center + (Rotation * ((Vector3.right * Extents.x) + (Vector3.forward * Extents.z) + (Vector3.up * Extents.y)));

            return cornerPositions;
        }

        public Vector3 GetFacePoint(FacePoint facePoint)
        {
            int pointIndex = (int)facePoint;
            return GetFacePositions()[pointIndex];
        }

        public Vector3 GetCornerPoint(CornerPoint cornerPoint)
        {
            int pointIndex = (int)cornerPoint;
            return GetCornerPositions()[pointIndex];
        }

        public Vector3 GetDirection(Vector3 point1, Vector3 point2)
        {
            return (point1 - point2).normalized;
        }

        public Vector3 GetCornerMidPoint(CornerPoint p1, CornerPoint p2)
        {
            return (GetCornerPoint(p1) + GetCornerPoint(p2)) * 0.5f;
        }

        public Vector3 GetFaceMidPoint(FacePoint p1, FacePoint p2)
        {
            return (GetFacePoint(p1) + GetFacePoint(p2)) * 0.5f;
        }

        public bool Contains(Vector3 point)
        {
            if ((point.x > GetFacePoint(FacePoint.Left).x && point.x < GetFacePoint(FacePoint.Right).x) &&
                (point.y > GetFacePoint(FacePoint.Bottom).y && point.y < GetFacePoint(FacePoint.Top).y) &&
                (point.z > GetFacePoint(FacePoint.Forward).z && point.z < GetFacePoint(FacePoint.Back).z))
            {
  
                return true;
            }

            Debug.DrawLine(GetFacePoint(FacePoint.Left), GetFacePoint(FacePoint.Right));
            Debug.DrawLine(GetFacePoint(FacePoint.Top), GetFacePoint(FacePoint.Bottom));
            Debug.DrawLine(GetFacePoint(FacePoint.Forward), GetFacePoint(FacePoint.Back));

            Debug.DrawLine(GetFacePoint(FacePoint.Left), GetFacePoint(FacePoint.Back));
            Debug.DrawLine(GetFacePoint(FacePoint.Back), GetFacePoint(FacePoint.Right));
            Debug.DrawLine(GetFacePoint(FacePoint.Right), GetFacePoint(FacePoint.Forward));
            Debug.DrawLine(GetFacePoint(FacePoint.Forward), GetFacePoint(FacePoint.Left));

            return false;
        }


        public void DrawBounds(Color color)
        {
            // Bottom Edges
            Debug.DrawLine(GetCornerPoint(CornerPoint.LeftBottomBack), GetCornerPoint(CornerPoint.LeftBottomForward), color);
            Debug.DrawLine(GetCornerPoint(CornerPoint.LeftBottomForward), GetCornerPoint(CornerPoint.RightBottomForward), color);
            Debug.DrawLine(GetCornerPoint(CornerPoint.RightBottomForward), GetCornerPoint(CornerPoint.RightBottomBack), color);
            Debug.DrawLine(GetCornerPoint(CornerPoint.RightBottomBack), GetCornerPoint(CornerPoint.LeftBottomBack), color);

            // Top Edges
            Debug.DrawLine(GetCornerPoint(CornerPoint.LeftTopBack), GetCornerPoint(CornerPoint.LeftTopForward), color);
            Debug.DrawLine(GetCornerPoint(CornerPoint.LeftTopForward), GetCornerPoint(CornerPoint.RightTopForward), color);
            Debug.DrawLine(GetCornerPoint(CornerPoint.RightTopForward), GetCornerPoint(CornerPoint.RightTopBack), color);
            Debug.DrawLine(GetCornerPoint(CornerPoint.RightTopBack), GetCornerPoint(CornerPoint.LeftTopBack), color);

            // Side Edges
            Debug.DrawLine(GetCornerPoint(CornerPoint.LeftBottomBack), GetCornerPoint(CornerPoint.LeftTopBack), color);
            Debug.DrawLine(GetCornerPoint(CornerPoint.LeftBottomForward), GetCornerPoint(CornerPoint.LeftTopForward), color);
            Debug.DrawLine(GetCornerPoint(CornerPoint.RightBottomForward), GetCornerPoint(CornerPoint.RightTopForward), color);
            Debug.DrawLine(GetCornerPoint(CornerPoint.RightBottomBack), GetCornerPoint(CornerPoint.RightTopBack), color);
        }
    }
}
