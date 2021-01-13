using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class UIVolumeCurve : UIVolume
    {
        public UIVolumePoint[] CurvePoints = new UIVolumePoint[3];

        [SerializeField]
        private bool adjustToCurve = false;

        public bool AdjustToCurve
        {
            get => adjustToCurve;
            set
            {
                adjustToCurve = value;
            }
        }

        [SerializeField]
        private bool backPointOverride = false;

        public bool BackPointOverride
        {
            get => backPointOverride;
            set
            {
                backPointOverride = value;
            }
        }


        private int lineSteps = 10;

        private Vector3[] curvePositions = new Vector3[10];

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.blue;

            foreach (var point in CurvePoints)
            {
                Gizmos.DrawSphere(point.Point, 0.02f);
            }

            Debug.DrawLine(GetCurvePoint("Left"), GetCurvePoint("Back"));
            Debug.DrawLine(GetCurvePoint("Back"), GetCurvePoint("Right"));

            Gizmos.color = Color.yellow;

            Vector3 lineStart = Point(0);

            for (int j = 1; j <= lineSteps; j++)
            {
                Vector3 lineEnd = Point(j / (float)lineSteps);

                curvePositions[j - 1] = lineStart;

                Gizmos.DrawSphere(lineStart, 0.01f);

                Gizmos.DrawLine(lineStart, lineEnd);

                lineStart = lineEnd;
            }

            Gizmos.color = Color.cyan;
            foreach (var point in curvePositions)
            {
                Gizmos.DrawSphere(point, 0.01f);
            }
        }

        private Vector3 Point(float t)
        {
            return GetPoint(GetCurvePoint("Left"), GetCurvePoint("Back"), GetCurvePoint("Right"), t);
        }

        private Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(p0, p2, t), Vector3.Lerp(p1, p2, t), t);
        }

        void Start()
        {
            CurvePoints[0] = new UIVolumePoint("Left");
            CurvePoints[1] = new UIVolumePoint("Back");
            CurvePoints[2] = new UIVolumePoint("Right");
        }

        private Vector3 GetCurvePoint(string name)
        {
            return ArrayUtility.Find(CurvePoints, (point) => point.PointName == name).Point;
        }

        protected override void Update()
        {
            base.Update();

            UpdateCurvePoints();

            if (AdjustToCurve)
            {
                CurvePositions();
            }
        }

        private void UpdateCurvePoints()
        {
            if (!BackPointOverride)
            {
                CurvePoints[1].Point = GetFacePoint(FacePoint.Back);
            }

            CurvePoints[0].Point = GetCornerMidPoint(CornerPoint.LeftTopForward, CornerPoint.LeftBottomForward);
            CurvePoints[2].Point = GetCornerMidPoint(CornerPoint.RightTopForward, CornerPoint.RightBottomForward);
        }

        private void CurvePositions()
        {
            List<Transform> childTransforms = GetComponentsInChildren<Transform>().ToList();

            for (int i = 1; i < childTransforms.Count; i++)
            {
                childTransforms[i].position = curvePositions[i];
            }
        }
    }
}
