using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [ExecuteAlways]
    public class UIVolume : MonoBehaviour
    {
        [SerializeField]
        private AnchorLocation anchorLocation;

        public AnchorLocation AnchorLocation
        {
            get => anchorLocation;
            set
            {
                anchorLocation = value;
            }
        }

        [SerializeField]
        private UIVolumePoint[] uiVolumeCorners = new UIVolumePoint[8];

        public UIVolumePoint[] UIVolumeCorners
        {
            get => uiVolumeCorners;
        }

        [SerializeField]
        private UIVolumePoint[] uiVolumeFaces = new UIVolumePoint[6];

        public UIVolumePoint[] UIVolumeFaces
        {
            get => uiVolumeFaces;
        }

        [SerializeField]
        private bool xAxisDynamicDistribute = false;

        public bool XAxisDynamicDistribute
        {
            get => xAxisDynamicDistribute;
            set
            {
                xAxisDynamicDistribute = value;
            }
        }

        [SerializeField]
        private bool yAxisDynamicDistribute = false;

        public bool YAxisDynamicDistribute
        {
            get => yAxisDynamicDistribute;
            set => yAxisDynamicDistribute = value;
        }

        [SerializeField]
        private bool zAxisDynamicDistribute = false;

        public bool ZAxisDynamicDistribute
        {
            get => zAxisDynamicDistribute;
            set => zAxisDynamicDistribute = value;
        }

        [SerializeField]
        private bool fillToParentY = false;

        public bool FillToParentY
        {
            get => fillToParentY;
            set => fillToParentY = value;
        }

        [SerializeField]
        private bool fillToParentX = false;

        public bool FillToParentX
        {
            get => fillToParentX;
            set => fillToParentX = value;
        }

        [SerializeField]
        private bool fillToParentZ = false;

        public bool FillToParentZ
        {
            get => fillToParentZ;
            set => fillToParentZ = value;
        }

        [SerializeField]
        private bool drawCornerPoints = false;

        public bool DrawCornerPoints
        {
            get => drawCornerPoints;
            set => drawCornerPoints = value;
        }

        [SerializeField]
        private bool drawFacePoints = false;

        public bool DrawFacePoints
        {
            get => drawFacePoints;
            set => drawFacePoints = value;
        }

        [SerializeField]
        private bool anchorPositionOverrideEnabled = true;

        public bool AnchorPositionOverrideEnabled
        {
            get => anchorPositionOverrideEnabled;
            set => anchorPositionOverrideEnabled = value;
        }

        [SerializeField]
        private GameObject backPlate;

        public GameObject BackPlate
        {
            get => backPlate;
            set => backPlate = value;
        }

        private bool useColliderBounds => GetComponent<Collider>() != null;

        protected string[] cornerNames = Enum.GetNames(typeof(CornerPoint)).ToArray();
        protected string[] faceNames = Enum.GetNames(typeof(FacePoint)).ToArray();

        public Transform rootTransform => transform.parent == null || (transform.parent.GetComponent<UIVolume>() == null) ? transform : transform.parent;

        public Transform UIVolumeParentTransform => transform != rootTransform ? transform.parent.GetComponent<UIVolume>().transform : GetComponent<UIVolume>().transform;

        public bool IsRootUIVolume => (transform == rootTransform);

        public Vector3 VolumeCenter => transform.position;

        #region Gizmos
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = IsRootUIVolume ? Color.green : Color.cyan;

            if (useColliderBounds)
            {
                Gizmos.DrawWireCube(gameObject.transform.position, gameObject.transform.GetColliderBounds().size);

            }
            else
            {
                Gizmos.DrawWireCube(gameObject.transform.position, gameObject.transform.localScale);
            }

            if (DrawCornerPoints)
            {
                Gizmos.color = Color.magenta;
                foreach (var point in UIVolumeCorners)
                {
                    Gizmos.DrawSphere(point.Point, 0.02f);
                }
            }

            if (DrawFacePoints)
            {
                Gizmos.color = Color.yellow;
                foreach (var point in UIVolumeFaces)
                {
                    Gizmos.DrawSphere(point.Point, 0.02f);
                }
            }
        }

        #endregion

        #region MonoBehaviour Methods

        protected virtual void OnEnable()
        {
            InitializePoints();
        }

        protected virtual void Update()
        {
            UpdateCornerPoints();
            UpdateFacePoints();

            if (XAxisDynamicDistribute)
            {
                Distribute(Axis.X);
            }

            if (YAxisDynamicDistribute)
            {
                Distribute(Axis.Y);
            }

            if (ZAxisDynamicDistribute)
            {
                Distribute(Axis.Z);
            }

            if (FillToParentX)
            {
                if (useColliderBounds)
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(UIVolumeParentTransform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
            }

            if (FillToParentY)
            {
                if (useColliderBounds)
                {
                    transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(transform.localScale.x, UIVolumeParentTransform.localScale.y, transform.localScale.z);
                }
            }

            if (FillToParentZ)
            {
                if (useColliderBounds)
                {
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
                }
                else
                {
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, UIVolumeParentTransform.localScale.x);

                }
            }

            if (anchorPositionOverrideEnabled && !IsRootUIVolume)
            {
                ChangeAnchorLocation(AnchorLocation);
            }

            UpdateCornerPoints();
            UpdateFacePoints();
        }

        #endregion

        #region Corner and Face Point Calculations

        private void InitializePoints()
        {
            for (int i = 0; i < UIVolumeCorners.Length; i++)
            {
                UIVolumeCorners[i] = new UIVolumePoint(cornerNames[i]);
            }

            for (int i = 0; i < UIVolumeFaces.Length; i++)
            {
                UIVolumeFaces[i] = new UIVolumePoint(faceNames[i]);
            }
        }

        private Vector3 CalculateVolumeSizeOffset()
        {
            float xOffset = useColliderBounds ? transform.GetColliderBounds().extents.x : transform.localScale.x * 0.5f;
            float yOffset = useColliderBounds ? transform.GetColliderBounds().extents.y : transform.localScale.y * 0.5f;
            float zOffset = useColliderBounds ? transform.GetColliderBounds().extents.z : transform.localScale.z * 0.5f;

            return new Vector3(xOffset, yOffset, zOffset);
        }

        private void UpdateCornerPoints()
        {
            Vector3 volumeSizeOffset = CalculateVolumeSizeOffset();

            for (int i = 0; i < UIVolumeCorners.Length; i++)
            {
                string[] pointNameParse = NameParse(UIVolumeCorners[i].PointName);

                float positionX = pointNameParse[0] == "Left" ? transform.position.x - (volumeSizeOffset.x) : transform.position.x + (volumeSizeOffset.x);
                float positionY = pointNameParse[1] == "Top" ? transform.position.y + (volumeSizeOffset.y) : transform.position.y - (volumeSizeOffset.y);
                float positionZ = pointNameParse[2] == "Forward" ? transform.position.z - (volumeSizeOffset.z) : transform.position.z + (volumeSizeOffset.z);

                UIVolumeCorners[i].Point = new Vector3(positionX, positionY, positionZ);
            }
        }

        private void UpdateFacePoints()
        {
            Vector3 volumeSizeOffset = CalculateVolumeSizeOffset();

            for (int i = 0; i < UIVolumeFaces.Length; i++)
            {
                float positionX = transform.position.x;
                float positionY = transform.position.y;
                float positionZ = transform.position.z;


                if (UIVolumeFaces[i].PointName == "Left")
                {
                    positionX = transform.position.x - (volumeSizeOffset.x);   
                }
                else if (UIVolumeFaces[i].PointName == "Right")
                {
                    positionX = transform.position.x + (volumeSizeOffset.x);
                }
                else if (UIVolumeFaces[i].PointName == "Top")
                {
                    positionY = transform.position.y + volumeSizeOffset.y;
                }
                else if (UIVolumeFaces[i].PointName == "Bottom")
                {
                    positionY = transform.position.y - volumeSizeOffset.y;
                }
                else if (UIVolumeFaces[i].PointName == "Forward")
                {
                    positionZ = transform.position.z - volumeSizeOffset.z;
                }
                else 
                {
                    positionZ = transform.position.z + volumeSizeOffset.z;
                }

                UIVolumeFaces[i].Point = new Vector3(positionX, positionY, positionZ);
            }
        }

        public Vector3 GetFacePoint(FacePoint name)
        {
            return ArrayUtility.Find(UIVolumeFaces, (point) => point.PointName == name.ToString()).Point;
        }

        public Vector3 GetCornerPoint(CornerPoint name)
        {
            return ArrayUtility.Find(UIVolumeCorners, (point) => point.PointName == name.ToString()).Point;
        }

        public Vector3 GetCornerMidPoint(CornerPoint p1, CornerPoint p2)
        {
            return (GetCornerPoint(p1) + GetCornerPoint(p2)) * 0.5f;
        }

        private string[] NameParse(string name)
        {
            string pointNameStringSpaces = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");

            string[] words = pointNameStringSpaces.Split(' ');

            return words;
        }

        #endregion


        public void ChangeAnchorLocation(AnchorLocation anchorLocation)
        {
            float positionXOffset = (UIVolumeParentTransform.localScale.x / 2);
            float positionYOffset = (UIVolumeParentTransform.localScale.y / 2);
            float positionZOffset = (UIVolumeParentTransform.localScale.z / 2);

            Vector3 volumeSizeOffset = CalculateVolumeSizeOffset();

            float positionX;
            float positionY;
            float positionZ;

            string[] anchorParsed = NameParse(anchorLocation.ToString());

            // Y Position
            if (anchorParsed[0] == "Top")
            {
                positionY = UIVolumeParentTransform.position.y + (positionYOffset) + -(volumeSizeOffset.y);
            }
            else if (anchorParsed[0] == "Center")
            {
                positionY = UIVolumeParentTransform.position.y;
            }
            else
            {
                positionY = UIVolumeParentTransform.position.y + -(positionYOffset) + (volumeSizeOffset.y);
            }

            // X Position
            if (anchorParsed[1] == "Left")
            {
                positionX = UIVolumeParentTransform.position.x + -(positionXOffset) + (volumeSizeOffset.x);
            }
            else if (anchorParsed[1] == "Center")
            {
                positionX = UIVolumeParentTransform.position.x;
            }
            else
            {
                positionX = UIVolumeParentTransform.position.x + (positionXOffset) + -(volumeSizeOffset.x);
            }

            // Z Position
            if (anchorParsed[2] == "Forward")
            {
                positionZ = UIVolumeParentTransform.position.z + -(positionZOffset) + (volumeSizeOffset.z);
            }
            else if (anchorParsed[2] == "Center")
            {
                positionZ = UIVolumeParentTransform.position.z;
            }
            else
            {
                positionZ = UIVolumeParentTransform.position.z + (positionZOffset) + -(volumeSizeOffset.z);
            }

            transform.position = new Vector3(positionX, positionY, positionZ);
        }

        #region Axis Distribution

        public void Distribute(Axis axis)
        {
            if (gameObject.transform.childCount > 0)
            {
                List<Transform> items = new List<Transform>();

                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    items.Add(transform.GetChild(i));
                }

                float placementIncrement;
                float startPlacement;

                Bounds bounds = items[0].transform.GetColliderBounds();

                if (axis == Axis.X)
                {
                    float width = transform.localScale.x;

                    placementIncrement = width / items.Count;

                    startPlacement = GetFacePoint(FacePoint.Left).x + bounds.extents.x;
                }
                else if (axis == Axis.Y)
                {
                    float height = transform.localScale.y;

                    placementIncrement = height / items.Count;

                    startPlacement = GetFacePoint(FacePoint.Bottom).y + bounds.extents.y;
                }
                else // Z
                {
                    float depth = transform.localScale.z;

                    placementIncrement = depth / items.Count;

                    startPlacement = GetFacePoint(FacePoint.Forward).z + bounds.extents.z;
                }

                foreach (var item in items)
                {
                    float newPositionX = axis == Axis.X ? startPlacement : transform.position.x;
                    float newPositionY = axis == Axis.Y ? startPlacement : transform.position.y;
                    float newPositionZ = axis == Axis.Z ? startPlacement : transform.position.z;

                    Vector3 newPosition = new Vector3(newPositionX, newPositionY, newPositionZ);

                    item.position = newPosition;

                    startPlacement += placementIncrement;
                }
            }
        }

        #endregion
    }
}
