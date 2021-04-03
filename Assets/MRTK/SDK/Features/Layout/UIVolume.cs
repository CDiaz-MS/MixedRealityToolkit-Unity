// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [ExecuteAlways]
    public class UIVolume : BaseVolume
    {
        [SerializeField]
        private AnchorLocation anchorLocation;

        public AnchorLocation AnchorLocation
        {
            get => anchorLocation;
            set => anchorLocation = value;
        }

        [SerializeField]
        private VolumeItemSmoothing anchorPositionSmoothing = new VolumeItemSmoothing();

        public VolumeItemSmoothing AnchorPositionSmoothing
        {
            get => anchorPositionSmoothing;
            set => anchorPositionSmoothing = value;
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

        [SerializeField]
        private VolumeItemSmoothing distributeSmoothing = new VolumeItemSmoothing();

        public VolumeItemSmoothing DistributeSmoothing
        {
            get => distributeSmoothing;
            set => distributeSmoothing = value;
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
        private bool useAnchorPositioning = true;

        public bool UseAnchorPositioning
        {
            get => useAnchorPositioning;
            set => useAnchorPositioning = value;
        }

        [SerializeField]
        private GameObject backPlateObject;

        public GameObject BackPlateObject
        {
            get => backPlateObject;
            set => backPlateObject = value;
        }

        public bool IsRootUIVolume => (transform == rootTransform);

        public Vector3 VolumeCenter => transform.position;

        #region MonoBehaviour Methods

        protected virtual void Start() { }

        public override void Update()
        {
            base.Update();

            UpdateSizingBehaviors();
        }


        public void UpdateSizingBehaviors()
        {
            UpdateDistribution();

            UpdateVolumeSizeMatch();

            UpdateAnchorLocation(AnchorLocation);
        }

        #endregion

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
                Gizmos.DrawWireCube(gameObject.transform.position, VolumeSize);

            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.TextMeshPro)
            {
                RectTransform rectTransfrom = transform as RectTransform;

                TextMeshPro textPro = gameObject.GetComponent<TextMeshPro>();

                float marginOffsetXPosition = gameObject.transform.position.x - ((textPro.margin.z * 0.5f) * rectTransfrom.lossyScale.x);
                float marginOffsetYPosition = gameObject.transform.position.y - ((textPro.margin.y * 0.5f) * rectTransfrom.lossyScale.y);

                Vector3 volumeOriginPosition = new Vector3(marginOffsetXPosition, marginOffsetYPosition, gameObject.transform.position.z);

                Gizmos.DrawWireCube(volumeOriginPosition, VolumeSize);
            }
            else
            {
                Gizmos.DrawWireCube(gameObject.transform.position, VolumeSize);
            }
        }

        private void DrawCornerAndFacePoints()
        {
            if (DrawCornerPoints)
            {
                Gizmos.color = Color.magenta;
                foreach (var point in UIVolumeCorners)
                {
                    Gizmos.DrawSphere(point.Point, 0.01f);
                }
            }

            if (DrawFacePoints)
            {
                Gizmos.color = Color.yellow;
                foreach (var point in UIVolumeFaces)
                {
                    Gizmos.DrawSphere(point.Point, 0.01f);
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
                
                    // The scale of buttons is 1 but the actual size is 32mm x 32mm and a conversion is needed
                    // Check if the prefab is a flexible button
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.x *= 31.25f;
                    }
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
                {
                    newScale.x = includeScaleFactor ? UIVolumeParentTransform.localScale.x * volumeSizeScaleFactorX : UIVolumeParentTransform.localScale.x;
                }
                else // Text Mesh Pro
                {
                    RectTransform rectTransform = transform as RectTransform;

                    float parentChildVolumeRatio = GetTextVolumeRatio(axis);

                    if (parentChildVolumeRatio != 0)
                    {
                        rectTransform.sizeDelta = new Vector2((rectTransform.sizeDelta.x * parentChildVolumeRatio) * volumeSizeScaleFactorX, rectTransform.sizeDelta.y);
                    }
                }
            }

            if (axis == Axis.Y)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.y = includeScaleFactor ? (UIVolumeParentTransform.localScale.y / UIVolumeParentTransform.transform.lossyScale.y) * volumeSizeScaleFactorY : (UIVolumeParentTransform.localScale.y / UIVolumeParentTransform.transform.lossyScale.y);

                    // The scale of buttons is 1 but the actual size is 32mm x 32mm and a conversion is needed
                    // Check if the prefab is a flexible button
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.y *= 31.25f;
                    }
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
                {
                    newScale.y = includeScaleFactor ? UIVolumeParentTransform.localScale.y * volumeSizeScaleFactorY : UIVolumeParentTransform.localScale.y;
                }
                else // Text Mesh Pro
                {
                    RectTransform rectTransform = transform as RectTransform;

                    float parentChildVolumeRatio = GetTextVolumeRatio(axis);

                    if (parentChildVolumeRatio != 0)
                    {
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (rectTransform.sizeDelta.y * parentChildVolumeRatio) * volumeSizeScaleFactorY);
                    }
                }
            }

            if (axis == Axis.Z)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.z = includeScaleFactor ? (UIVolumeParentTransform.localScale.z / UIVolumeParentTransform.transform.lossyScale.z) * volumeSizeScaleFactorZ : (UIVolumeParentTransform.localScale.z / UIVolumeParentTransform.transform.lossyScale.z);

                    // The scale of buttons is 1 but the actual size is 32mm x 32mm and a conversion is needed
                    // Check if the prefab is a flexible button
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.z *= 62.5f;
                    }
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
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

        private float GetTextVolumeRatio(Axis axis)
        {
            RectTransform rectTransform = transform as RectTransform;

            float rectSize;

            if (axis == Axis.X)
            {
                rectSize = rectTransform.rect.width;
            }
            else if (axis == Axis.Y)
            {
                rectSize = rectTransform.rect.height;
            }
            else
            {
                // Z axis scaling is not supported for rect transforms
                return 0;
            }

            float childVolumeAxisDistance = GetAxisDistance(axis);

            float parentVolumeAxisDistance = UIVolumeParentTransform.GetComponent<UIVolume>().GetAxisDistance(axis);

            float parentChildVolumeRatio = parentVolumeAxisDistance / childVolumeAxisDistance;

            return parentChildVolumeRatio;
        }

        #region Maintain Scale 

        public void SetMaintainScale(bool maintainScale, GameObject target)
        {
            UIVolume parentUIVolume = UIVolumeParentTransform.GetComponent<UIVolume>();

            if (parentUIVolume != null)
            {
                ChildVolumeItem childVolumeItem = parentUIVolume.ChildVolumeItems.Find((item) => item.Transform.gameObject == target);
                childVolumeItem.MaintainScale = maintainScale;
            }
        }

        public bool GetMaintainScale(GameObject target)
        {
            UIVolume parentUIVolume = UIVolumeParentTransform.GetComponent<UIVolume>();

            if (parentUIVolume != null)
            {
                ChildVolumeItem childVolumeItem = parentUIVolume.ChildVolumeItems.Find((item) => item.Transform.gameObject == target);
                return childVolumeItem.MaintainScale;
            }

            return false;
        }

        #endregion

        public void UpdateAnchorLocation(AnchorLocation anchorLocation)
        {
            if (UseAnchorPositioning && !IsRootUIVolume)
            {
                Vector3 volumeSizeOffsetParent = CalculateVolumeSizeOffsetParent();
                Vector3 volumeSizeOffset = CalculateVolumeSizeOffset();

                Vector3 newPosition = Vector3.zero;

                string[] anchorParsed = NameParse(anchorLocation.ToString());

                // Y Position
                if (anchorParsed[0] == "Top")
                {
                    newPosition.y = UIVolumeParentTransform.position.y + (volumeSizeOffsetParent.y) + -(volumeSizeOffset.y);
                }
                else if (anchorParsed[0] == "Center")
                {
                    newPosition.y = UIVolumeParentTransform.position.y;
                }
                else
                {
                    newPosition.y = UIVolumeParentTransform.position.y + -(volumeSizeOffsetParent.y) + (volumeSizeOffset.y);
                }

                // X Position
                if (anchorParsed[1] == "Left")
                {
                    newPosition.x = UIVolumeParentTransform.position.x + -(volumeSizeOffsetParent.x) + (volumeSizeOffset.x);
                }
                else if (anchorParsed[1] == "Center")
                {
                    newPosition.x = UIVolumeParentTransform.position.x;
                }
                else
                {
                    newPosition.x = UIVolumeParentTransform.position.x + (volumeSizeOffsetParent.x) + -(volumeSizeOffset.x);
                }

                // Z Position
                if (anchorParsed[2] == "Forward")
                {
                    newPosition.z = UIVolumeParentTransform.position.z + -(volumeSizeOffsetParent.z) + (volumeSizeOffset.z);
                }
                else if (anchorParsed[2] == "Center")
                {
                    newPosition.z = UIVolumeParentTransform.position.z;
                }
                else
                {
                    newPosition.z = UIVolumeParentTransform.position.z + (volumeSizeOffsetParent.z) + -(volumeSizeOffset.z);
                }

                if (newPosition.IsValidVector())
                {
                    transform.position = AnchorPositionSmoothing.Smoothing && Application.isPlaying ? Vector3.Lerp(transform.position, newPosition, AnchorPositionSmoothing.LerpTime * Time.deltaTime) : newPosition;
                }
            }
        }

        #region Axis Distribution

        public void Distribute(Axis axis)
        {
            if (gameObject.transform.childCount > 0)
            {
                List<ChildVolumeItem> itemsToDistribute = ChildVolumeItems;

                // Remove the back plate from the distribution calculation
                foreach (var item in itemsToDistribute.ToList())
                {
                    if (BackPlateObject != null)
                    {
                        if (item.Transform == BackPlateObject.transform)
                        {
                            itemsToDistribute.Remove(item);
                        }
                    }

                    if (!item.Transform.gameObject.activeSelf)
                    {
                        itemsToDistribute.Remove(item);
                    }
                }


                float placementIncrement = GetAxisDistance(axis) / itemsToDistribute.Count;
                float startPlacement;

                // Offset the first item to appear in the container
                Bounds bounds = itemsToDistribute[0].Transform.GetColliderBounds();

                Vector3 volumeAxisDistance = GetAxisDistances();

                if (axis == Axis.X)
                {
                    startPlacement = GetFacePoint(FacePoint.Left).x + bounds.extents.x + (LeftMargin * volumeAxisDistance.x);
                }
                else if (axis == Axis.Y)
                {
                    startPlacement = GetFacePoint(FacePoint.Top).y - bounds.extents.y - (TopMargin * volumeAxisDistance.y);
                }
                else // Z
                {
                    startPlacement = GetFacePoint(FacePoint.Forward).z + bounds.extents.z + (ForwardMargin * volumeAxisDistance.z);
                }

                foreach (var item in itemsToDistribute)
                {
                    //  Anchor positioning cannot be acitve for distribution placement
                    if (item.UIVolume != null)
                    {
                        item.UIVolume.UseAnchorPositioning = false;
                    }

                    float newPositionX = axis == Axis.X ? startPlacement : transform.position.x;
                    float newPositionY = axis == Axis.Y ? startPlacement : transform.position.y;
                    float newPositionZ = axis == Axis.Z ? startPlacement : transform.position.z;

                    Vector3 newPosition = new Vector3(newPositionX, newPositionY, newPositionZ);

                    if (newPosition.IsValidVector())
                    {
                        item.Transform.position = DistributeSmoothing.Smoothing && Application.isPlaying? Vector3.Lerp(item.Transform.position, newPosition, DistributeSmoothing.LerpTime * Time.deltaTime) : newPosition;
                    }

                    // Reverse the Y Axis, start placement from the top of the container
                    if (axis == Axis.Y)
                    {
                        startPlacement -= placementIncrement;
                    }
                    else
                    {
                        startPlacement += placementIncrement;
                    }
                }

                UpdateContainerFill(axis, itemsToDistribute);

                UIVolumeParentTransform.GetComponent<UIVolume>().UpdateCornerPoints();
                UIVolumeParentTransform.GetComponent<UIVolume>().UpdateFacePoints();
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

            UpdatePoints();
        }

        private void UpdateContainerFill(Axis distributeAxis, List<ChildVolumeItem> childItems)
        {
            foreach (var item in childItems)
            {
                if (item.UIVolume != null)
                {
                    if (distributeAxis == Axis.X)
                    {
                        distributeContainerFillX.UpdateDistributeContainerFillAxis(distributeAxis, item.UIVolume, childItems.Count);
                    }

                    if (distributeAxis == Axis.Y)
                    {
                        distributeContainerFillY.UpdateDistributeContainerFillAxis(distributeAxis, item.UIVolume, childItems.Count);
                    }

                    if (distributeAxis == Axis.Z)
                    {
                        distributeContainerFillY.UpdateDistributeContainerFillAxis(distributeAxis, item.UIVolume, childItems.Count);
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

        public void SwitchChildVolumes(UIVolume child, UIVolume targetVolume)
        {
            UIVolume volumeToSwitch = DirectChildUIVolumes.Find((item) => item == child);

            volumeToSwitch.transform.SetParent(targetVolume.transform, false);
        }

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
