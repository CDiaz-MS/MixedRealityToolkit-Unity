// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class VolumeCurve : Volume
    {
        [SerializeField]
        private Vector3[] curvePoints = new Vector3[3];

        public Vector3[] CurvePoints
        {
            get => curvePoints;
            set => curvePoints = value;
        }

        [SerializeField]
        private bool adjustToCurve = false;

        public bool AdjustToCurve
        {
            get => adjustToCurve;
            set => adjustToCurve = value;
        }

        [SerializeField]
        private bool backPointOverride = false;

        public bool BackPointOverride
        {
            get => backPointOverride;
            set => backPointOverride = value;
        }

        [SerializeField]
        private int lineSteps = 10;

        public int LineSteps
        {
            get => lineSteps;
            set => lineSteps = value;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Vector3[] curvePositions = CalculateCurvePositions();

            Gizmos.color = Color.blue;

            foreach (var point in CurvePoints)
            {
                Gizmos.DrawSphere(point, 0.02f);
            }

            Gizmos.color = Color.yellow;

            Gizmos.color = Color.cyan;


            for (int i = 0; i < curvePositions.Length; i++)
            {
                Gizmos.DrawSphere(curvePositions[i], 0.005f);
                
                if (i != curvePositions.Length - 1)
                {
                    Gizmos.DrawLine(curvePositions[i], curvePositions[i + 1]);
                }
            }
        }


        private Vector3[] CalculateCurvePositions()
        {
            Vector3[] curvePositions = new Vector3[LineSteps];

            Vector3 lineStart = Point(0);

            for (int j = 1; j <= lineSteps; j++)
            {
                Vector3 lineEnd = Point(j / (float)lineSteps);

                curvePositions[j - 1] = lineStart;

                lineStart = lineEnd;
            }

            return curvePositions;
        }

        private Vector3 Point(float t)
        {
            return GetPoint(CurvePoints[0], CurvePoints[1], CurvePoints[2], t);
        }

        private Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(p0, p2, t), Vector3.Lerp(p1, p2, t), t);
        }

        protected override void Start()
        {
            base.Start();

            InitializeCurvePointPositions();
        }

        private void InitializeCurvePointPositions()
        {
            CurvePoints[0] = GetCornerMidPoint(CornerPoint.LeftTopForward, CornerPoint.LeftBottomForward);
            CurvePoints[1] = GetFacePoint(FacePoint.Back);
            CurvePoints[2] = GetCornerMidPoint(CornerPoint.RightTopForward, CornerPoint.RightBottomForward);
        }

        public override void Update()
        {
            base.Update();

            UpdateCurvePoints();

            SetObjectPositions();
        }

        private void UpdateCurvePoints()
        {
            CurvePoints[0] = GetCornerMidPoint(CornerPoint.LeftTopForward, CornerPoint.LeftBottomForward);
            CurvePoints[1] = GetFacePoint(FacePoint.Back);
            CurvePoints[2] = GetCornerMidPoint(CornerPoint.RightTopForward, CornerPoint.RightBottomForward);

            Debug.DrawLine(CurvePoints[0], CurvePoints[1]);
            Debug.DrawLine(CurvePoints[1], CurvePoints[2]);
        }

        private void SetObjectPositions()
        {
            Vector3[] curvePositions = CalculateCurvePositions();

            for (int i = 1; i < ChildVolumeItems.Count; i++)
            {
                ChildVolumeItems[i].Transform.position = curvePositions[i];
            }
        }
    }
}
