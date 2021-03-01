// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
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
        private VolumeSizeOrigin volumeSizeOrigin;

        public VolumeSizeOrigin VolumeSizeOrigin
        {
            get => volumeSizeOrigin;
            private set => volumeSizeOrigin = value;
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

        #region Distribute Properties

        // X Axis
        [SerializeField]
        private bool xAxisDynamicDistribute = false;

        public bool XAxisDynamicDistribute
        {
            get => xAxisDynamicDistribute;
            set => xAxisDynamicDistribute = value;
        }

        [SerializeField]
        private float leftMargin = 0;

        // X Axis
        public float LeftMargin
        {
            get => leftMargin;
            set => leftMargin = value;
        }

        [SerializeField]
        private DistributeAxisFill distributeContainerFillX = new DistributeAxisFill(Axis.X);

        public DistributeAxisFill DistributeContainerFillX
        {
            get => distributeContainerFillX;
            set => distributeContainerFillX = value;
        }

        // Y Axis
        [SerializeField]
        private bool yAxisDynamicDistribute = false;

        public bool YAxisDynamicDistribute
        {
            get => yAxisDynamicDistribute;
            set => yAxisDynamicDistribute = value;
        }

        [SerializeField]
        private float topMargin = 0;

        public float TopMargin
        {
            get => topMargin;
            set => topMargin = value;
        }

        [SerializeField]
        private DistributeAxisFill distributeContainerFillY = new DistributeAxisFill(Axis.Y);

        public DistributeAxisFill DistributeContainerFillY
        {
            get => distributeContainerFillY;
            set => distributeContainerFillY = value;
        }

        [SerializeField]
        private bool zAxisDynamicDistribute = false;

        public bool ZAxisDynamicDistribute
        {
            get => zAxisDynamicDistribute;
            set => zAxisDynamicDistribute = value;
        }

        [SerializeField]
        private float forwardMargin = 0;

        public float ForwardMargin
        {
            get => forwardMargin;
            set => forwardMargin = value;
        }

        [SerializeField]
        private DistributeAxisFill distributeContainerFillZ = new DistributeAxisFill(Axis.Z);

        public DistributeAxisFill DistributeContainerFillZ
        {
            get => distributeContainerFillZ;
            set => distributeContainerFillZ = value;
        }

        [SerializeField]
        private float backMargin = 0;

        public float BackMargin
        {
            get => backMargin;
            set => backMargin = value;
        }

        #endregion

        #region Volume Size Properties

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
        private float volumeSizeScaleFactorX = 1;

        public float VolumeSizeScaleFactorX
        {
            get => volumeSizeScaleFactorX;
            set => volumeSizeScaleFactorX = value;
        }

        [SerializeField]
        private float volumeSizeScaleFactorY = 1;

        public float VolumeSizeScaleFactorY
        {
            get => volumeSizeScaleFactorY;
            set => volumeSizeScaleFactorY = value;
        }

        [SerializeField]
        private float volumeSizeScaleFactorZ = 1;

        public float VolumeSizeScaleFactorZ
        {
            get => volumeSizeScaleFactorZ;
            set => volumeSizeScaleFactorZ = value;
        }

        #endregion

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
        private bool useAnchorPositioning = true;

        public bool UseAnchorPositioning
        {
            get => useAnchorPositioning;
            set => useAnchorPositioning = value;
        }

        [SerializeField]
        private List<ChildVolumeItem> childVolumeItems = new List<ChildVolumeItem>();

        public List<ChildVolumeItem> ChildVolumeItems
        {
            get => childVolumeItems;
            private set => childVolumeItems = value;
        }

        [SerializeField]
        private GameObject backPlateObject;

        public GameObject BackPlateObject
        {
            get => backPlateObject;
            set => backPlateObject = value;
        }

        protected string[] cornerNames = Enum.GetNames(typeof(CornerPoint)).ToArray();
        protected string[] faceNames = Enum.GetNames(typeof(FacePoint)).ToArray();

        public Transform rootTransform => transform.parent == null || (transform.parent.GetComponent<UIVolume>() == null) ? transform : transform.parent;

        public Transform UIVolumeParentTransform => transform != rootTransform ? transform.parent.GetComponent<UIVolume>().transform : GetComponent<UIVolume>().transform;

        public bool IsRootUIVolume => (transform == rootTransform);

        public Vector3 VolumeCenter => transform.position;

        private int currentChildCount;

        #region Gizmos
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = IsRootUIVolume ? Color.green : Color.cyan;

            DrawVolumeContainer();

            DrawCornerAndFacePoints();

            DrawMargins();
        }

        private void DrawVolumeContainer()
        {
            if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
            {
                Gizmos.DrawWireCube(gameObject.transform.position, GetVolumeSize());

            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.TextMeshPro)
            {
                RectTransform rectTransfrom = transform as RectTransform;

                TextMeshPro textPro = gameObject.GetComponent<TextMeshPro>();

                float marginOffsetXPosition = gameObject.transform.position.x - ((textPro.margin.z * 0.5f) * rectTransfrom.lossyScale.x);
                float marginOffsetYPosition = gameObject.transform.position.y - ((textPro.margin.y * 0.5f) * rectTransfrom.lossyScale.y);

                Vector3 volumeOriginPosition = new Vector3(marginOffsetXPosition, marginOffsetYPosition, gameObject.transform.position.z);

                Gizmos.DrawWireCube(volumeOriginPosition, GetVolumeSize());
            }
            else
            {
                Gizmos.DrawWireCube(gameObject.transform.position, GetVolumeSize());
            }
        }

        private void DrawCornerAndFacePoints()
        {
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

        private void DrawMargins()
        {
            Gizmos.color = Color.yellow;

            // Left Margin
            Vector3 leftMarginStartPoint = GetFacePoint(FacePoint.Left);
            float volumeWidth = Vector3.Distance(GetFacePoint(FacePoint.Left), GetFacePoint(FacePoint.Right));
            Gizmos.DrawLine(leftMarginStartPoint, leftMarginStartPoint + (Vector3.right * (LeftMargin * volumeWidth)));

            // Top Margin
            Vector3 topMarginStartPoint = GetFacePoint(FacePoint.Top);
            float volumeHeight = Vector3.Distance(GetFacePoint(FacePoint.Top), GetFacePoint(FacePoint.Bottom));
            Gizmos.DrawLine(topMarginStartPoint, topMarginStartPoint + (Vector3.down * (TopMargin * volumeHeight)));

            // Forward Margin
            Vector3 forwardMarginStartPoint = GetFacePoint(FacePoint.Forward);
            float volumeDepth = Vector3.Distance(GetFacePoint(FacePoint.Forward), GetFacePoint(FacePoint.Back));
            Gizmos.DrawLine(forwardMarginStartPoint, forwardMarginStartPoint + (Vector3.forward * (ForwardMargin * volumeDepth)));
        }

        private void DrawDebugTextMeshProContainer()
        {
            if (VolumeSizeOrigin == VolumeSizeOrigin.TextMeshPro)
            {
                RectTransform rectTransfrom = transform as RectTransform;

                TextMeshPro textPro = gameObject.GetComponent<TextMeshPro>();

                float xOffset = rectTransfrom.lossyScale.x;
                float widthOffset = rectTransfrom.rect.width * 0.5f;
                float scaleWithOffset = xOffset * widthOffset;
                float marginRightOffset = -textPro.margin.z * xOffset;
                float finalRightOffset = scaleWithOffset + marginRightOffset;

                float marginLeftOffset = -textPro.margin.x * xOffset;
                float finalLeftOffset = scaleWithOffset + marginLeftOffset;

                Vector3 right = rectTransfrom.position + (Vector3.right * finalRightOffset);
                Vector3 left = rectTransfrom.position + (Vector3.left * finalLeftOffset);

                Gizmos.DrawSphere(right, 0.05f);
                Gizmos.DrawSphere(left, 0.05f);
            }
        }

        #endregion

        #region MonoBehaviour Methods

        protected virtual void OnEnable()
        {
            InitializePoints();
        }

        protected virtual void Start() { }

        protected virtual void Update()
        {
            SetVolumeSizeOrigin();

            UpdateCornerPoints();
            UpdateFacePoints();

            UpdateDistribution();

            UpdateVolumeSizeMatch();

            if (useAnchorPositioning && !IsRootUIVolume)
            {
                ChangeAnchorLocation(AnchorLocation);
            }

            UpdateCornerPoints();
            UpdateFacePoints();

            PopulateChildObjects();
            MaintainScaleChildItems();
            
            currentChildCount = transform.childCount;
        }

        #endregion

        private void UpdateVolumeSizeMatch()
        {
            if (FillToParentX)
            {
                SetMatchParentVolumeSize(Axis.X);
            }

            if (FillToParentY)
            {
                SetMatchParentVolumeSize(Axis.Y);
            }

            if (FillToParentZ)
            {
                SetMatchParentVolumeSize(Axis.Z);
            }
        }

        private void SetMatchParentVolumeSize(Axis axis, bool includeScaleFactor = true)
        {
            Vector3 newScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (axis == Axis.X)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.x = includeScaleFactor ? (UIVolumeParentTransform.localScale.x / UIVolumeParentTransform.transform.lossyScale.x) * volumeSizeScaleFactorX : (UIVolumeParentTransform.localScale.x / UIVolumeParentTransform.transform.lossyScale.x);
                
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.x *= 32;
                    }
                }
                else
                {
                    newScale.x = includeScaleFactor ? UIVolumeParentTransform.localScale.x * volumeSizeScaleFactorX : UIVolumeParentTransform.localScale.x;
                }
            }

            if (axis == Axis.Y)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.y = includeScaleFactor ? (UIVolumeParentTransform.localScale.y / UIVolumeParentTransform.transform.lossyScale.y) * volumeSizeScaleFactorY : (UIVolumeParentTransform.localScale.y / UIVolumeParentTransform.transform.lossyScale.y);

                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.y *= 32;
                    }
                }
                else
                {
                    newScale.y = includeScaleFactor ? UIVolumeParentTransform.localScale.y * volumeSizeScaleFactorY : UIVolumeParentTransform.localScale.y;
                }
            }

            if (axis == Axis.Z)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.z = includeScaleFactor ? (UIVolumeParentTransform.localScale.z / UIVolumeParentTransform.transform.lossyScale.z) * volumeSizeScaleFactorZ : (UIVolumeParentTransform.localScale.z / UIVolumeParentTransform.transform.lossyScale.z);

                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.z *= 16;
                    }
                }
                else
                {
                    newScale.z = includeScaleFactor ? UIVolumeParentTransform.localScale.z * volumeSizeScaleFactorZ : UIVolumeParentTransform.localScale.z;
                }
            }

            transform.localScale = newScale;
        }

        public void EqualizeVolumeSizeToParent()
        {
            var axisValues = Enum.GetValues(typeof(Axis));

            foreach (Axis axis in axisValues)
            {
                if (axis != Axis.None)
                {
                    SetMatchParentVolumeSize(axis, false);
                }
            }
        }

        #region Maintain Scale 

        private void PopulateChildObjects()
        {
            if (currentChildCount != transform.childCount)
            {
                if (currentChildCount < transform.childCount)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        var volumeTransform = ChildVolumeItems.Find((volume) => volume.Transform == transform.GetChild(i).transform);
                        if (volumeTransform == null)
                        {
                            ChildVolumeItems.Add(new ChildVolumeItem(transform.GetChild(i)));
                        }
                    }
                }
                else
                {
                    List<Transform> directChildren = new List<Transform>();

                    for (int i = 0; i < transform.childCount; i++)
                    {
                        directChildren.Add(transform.GetChild(i));
                    }

                    foreach (var volume in ChildVolumeItems.ToList())
                    {
                        if (!directChildren.Contains(volume.Transform))
                        {
                            ChildVolumeItems.Remove(volume);
                        }
                    }
                }
            }
        }

        private void MaintainScaleChildItems()
        {
            int i = 0;

            foreach(var childVolumeItem in ChildVolumeItems)
            {
                // If the transform has switched to a rect transform, make sure to update it
                if (childVolumeItem.Transform == null)
                {
                    childVolumeItem.Transform = transform.GetChild(i);
                }

                VolumeSizeOrigin childVolumeSizeOrigin = childVolumeItem.IsUIVolume() ? childVolumeItem.Transform.GetComponent<UIVolume>().VolumeSizeOrigin : VolumeSizeOrigin.None;

                childVolumeItem.OnParentScaleChanged(childVolumeSizeOrigin);

                i++;
            }
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
            Vector3 volumeSizeOffset = new Vector3(0, 0, 0);

            if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
            {
                volumeSizeOffset.x = transform.GetColliderBounds().extents.x;
                volumeSizeOffset.y = transform.GetColliderBounds().extents.y;
                volumeSizeOffset.z = transform.GetColliderBounds().extents.z;

            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.TextMeshPro)
            {
                // A text mesh pro object uses a rect transform 
                RectTransform rectTransfrom = transform as RectTransform;
                TextMeshPro textPro = gameObject.GetComponent<TextMeshPro>();

                float xScaleOffset = rectTransfrom.lossyScale.x ;
                float widthOffset = rectTransfrom.rect.width * 0.5f;
                float xScaleWidthOffset = xScaleOffset * widthOffset;
                float marginRightOffset = -textPro.margin.z * xScaleOffset;
                volumeSizeOffset.x = xScaleWidthOffset + marginRightOffset;

                float yScaleOffset = rectTransfrom.lossyScale.y;
                float heightOffset = rectTransfrom.rect.height * 0.5f;
                volumeSizeOffset.y = yScaleOffset * heightOffset;
            }
            else
            {
                volumeSizeOffset.x = transform.localScale.x * 0.5f;
                volumeSizeOffset.y = transform.localScale.y * 0.5f;
                volumeSizeOffset.z = transform.localScale.z * 0.5f;
            }

            return volumeSizeOffset;
        }

        private Vector3 CalculateVolumeSizeOffsetParent()
        {
            bool isParentColliderContainer = UIVolumeParentTransform.GetComponent<Collider>() != null;

            float xOffset = isParentColliderContainer ? UIVolumeParentTransform.GetColliderBounds().extents.x : UIVolumeParentTransform.localScale.x * 0.5f;
            float yOffset = isParentColliderContainer ? UIVolumeParentTransform.GetColliderBounds().extents.y : UIVolumeParentTransform.localScale.y * 0.5f;
            float zOffset = isParentColliderContainer ? UIVolumeParentTransform.GetColliderBounds().extents.z : UIVolumeParentTransform.localScale.z * 0.5f;

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
            Vector3 volumeSizeOffsetParent = CalculateVolumeSizeOffsetParent();
            Vector3 volumeSizeOffset = CalculateVolumeSizeOffset();

            float positionX;
            float positionY;
            float positionZ;

            string[] anchorParsed = NameParse(anchorLocation.ToString());

            // Y Position
            if (anchorParsed[0] == "Top")
            {
                positionY = UIVolumeParentTransform.position.y + (volumeSizeOffsetParent.y) + -(volumeSizeOffset.y);
            }
            else if (anchorParsed[0] == "Center")
            {
                positionY = UIVolumeParentTransform.position.y;
            }
            else
            {
                positionY = UIVolumeParentTransform.position.y + -(volumeSizeOffsetParent.y) + (volumeSizeOffset.y);
            }

            // X Position
            if (anchorParsed[1] == "Left")
            {
                positionX = UIVolumeParentTransform.position.x + -(volumeSizeOffsetParent.x) + (volumeSizeOffset.x);
            }
            else if (anchorParsed[1] == "Center")
            {
                positionX = UIVolumeParentTransform.position.x;
            }
            else
            {
                positionX = UIVolumeParentTransform.position.x + (volumeSizeOffsetParent.x) + -(volumeSizeOffset.x);
            }

            // Z Position
            if (anchorParsed[2] == "Forward")
            {
                positionZ = UIVolumeParentTransform.position.z + -(volumeSizeOffsetParent.z) + (volumeSizeOffset.z);
            }
            else if (anchorParsed[2] == "Center")
            {
                positionZ = UIVolumeParentTransform.position.z;
            }
            else
            {
                positionZ = UIVolumeParentTransform.position.z + (volumeSizeOffsetParent.z) + -(volumeSizeOffset.z);
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
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        items.Add(transform.GetChild(i));
                    }
                }

                if (BackPlateObject != null)
                {
                    items.Remove(BackPlateObject.transform);
                }

                float placementIncrement;
                float startPlacement;

                // Offset the first item to appear in the container
                Bounds bounds = items[0].transform.GetColliderBounds();

                if (axis == Axis.X)
                {
                    float width = Vector3.Distance(GetFacePoint(FacePoint.Left), GetFacePoint(FacePoint.Right));

                    placementIncrement = width / items.Count;

                    startPlacement = GetFacePoint(FacePoint.Left).x + bounds.extents.x + (LeftMargin * width);
                }
                else if (axis == Axis.Y)
                {
                    float height = Vector3.Distance(GetFacePoint(FacePoint.Top), GetFacePoint(FacePoint.Bottom));

                    placementIncrement = height / items.Count;

                    startPlacement = GetFacePoint(FacePoint.Top).y - bounds.extents.y - (TopMargin * height);
                }
                else // Z
                {
                    float depth = Vector3.Distance(GetFacePoint(FacePoint.Forward), GetFacePoint(FacePoint.Back));

                    placementIncrement = depth / items.Count;

                    startPlacement = GetFacePoint(FacePoint.Forward).z + bounds.extents.z + (ForwardMargin * depth);
                }

                foreach (var item in items)
                {
                    if (item.GetComponent<UIVolume>() != null)
                    {
                        UIVolume itemUIVolume = item.gameObject.GetComponent<UIVolume>();
                        itemUIVolume.UseAnchorPositioning = false;
                    }

                    float newPositionX = axis == Axis.X ? startPlacement : transform.position.x;
                    float newPositionY = axis == Axis.Y ? startPlacement : transform.position.y;
                    float newPositionZ = axis == Axis.Z ? startPlacement : transform.position.z;

                    Vector3 newPosition = new Vector3(newPositionX, newPositionY, newPositionZ);

                    item.position = newPosition;

                    if (axis == Axis.Y)
                    {
                        startPlacement -= placementIncrement;
                    }
                    else
                    {
                        startPlacement += placementIncrement;
                    }
                }

                UpdateContainerFill(axis, items);
            }
        }

        private void UpdateDistribution()
        {
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
        }

        private void UpdateContainerFill(Axis distributeAxis, List<Transform> childItems)
        {
            foreach (var item in childItems)
            {
                if (item.GetComponent<UIVolume>() != null)
                {
                    UIVolume itemUIVolume = item.gameObject.GetComponent<UIVolume>();

                    if (distributeAxis == Axis.X)
                    {
                        distributeContainerFillX.UpdateDistributeContainerFillAxis(distributeAxis, itemUIVolume, childItems.Count);
                    }

                    if (distributeAxis == Axis.Y)
                    {
                        distributeContainerFillY.UpdateDistributeContainerFillAxis(distributeAxis, itemUIVolume, childItems.Count);
                    }

                    if (distributeAxis == Axis.Z)
                    {
                        distributeContainerFillY.UpdateDistributeContainerFillAxis(distributeAxis, itemUIVolume, childItems.Count);
                    }
                }
            }
        }

        internal void ResetContainerFillProperties(Axis axis)
        {
            if (axis == Axis.X)
            {
                distributeContainerFillX.ResetContainerFillProperties(this);
            }

            if (axis == Axis.Y)
            {
                distributeContainerFillY.ResetContainerFillProperties(this);
            }

            if (axis == Axis.Z)
            {
                distributeContainerFillZ.ResetContainerFillProperties(this);
            }
        }

        #endregion

        #region Volume Size Origin

        private void SetVolumeSizeOrigin()
        {
            if (GetComponent<Collider>() != null)
            {
                VolumeSizeOrigin = VolumeSizeOrigin.ColliderBounds;
            }
            else if (GetComponent<TextMeshPro>() != null)
            {
                VolumeSizeOrigin = VolumeSizeOrigin.TextMeshPro;
            }
            else
            {
                VolumeSizeOrigin = VolumeSizeOrigin.LocalScale;
            }
        }

        public Vector3 GetVolumeSize()
        {
            if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
            {
                return gameObject.transform.GetColliderBounds().size;
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.TextMeshPro)
            {
                RectTransform rectTransfrom = transform as RectTransform;

                TextMeshPro textPro = gameObject.GetComponent<TextMeshPro>();

                float xOffset = rectTransfrom.lossyScale.x;
                float widthOffset = rectTransfrom.rect.width;
                float scaleWithOffset = xOffset * widthOffset;
                float marginOffset = -textPro.margin.z * xOffset;
                float finalOffsetX = scaleWithOffset + marginOffset;

                float yOffset = rectTransfrom.lossyScale.y;
                float heightOffset = rectTransfrom.rect.height;
                float scaleHeightOffset = yOffset * heightOffset;
                float marginOffsetY = -textPro.margin.y * yOffset;
                float finalOffsetY = scaleHeightOffset + marginOffsetY;

                Vector3 volumeSize = new Vector3(finalOffsetX, finalOffsetY, 0);

                return volumeSize;
            }
            else
            {
                return gameObject.transform.localScale;
            }
        }

        #endregion

        private void PrintMatrix()
        {
            Matrix4x4 localToWoldMatrix = transform.localToWorldMatrix;

            Debug.Log("======== Local to World Matrix ==========");

            Debug.Log(localToWoldMatrix.GetRow(0).ToString("F6"));
            Debug.Log(localToWoldMatrix.GetRow(1).ToString("F6"));
            Debug.Log(localToWoldMatrix.GetRow(2).ToString("F6"));
            Debug.Log(localToWoldMatrix.GetRow(3).ToString("F6"));

            Debug.Log("==================");

            Debug.Log("Local Scale: " + transform.localScale.ToString("F6"));
            Debug.Log("Lossy Scale: " + transform.lossyScale.ToString("F6"));

            if (transform.parent != null)
            {
                Debug.Log("Parent Lossy Scale: " + transform.parent.lossyScale.ToString("F6"));
            }
           
            Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;

            Debug.Log("======== World to Local Matrix ==========");

            Debug.Log(worldToLocalMatrix.GetRow(0).ToString("F6"));
            Debug.Log(worldToLocalMatrix.GetRow(1).ToString("F6"));
            Debug.Log(worldToLocalMatrix.GetRow(2).ToString("F6"));
            Debug.Log(worldToLocalMatrix.GetRow(3).ToString("F6"));

            Debug.Log("==================");
        }
    }
}
